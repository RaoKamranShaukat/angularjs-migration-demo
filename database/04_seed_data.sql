-- 04_seed_data.sql
-- Sample data so both apps show something on first run. Only inserts into empty tables.

USE EmployeeDemo;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Departments)
BEGIN
    INSERT INTO dbo.Departments (Name)
    VALUES (N'Engineering'), (N'Finance'), (N'Human Resources'), (N'Sales'), (N'Support');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Employees)
BEGIN
    INSERT INTO dbo.Employees (FirstName, LastName, Email, DepartmentId, Salary, HireDate, IsActive)
    SELECT v.FirstName, v.LastName, v.Email, d.DepartmentId, v.Salary, v.HireDate, v.IsActive
    FROM (VALUES
        (N'Ayesha', N'Khan',    N'ayesha.khan@example.com',    N'Engineering',     250000, '2019-03-11', 1),
        (N'Bilal',  N'Ahmed',   N'bilal.ahmed@example.com',    N'Finance',         180000, '2020-07-01', 1),
        (N'Sara',   N'Malik',   N'sara.malik@example.com',     N'Human Resources', 150000, '2021-01-18', 1),
        (N'Omar',   N'Farooq',  N'omar.farooq@example.com',    N'Sales',           160000, '2018-10-05', 0),
        (N'Hina',   N'Raza',    N'hina.raza@example.com',      N'Support',         120000, '2022-05-23', 1),
        (N'Usman',  N'Tariq',   N'usman.tariq@example.com',    N'Engineering',     310000, '2017-02-14', 1)
    ) AS v (FirstName, LastName, Email, DepartmentName, Salary, HireDate, IsActive)
    INNER JOIN dbo.Departments d ON d.Name = v.DepartmentName;
END
GO

-- Demo login: admin / Admin@123  (public demo credentials, shown on the login page)
-- Hash = PBKDF2-SHA256(password, salt, 100000 iterations, 32 bytes), produced with:
--   new Rfc2898DeriveBytes("Admin@123", salt, 100000, HashAlgorithmName.SHA256).GetBytes(32)
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = N'admin')
BEGIN
    INSERT INTO dbo.Users (Username, FullName, PasswordHash, PasswordSalt, IsActive)
    VALUES (N'admin', N'Demo Admin',
            0x45425E4DF266BAFA17D482506CE3BCF3391632ABE4436B204D68EEF506FC6C21,
            0xA01C8FD9DE1F6F0E83E99C0C89162828,
            1);
END
GO
