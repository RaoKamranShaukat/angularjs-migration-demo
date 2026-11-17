-- 03_stored_procedures.sql
-- All data access goes through these procedures.
-- The "before" (.NET Framework) and "after" (.NET 8) apps call exactly the same ones,
-- so the database does not change during the migration.
-- CREATE OR ALTER needs SQL Server 2016 SP1 or later.

USE EmployeeDemo;
GO

-- Lookup list for the Department dropdown.
CREATE OR ALTER PROCEDURE dbo.Department_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DepartmentId, Name
    FROM dbo.Departments
    ORDER BY Name;
END
GO

-- List employees. @Search is optional and matches first name, last name or email.
CREATE OR ALTER PROCEDURE dbo.Employee_GetAll
    @Search NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT e.EmployeeId, e.FirstName, e.LastName, e.Email,
           e.DepartmentId, d.Name AS DepartmentName,
           e.Salary, e.HireDate, e.IsActive
    FROM dbo.Employees e
    INNER JOIN dbo.Departments d ON d.DepartmentId = e.DepartmentId
    WHERE @Search IS NULL
       OR e.FirstName LIKE N'%' + @Search + N'%'
       OR e.LastName  LIKE N'%' + @Search + N'%'
       OR e.Email     LIKE N'%' + @Search + N'%'
    ORDER BY e.LastName, e.FirstName;
END
GO

-- One employee by id. Returns no rows if not found.
CREATE OR ALTER PROCEDURE dbo.Employee_GetById
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT e.EmployeeId, e.FirstName, e.LastName, e.Email,
           e.DepartmentId, d.Name AS DepartmentName,
           e.Salary, e.HireDate, e.IsActive
    FROM dbo.Employees e
    INNER JOIN dbo.Departments d ON d.DepartmentId = e.DepartmentId
    WHERE e.EmployeeId = @EmployeeId;
END
GO

-- Insert. Returns the new EmployeeId as a single value (read it with ExecuteScalar).
CREATE OR ALTER PROCEDURE dbo.Employee_Insert
    @FirstName    NVARCHAR(50),
    @LastName     NVARCHAR(50),
    @Email        NVARCHAR(100),
    @DepartmentId INT,
    @Salary       DECIMAL(18,2),
    @HireDate     DATE,
    @IsActive     BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Employees (FirstName, LastName, Email, DepartmentId, Salary, HireDate, IsActive)
    VALUES (@FirstName, @LastName, @Email, @DepartmentId, @Salary, @HireDate, @IsActive);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS EmployeeId;
END
GO

-- Update. Returns rows affected (0 = employee not found).
CREATE OR ALTER PROCEDURE dbo.Employee_Update
    @EmployeeId   INT,
    @FirstName    NVARCHAR(50),
    @LastName     NVARCHAR(50),
    @Email        NVARCHAR(100),
    @DepartmentId INT,
    @Salary       DECIMAL(18,2),
    @HireDate     DATE,
    @IsActive     BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Employees
    SET FirstName    = @FirstName,
        LastName     = @LastName,
        Email        = @Email,
        DepartmentId = @DepartmentId,
        Salary       = @Salary,
        HireDate     = @HireDate,
        IsActive     = @IsActive
    WHERE EmployeeId = @EmployeeId;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- Login lookup. The application verifies the password hash in C#, never in SQL.
CREATE OR ALTER PROCEDURE dbo.User_GetByUsername
    @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT UserId, Username, FullName, PasswordHash, PasswordSalt, IsActive
    FROM dbo.Users
    WHERE Username = @Username;
END
GO

-- Delete. Returns rows affected (0 = employee not found).
CREATE OR ALTER PROCEDURE dbo.Employee_Delete
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Employees
    WHERE EmployeeId = @EmployeeId;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
