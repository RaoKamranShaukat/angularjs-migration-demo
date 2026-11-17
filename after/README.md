# After: Angular 22 + ASP.NET Core Web API (.NET 10)

The same Employee Manager app as [`before/`](../before), migrated to the current stack.
Same database, same stored procedures, same screens, same login. Only the technology changed.

> **Why .NET 10 and not .NET 8?** .NET 8 reached end of support on 10 November 2026.
> .NET 10 is the current LTS release (supported until November 2028).

## Run it

You need: the database from [`../database`](../database/README.md), the .NET 10 SDK, and Node.js 22.22+ or 24.15+
(Visual Studio 2026 includes Node 24 in `...\MSBuild\Microsoft\VisualStudio\NodeJs`).

1. Set your SQL Server in `EmployeeDemo.Api/appsettings.Development.json`
   (create it if missing; it is not committed):

   ```json
   { "ConnectionStrings": { "EmployeeDemo": "Server=localhost;Database=EmployeeDemo;Integrated Security=True;TrustServerCertificate=True" } }
   ```

2. **Development** (two terminals, hot reload):

   ```powershell
   dotnet run --project EmployeeDemo.Api           # API on http://localhost:5100
   ```
   ```powershell
   cd client
   npm install
   npm start                                       # app on http://localhost:4200
   ```

   The Angular dev server forwards `/api/*` to the API (`client/proxy.conf.json`), so the browser
   sees one site and the login cookie just works. No CORS needed.

3. **Production-style** (one server, like the old app):

   ```powershell
   cd client
   npm run build                                   # writes the app into EmployeeDemo.Api/wwwroot
   cd ..
   dotnet run --project EmployeeDemo.Api           # app + API on http://localhost:5100
   ```

Demo login: **admin / Admin@123**.

## What is inside

```
after/
├── EmployeeDemo.After.slnx           Solution (new XML format)
├── EmployeeDemo.Api/                 ASP.NET Core Web API (.NET 10)
│   ├── Program.cs                    Startup, DI, cookie auth, authorization policy
│   ├── appsettings.json              Settings + connection string
│   ├── Models/                       Employee, Department, AppUser / LoginRequest / UserInfo
│   ├── Data/                         EmployeeRepository, UserRepository: async ADO.NET -> stored procedures
│   ├── Security/PasswordHasher.cs    PBKDF2-SHA256 (same hashes as the old app)
│   └── Controllers/                  Auth, Employees, Departments
└── client/                           Angular 22 (standalone components, signals, no zone.js)
    ├── proxy.conf.json               /api -> http://localhost:5100 during development
    ├── public/images/                Same SVG illustrations as before
    └── src/
        ├── styles.css                Same theme as before + a small base that replaces Bootstrap 3
        └── app/
            ├── app.ts / app.html     Top bar + <router-outlet>
            ├── app.config.ts         HttpClient + interceptor + router
            ├── app.routes.ts         Routes + guard
            ├── core/                 models, employee.service, auth.service, auth.guard, auth.interceptor
            ├── shared/               initials.pipe
            └── pages/                login, employee-list, employee-form
```

## Before → After: file by file

### Backend (.NET Framework 4.8 → .NET 10)

| Before | After | What changed |
|--------|-------|--------------|
| `Global.asax`, `App_Start/WebApiConfig.cs` | `Program.cs` | One startup file instead of three places |
| `Web.config` (connection string, auth, handlers, binding redirects) | `appsettings.json` + `Program.cs` | No XML, no binding redirects |
| Classic `.csproj` + `packages.config` (every file listed) | 10-line SDK-style `.csproj` | Files are found automatically |
| `ApiController` + `[RoutePrefix]` | `ControllerBase` + `[ApiController]` + `[Route]` | Invalid models get an automatic 400 |
| `new EmployeeRepository()` in each controller | Constructor injection (`AddScoped`) | Built-in dependency injection |
| `System.Data.SqlClient`, synchronous | `Microsoft.Data.SqlClient`, `async/await` | **Same stored procedures**, threads freed while SQL works |
| `ConfigurationManager.ConnectionStrings[...]` | `IConfiguration.GetConnectionString(...)` | Per-environment files (`appsettings.Development.json`) |
| `DateTime HireDate` | `DateOnly HireDate` | JSON `"2019-03-11"`, no time zone bugs |
| `FormsAuthentication.SetAuthCookie` | `HttpContext.SignInAsync` (cookie auth) | Claims-based, same HttpOnly + SameSite cookie |
| Global `AuthorizeAttribute` filter | Authorization `FallbackPolicy` | Secure by default, `[AllowAnonymous]` to opt out |
| `SuppressFormsAuthenticationRedirect` in `Global.asax` | `OnRedirectToLogin` → 401 | APIs return 401, not a redirect |
| `TemplateRedirectModule` (`IHttpModule`) | Not needed | Clean URLs + `MapFallbackToFile("index.html")` |
| `new Rfc2898DeriveBytes(...)` + hand-written compare | `Rfc2898DeriveBytes.Pbkdf2(...)` + `CryptographicOperations.FixedTimeEquals` | Same bytes, less code |

