# Legacy to Modern: Employee Management Migration Demo

The same small Employee CRUD app built twice:

| Folder      | Stack |
|-------------|-------|
| `before/`   | AngularJS 1.8 + ASP.NET Web API 2 on .NET Framework 4.8 |
| `after/`    | Angular 22 + ASP.NET Core Web API on .NET 10 (current LTS) |
| `database/` | SQL Server tables and stored procedures, **shared by both versions** |

> Work in progress: before/after screenshots are coming.

## How to run

1. **Database:** [database/README.md](database/README.md)
2. **Before** (AngularJS + .NET Framework 4.8): [before/README.md](before/README.md)
3. **After** (Angular 22 + .NET 10): [after/README.md](after/README.md)

Demo login for both apps: **admin / Admin@123**

## What the migration changed

The full file-by-file mapping and the problems solved are in [after/README.md](after/README.md).
In short:

| Layer | Before | After |
|-------|--------|-------|
| Database | SQL Server tables + stored procedures | **Unchanged** |
| Data access | ADO.NET, `System.Data.SqlClient`, synchronous | ADO.NET, `Microsoft.Data.SqlClient`, async |
| API | ASP.NET Web API 2, `Global.asax`, `Web.config` | ASP.NET Core, `Program.cs`, `appsettings.json` |
| Login | Forms Authentication cookie | ASP.NET Core cookie authentication (same users, same password hashes) |
| Frontend | AngularJS 1.8 controllers + `$scope` + `$http` | Angular 22 standalone components + signals + `HttpClient` |
| Routing | `ngRoute`, `#!/employees` | Angular Router, `/employees`, route guard |
| Styling | Bootstrap 3 from a CDN + theme | Same theme, no Bootstrap |
