# Database (shared by `before` and `after`)

Both apps use the **same database and the same stored procedures**. The migration changes the
application code only, so the database stays as it is.

## Scripts (run in order)

| Script | What it does |
|--------|--------------|
| `01_create_database.sql` | Creates the `EmployeeDemo` database |
| `02_tables.sql` | `Departments`, `Employees` and `Users` tables |
| `03_stored_procedures.sql` | All CRUD procedures + login lookup |
| `04_seed_data.sql` | Sample departments, employees and the demo login (`admin` / `Admin@123`) |

All scripts can be run more than once.

## Run

Option A: open each file in SSMS and execute it, in order.

Option B: use PowerShell with `sqlcmd`:

```powershell
cd database
.\setup.ps1 -Server "localhost"        # or ".\SQLEXPRESS", etc.
```

## Stored procedures

| Procedure | Parameters | Returns |
|-----------|-----------|---------|
| `Department_GetAll` | none | DepartmentId, Name |
| `Employee_GetAll` | `@Search` (optional) | Employee rows with DepartmentName |
| `Employee_GetById` | `@EmployeeId` | One row, or no rows |
| `Employee_Insert` | all fields except id | New `EmployeeId` |
| `Employee_Update` | `@EmployeeId` + all fields | `RowsAffected` (0 = not found) |
| `Employee_Delete` | `@EmployeeId` | `RowsAffected` (0 = not found) |
| `User_GetByUsername` | `@Username` | User row with `PasswordHash` + `PasswordSalt`, or no rows |

Passwords are stored as PBKDF2-SHA256 (100,000 iterations, 16-byte salt, 32-byte hash).
The password is checked in C#, not in SQL. .NET Framework and .NET 8 produce the same hash,
so the same `Users` table works for both apps.
