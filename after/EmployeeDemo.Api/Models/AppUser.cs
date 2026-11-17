namespace EmployeeDemo.Api.Models;

// A row from dbo.Users. Never returned to the browser (it contains the password hash).
public record AppUser(
    int UserId,
    string Username,
    string FullName,
    byte[] PasswordHash,
    byte[] PasswordSalt,
    bool IsActive);

// What the login form posts.
public record LoginRequest(string? Username, string? Password);

// What the API returns about the signed-in user.
public record UserInfo(string Username, string FullName);
