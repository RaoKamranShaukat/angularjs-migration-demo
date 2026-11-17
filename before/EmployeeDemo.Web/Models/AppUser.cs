namespace EmployeeDemo.Web.Models
{
    // A row from dbo.Users. Never returned to the browser (it contains the password hash).
    public class AppUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsActive { get; set; }
    }

    // What the login form posts.
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
