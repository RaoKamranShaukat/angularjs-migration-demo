using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using EmployeeDemo.Web.Models;

namespace EmployeeDemo.Web.Data
{
    // Plain ADO.NET: every method opens a connection, runs one stored procedure and maps the result.
    // Synchronous on purpose: this is how most .NET Framework apps were written.
    public class EmployeeRepository
    {
        private readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["EmployeeDemo"].ConnectionString;

        public List<Employee> GetAll(string search)
        {
            var employees = new List<Employee>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = CreateCommand(conn, "dbo.Employee_GetAll"))
            {
                cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 100).Value =
                    string.IsNullOrWhiteSpace(search) ? (object)DBNull.Value : search.Trim();

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(MapEmployee(reader));
                    }
                }
            }

            return employees;
        }

        public Employee GetById(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = CreateCommand(conn, "dbo.Employee_GetById"))
            {
                cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = id;

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() ? MapEmployee(reader) : null;
                }
            }
        }

        // Returns the new EmployeeId.
        public int Insert(Employee employee)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = CreateCommand(conn, "dbo.Employee_Insert"))
            {
                AddEmployeeParameters(cmd, employee);

                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        // Returns false if the employee was not found.
        public bool Update(Employee employee)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = CreateCommand(conn, "dbo.Employee_Update"))
            {
                cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employee.EmployeeId;
                AddEmployeeParameters(cmd, employee);

                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        // Returns false if the employee was not found.
        public bool Delete(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = CreateCommand(conn, "dbo.Employee_Delete"))
            {
                cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = id;

                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public List<Department> GetDepartments()
        {
            var departments = new List<Department>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = CreateCommand(conn, "dbo.Department_GetAll"))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }
                }
            }

            return departments;
        }

        // ---- helpers ----

        private static SqlCommand CreateCommand(SqlConnection conn, string procedureName)
        {
            return new SqlCommand(procedureName, conn) { CommandType = CommandType.StoredProcedure };
        }

        private static void AddEmployeeParameters(SqlCommand cmd, Employee e)
        {
            cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = e.FirstName.Trim();
            cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = e.LastName.Trim();
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = e.Email.Trim();
            cmd.Parameters.Add("@DepartmentId", SqlDbType.Int).Value = e.DepartmentId;

            var salary = cmd.Parameters.Add("@Salary", SqlDbType.Decimal);
            salary.Precision = 18;
            salary.Scale = 2;
            salary.Value = e.Salary;

            cmd.Parameters.Add("@HireDate", SqlDbType.Date).Value = e.HireDate.Date;
            cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = e.IsActive;
        }

        private static Employee MapEmployee(SqlDataReader r)
        {
            return new Employee
            {
                EmployeeId = r.GetInt32(r.GetOrdinal("EmployeeId")),
                FirstName = r.GetString(r.GetOrdinal("FirstName")),
                LastName = r.GetString(r.GetOrdinal("LastName")),
                Email = r.GetString(r.GetOrdinal("Email")),
                DepartmentId = r.GetInt32(r.GetOrdinal("DepartmentId")),
                DepartmentName = r.GetString(r.GetOrdinal("DepartmentName")),
                Salary = r.GetDecimal(r.GetOrdinal("Salary")),
                HireDate = r.GetDateTime(r.GetOrdinal("HireDate")),
                IsActive = r.GetBoolean(r.GetOrdinal("IsActive"))
            };
        }
    }
}
