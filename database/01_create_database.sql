-- 01_create_database.sql
-- Creates the demo database. Safe to run more than once.

IF DB_ID(N'EmployeeDemo') IS NULL
BEGIN
    CREATE DATABASE EmployeeDemo;
END
GO
