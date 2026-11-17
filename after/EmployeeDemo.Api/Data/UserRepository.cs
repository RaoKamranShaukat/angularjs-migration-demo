using System.Data;
using EmployeeDemo.Api.Models;
using Microsoft.Data.SqlClient;

namespace EmployeeDemo.Api.Data;

public class UserRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("EmployeeDemo")
        ?? throw new InvalidOperationException("Connection string 'EmployeeDemo' is missing.");

    // Returns null if the username does not exist.
    public async Task<AppUser?> GetByUsernameAsync(string username)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("dbo.User_GetByUsername", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;

        await conn.OpenAsync();
        await using var r = await cmd.ExecuteReaderAsync();
        if (!await r.ReadAsync())
            return null;

        return new AppUser(
            r.GetInt32(r.GetOrdinal("UserId")),
            r.GetString(r.GetOrdinal("Username")),
            r.GetString(r.GetOrdinal("FullName")),
            (byte[])r["PasswordHash"],
            (byte[])r["PasswordSalt"],
            r.GetBoolean(r.GetOrdinal("IsActive")));
    }
}
