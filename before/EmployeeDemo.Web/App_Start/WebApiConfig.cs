using System.Web.Http;

namespace EmployeeDemo.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Use the [Route] attributes on the controllers.
            config.MapHttpAttributeRoutes();

            // Always return JSON (by default Web API 2 returns XML to browsers).
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Every API call needs a logged-in user, unless the action says [AllowAnonymous] (login/logout).
            // Secure by default: a new controller can't be left open by forgetting [Authorize].
            config.Filters.Add(new AuthorizeAttribute());

            // Note: Web API 2 keeps C# property names as they are (PascalCase),
            // so the JSON looks like { "EmployeeId": 1, "FirstName": "Ayesha" }.
        }
    }
}
