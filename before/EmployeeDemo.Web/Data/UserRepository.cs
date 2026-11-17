using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using EmployeeDemo.Web.Models;

namespace EmployeeDemo.Web.Data
{
    public class UserRepository
    {
        private readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["EmployeeDemo"].ConnectionString;

        // Returns null if the username does not exist.
        public AppUser GetByUsername(string username)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.User_GetByUsername", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;

                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                        return null;

                    return new AppUser
                    {
                        UserId = r.GetInt32(r.GetOrdinal("UserId")),
                        Username = r.GetString(r.GetOrdinal("Username")),
                        FullName = r.GetString(r.GetOrdinal("FullName")),
                        PasswordHash = (byte[])r["PasswordHash"],
                        PasswordSalt = (byte[])r["PasswordSalt"],
                        IsActive = r.GetBoolean(r.GetOrdinal("IsActive"))
                    };
                }
            }
        }
    }
}