### Frontend (AngularJS 1.8 → Angular 22)

| Before | After | What changed |
|--------|-------|--------------|
| `index.html` with `<script>` tags from a CDN | Angular CLI build (`npm run build`) | TypeScript, bundling, tree-shaking: ~94 kB to download |
| `app.module.js`: `angular.module` + `.config` | `app.config.ts`: providers | |
| `$routeProvider`, URLs like `#!/employees` | `app.routes.ts`, URLs like `/employees` | Clean URLs |
| Route `resolve: { user: requireUser }` | `authGuard` (`CanActivateFn`) | |
| `$httpProvider.interceptors` + `$injector.get()` workaround | `authInterceptor` (`HttpInterceptorFn`) | Plain function, `inject()` just works |
| `employee.service.js`: `$http`, promises | `employee.service.ts`: `HttpClient`, typed Observables | |
| No types | `core/models.ts` interfaces | Typos are compile errors, not blank cells |
| `ShellController` on `<body>` + `ng-view` | `App` component + `<router-outlet>` | |
| `EmployeeListController` + `$scope` | `EmployeeList` component + signals | `$scope.x` → `signal`, stats → `computed()` |
| `EmployeeFormController` + `ng-model` + rules in HTML | `EmployeeForm` + typed Reactive Form | Rules in TypeScript |
| `$routeParams.id` | `id = input<string>()` (`withComponentInputBinding`) | |
| `LoginController` | `LoginPage` component | |
| `.filter('initials')` | `InitialsPipe` | |
| `ng-repeat`, `ng-if`, `ng-show` | `@for`, `@if` | Built-in control flow |
| `ng-click`, `ng-submit`, `ng-class` | `(click)`, `(ngSubmit)`, `[class.x]` | |
| Two password inputs with `ng-if` (can't change `type`) | `[type]="showPassword() ? 'text' : 'password'"` | |
| Bootstrap 3 (end of life) | Small base CSS in `styles.css` | Same look, one less dependency |
| zone.js digest cycle | Zoneless, signals drive updates | |

## Problems solved during the migration

1. **JSON property names changed.** Web API 2 sent PascalCase (`EmployeeId`); ASP.NET Core sends camelCase
   (`employeeId`). An old AngularJS client pointed at the new API would show empty cells with no error.
   The new client uses camelCase models; if you must keep an old client running during a phased
   migration, set `JsonSerializerOptions.PropertyNamingPolicy = null` on the new API.
2. **SQL connection refused after upgrading the driver.** `Microsoft.Data.SqlClient` encrypts connections
   by default. A local SQL Server with a self-signed certificate rejects it until you add
   `TrustServerCertificate=True` (dev) or install a trusted certificate (production).
3. **Hire date saved one day early.** The AngularJS app sent a JavaScript `Date`, which becomes UTC
   (in Pakistan, midnight 1 Jan becomes 31 Dec 19:00Z). Fixed for good with `DateOnly` on the server and
   `"yyyy-MM-dd"` strings in the form, so no conversion happens anywhere.
4. **Login redirects instead of 401.** Forms Authentication redirected AJAX calls to a login page.
   ASP.NET Core cookie auth does the same by default; `OnRedirectToLogin` returns a 401 instead.
5. **Same users, new framework.** Passwords are PBKDF2 hashes stored in SQL Server. .NET 10's
   `Rfc2898DeriveBytes.Pbkdf2` produces exactly the same bytes, so no user has to reset a password.
6. **.NET 8 end of support.** The target moved to .NET 10 LTS so the migrated app is supported until 2028.
