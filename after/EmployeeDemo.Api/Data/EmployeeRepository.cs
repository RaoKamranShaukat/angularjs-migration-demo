using System.Data;
using EmployeeDemo.Api.Models;
using Microsoft.Data.SqlClient;

namespace EmployeeDemo.Api.Data;

// Same stored procedures, same ADO.NET style as the "before" app. What changed:
//   System.Data.SqlClient        -> Microsoft.Data.SqlClient
//   ConfigurationManager         -> IConfiguration (injected)
//   synchronous calls            -> async/await (threads are freed while SQL Server works)
//   DateTime for HireDate        -> DateOnly
public class EmployeeRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("EmployeeDemo")
        ?? throw new InvalidOperationException("Connection string 'EmployeeDemo' is missing.");

    public async Task<List<Employee>> GetAllAsync(string? search)
    {
        var employees = new List<Employee>();

        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = CreateCommand(conn, "dbo.Employee_GetAll");
        cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 100).Value =
            string.IsNullOrWhiteSpace(search) ? DBNull.Value : search.Trim();

        await conn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            employees.Add(MapEmployee(reader));
        }

        return employees;
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = CreateCommand(conn, "dbo.Employee_GetById");
        cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = id;

        await conn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapEmployee(reader) : null;
    }

    // Returns the new EmployeeId.
    public async Task<int> InsertAsync(Employee employee)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = CreateCommand(conn, "dbo.Employee_Insert");
        AddEmployeeParameters(cmd, employee);

        await conn.OpenAsync();
        return (int)(await cmd.ExecuteScalarAsync())!;
    }

    // Returns false if the employee was not found.
    public async Task<bool> UpdateAsync(Employee employee)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = CreateCommand(conn, "dbo.Employee_Update");
        cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = employee.EmployeeId;
        AddEmployeeParameters(cmd, employee);

        await conn.OpenAsync();
        return (int)(await cmd.ExecuteScalarAsync())! > 0;
    }

    // Returns false if the employee was not found.
    public async Task<bool> DeleteAsync(int id)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = CreateCommand(conn, "dbo.Employee_Delete");
        cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = id;

        await conn.OpenAsync();
        return (int)(await cmd.ExecuteScalarAsync())! > 0;
    }

    public async Task<List<Department>> GetDepartmentsAsync()
    {
        var departments = new List<Department>();

        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = CreateCommand(conn, "dbo.Department_GetAll");

        await conn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            departments.Add(new Department(
                reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                reader.GetString(reader.GetOrdinal("Name"))));
        }

        return departments;
    }

    // ---- helpers ----

    private static SqlCommand CreateCommand(SqlConnection conn, string procedureName) =>
        new(procedureName, conn) { CommandType = CommandType.StoredProcedure };

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

        cmd.Parameters.Add("@HireDate", SqlDbType.Date).Value = e.HireDate;
        cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = e.IsActive;
    }

    private static Employee MapEmployee(SqlDataReader r) => new()
    {
        EmployeeId = r.GetInt32(r.GetOrdinal("EmployeeId")),
        FirstName = r.GetString(r.GetOrdinal("FirstName")),
        LastName = r.GetString(r.GetOrdinal("LastName")),
        Email = r.GetString(r.GetOrdinal("Email")),
        DepartmentId = r.GetInt32(r.GetOrdinal("DepartmentId")),
        DepartmentName = r.GetString(r.GetOrdinal("DepartmentName")),
        Salary = r.GetDecimal(r.GetOrdinal("Salary")),
        HireDate = r.GetFieldValue<DateOnly>(r.GetOrdinal("HireDate")),
        IsActive = r.GetBoolean(r.GetOrdinal("IsActive"))
    };
}
