using EmployeeDemo.Api.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

// Program.cs replaces Global.asax + App_Start/WebApiConfig.cs + most of Web.config.
var builder = WebApplication.CreateBuilder(args);

// ---- Services (dependency injection, built in) ----

// BEFORE: controllers did "new EmployeeRepository()". AFTER: ASP.NET Core creates and injects them.
builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<UserRepository>();

// JSON: ASP.NET Core uses camelCase ("employeeId") by default. Web API 2 used PascalCase ("EmployeeId").
builder.Services.AddControllers();

// BEFORE: <authentication mode="Forms"> in Web.config + FormsAuthentication.SetAuthCookie.
// AFTER: cookie authentication middleware. Same idea: an encrypted, HttpOnly cookie.
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = ".EmployeeDemoAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;                    // blocks cross-site POSTs (CSRF)
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // use "Always" on HTTPS hosting
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;

        // BEFORE: Response.SuppressFormsAuthenticationRedirect in Global.asax.
        // AFTER: an API answers 401/403 instead of redirecting to a login page.
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

// BEFORE: config.Filters.Add(new AuthorizeAttribute()) in WebApiConfig.
// AFTER: a fallback policy. Every endpoint needs a signed-in user unless it says [AllowAnonymous].
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

// ---- Request pipeline (middleware runs in this order) ----

// Serves the built Angular app from wwwroot (production). Runs before authentication,
// so the login page, scripts and images load without a cookie.
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Angular routes like /employees/5/edit are handled in the browser, so the server returns index.html.
app.MapFallbackToFile("index.html").AllowAnonymous();

app.Run();
