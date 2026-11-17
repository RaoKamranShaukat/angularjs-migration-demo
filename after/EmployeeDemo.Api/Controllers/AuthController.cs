using System.Security.Claims;
using EmployeeDemo.Api.Data;
using EmployeeDemo.Api.Models;
using EmployeeDemo.Api.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDemo.Api.Controllers;

// POST api/auth/login   { "username": "...", "password": "..." }  -> sets the auth cookie
// POST api/auth/logout                                             -> removes the auth cookie
// GET  api/auth/me                                                 -> who is logged in (401 if nobody)
[ApiController]
[Route("api/auth")]
public class AuthController(UserRepository users) : ControllerBase
{
    [HttpPost("login"), AllowAnonymous]
    public async Task<ActionResult<UserInfo>> Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrEmpty(request.Password))
            return BadRequest(new { message = "Username and password are required." });

        var user = await users.GetByUsernameAsync(request.Username.Trim());

        // Same message for "no such user" and "wrong password", so attackers can't discover usernames.
        if (user is null || !user.IsActive || !PasswordHasher.Verify(request.Password, user.PasswordSalt, user.PasswordHash))
            return Unauthorized(new { message = "Invalid username or password." });

        // BEFORE: FormsAuthentication.SetAuthCookie(user.Username, false)
        // AFTER:  build a ClaimsPrincipal and let the cookie middleware write the encrypted cookie.
        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, user.Username)],
            CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return new UserInfo(user.Username, user.FullName);
    }

    [HttpPost("logout"), AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        // BEFORE: FormsAuthentication.SignOut()
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserInfo>> Me()
    {
        var user = await users.GetByUsernameAsync(User.Identity!.Name!);
        if (user is null || !user.IsActive)
            return Unauthorized();

        return new UserInfo(user.Username, user.FullName);
    }
}
