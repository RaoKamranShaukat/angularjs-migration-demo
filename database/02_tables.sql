-- 02_tables.sql
-- Two tables: Departments (lookup) and Employees (main CRUD table).

USE EmployeeDemo;
GO

IF OBJECT_ID(N'dbo.Departments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Departments
    (
        DepartmentId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Departments PRIMARY KEY,
        Name         NVARCHAR(100)     NOT NULL CONSTRAINT UQ_Departments_Name UNIQUE
    );
END
GO

IF OBJECT_ID(N'dbo.Employees', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Employees
    (
        EmployeeId   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Employees PRIMARY KEY,
        FirstName    NVARCHAR(50)      NOT NULL,
        LastName     NVARCHAR(50)      NOT NULL,
        Email        NVARCHAR(100)     NOT NULL CONSTRAINT UQ_Employees_Email UNIQUE,
        DepartmentId INT               NOT NULL
            CONSTRAINT FK_Employees_Departments REFERENCES dbo.Departments(DepartmentId),
        Salary       DECIMAL(18,2)     NOT NULL CONSTRAINT CK_Employees_Salary CHECK (Salary >= 0),
        HireDate     DATE              NOT NULL,
        IsActive     BIT               NOT NULL CONSTRAINT DF_Employees_IsActive DEFAULT (1),
        CreatedAt    DATETIME2(0)      NOT NULL CONSTRAINT DF_Employees_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
END
GO

-- Login users. Passwords are never stored: only a PBKDF2-SHA256 hash (100,000 iterations) + random salt.
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserId       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
        Username     NVARCHAR(50)      NOT NULL CONSTRAINT UQ_Users_Username UNIQUE,
        FullName     NVARCHAR(100)     NOT NULL,
        PasswordHash VARBINARY(32)     NOT NULL,
        PasswordSalt VARBINARY(16)     NOT NULL,
        IsActive     BIT               NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1)
    );
END
GO
