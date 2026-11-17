using System.Net;
using System.Web.Http;
using System.Web.Security;
using EmployeeDemo.Web.Data;
using EmployeeDemo.Web.Models;
using EmployeeDemo.Web.Security;

namespace EmployeeDemo.Web.Controllers
{
    // POST api/auth/login   { "Username": "...", "Password": "..." }  -> sets the auth cookie
    // POST api/auth/logout                                             -> removes the auth cookie
    // GET  api/auth/me                                                 -> who is logged in (401 if nobody)
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly UserRepository _users = new UserRepository();

        [HttpPost, Route("login"), AllowAnonymous]
        public IHttpActionResult Login(LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Username and password are required.");

            var user = _users.GetByUsername(request.Username.Trim());

            // Same message for "no such user" and "wrong password", so attackers can't discover usernames.
            if (user == null || !user.IsActive || !PasswordHasher.Verify(request.Password, user.PasswordSalt, user.PasswordHash))
                return Content(HttpStatusCode.Unauthorized, new { Message = "Invalid username or password." });

            // Classic ASP.NET Forms Authentication: an encrypted, HttpOnly cookie holds the username.
            FormsAuthentication.SetAuthCookie(user.Username, createPersistentCookie: false);

            return Ok(new { user.Username, user.FullName });
        }

        [HttpPost, Route("logout"), AllowAnonymous]
        public IHttpActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet, Route("me")]
        public IHttpActionResult Me()
        {
            var user = _users.GetByUsername(User.Identity.Name);
            if (user == null || !user.IsActive)
                return Unauthorized();

            return Ok(new { user.Username, user.FullName });
        }
    }
}
