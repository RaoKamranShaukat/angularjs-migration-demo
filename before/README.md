# Before: AngularJS 1.8 + ASP.NET Web API 2 (.NET Framework 4.8)

This is the "legacy" version, written the way many real apps from 2014–2019 were written.

## Run it

1. Create the database first (see [../database/README.md](../database/README.md)).
2. In `EmployeeDemo.Web/Web.config`, set `Server=` to your SQL Server instance.
3. Open `EmployeeDemo.Before.sln` in Visual Studio (needs the "ASP.NET and web development"
   workload with .NET Framework 4.8 targeting pack) and press **F5**.
   NuGet packages restore automatically.
4. The app opens at http://localhost:52100/ and asks you to sign in.
   Always start from the site root. Files in `app/` are templates that AngularJS loads into
   `index.html`; if one is opened directly, the server redirects to `/`.
   Demo account: **admin / Admin@123** (or click "Fill in" on the login page).

Command line alternative (Developer PowerShell):

```powershell
msbuild EmployeeDemo.Before.sln -restore -p:RestorePackagesConfig=true
& "C:\Program Files\IIS Express\iisexpress.exe" /path:"$PWD\EmployeeDemo.Web" /port:52100
```

## What is inside

```
EmployeeDemo.Web/
├── Global.asax(.cs)          App start; returns real 401s (not redirects) for /api/*
├── App_Start/WebApiConfig.cs Attribute routes, JSON only, [Authorize] on everything by default
├── Web.config                Connection string, Forms auth cookie, IIS handlers, binding redirects
├── packages.config           NuGet packages (old style)
├── Models/                   Employee, Department, AppUser/LoginRequest
├── Data/                     EmployeeRepository, UserRepository: ADO.NET -> stored procedures (synchronous)
├── Security/PasswordHasher.cs  PBKDF2-SHA256 password check
├── Modules/TemplateRedirectModule.cs  Sends direct browser visits to app/*.html back to the app
├── Controllers/              Auth, Employees, Departments (ApiController)
├── index.html                Shell: top bar + ng-view. AngularJS from CDN, no build step
├── css/app.css               Theme (shared with the "after" app so both look the same)
├── images/                   Logo and illustrations (hand-made SVG)
└── app/
    ├── app.module.js              Routes (+ resolve guard), 401 interceptor, initials filter
    ├── auth.service.js            login / logout / current user
    ├── shell.controller.js        Top bar, sign out
    ├── login.controller.js + login.html                 Login page
    ├── employee.service.js        $http calls (returns promises)
    ├── employee-list.controller.js + employee-list.html   Stats, list, search, delete
    └── employee-form.controller.js + employee-form.html   Add / edit with validation
```

## Login: how it works

1. `login.html` posts `{ Username, Password }` to `api/auth/login`.
2. `AuthController` loads the user with `dbo.User_GetByUsername` and checks the password with
   PBKDF2 (`PasswordHasher`). The database stores only a hash and a salt.
3. On success, `FormsAuthentication.SetAuthCookie` sets an **HttpOnly, SameSite=Lax** cookie.
   The browser sends it automatically with every API call.
4. `WebApiConfig` adds a global `AuthorizeAttribute`, so every endpoint needs that cookie,
   except the ones marked `[AllowAnonymous]` (login, logout).
5. In AngularJS, every protected route has a `resolve` that calls `api/auth/me`. Any 401 is caught
   by `authInterceptor`, which sends the user to `#!/login`.

## API

All endpoints except login/logout return **401** when not signed in.

| Method | URL | Result |
|--------|-----|--------|
| POST | `api/auth/login` | 200 + `{ Username, FullName }` and auth cookie, or 401 |
| POST | `api/auth/logout` | 204, cookie removed |
| GET | `api/auth/me` | 200 + current user, or 401 |
| GET | `api/employees?search=` | 200 + list |
| GET | `api/employees/{id}` | 200, or 404 |
| POST | `api/employees` | 201 + Location header, 400 invalid, 409 duplicate email |
| PUT | `api/employees/{id}` | 204, 400, 404, 409 |
| DELETE | `api/employees/{id}` | 204, or 404 |
| GET | `api/departments` | 200 + list |

JSON uses **PascalCase** property names (`EmployeeId`, `FirstName`), which is the Web API 2 default.
