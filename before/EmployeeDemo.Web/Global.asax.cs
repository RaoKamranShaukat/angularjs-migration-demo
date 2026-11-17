using System;
using System.Web;
using System.Web.Http;

namespace EmployeeDemo.Web
{
    // Application entry point in classic ASP.NET.
    // (In ASP.NET Core this becomes Program.cs.)
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_BeginRequest()
        {
            // Forms Authentication normally turns every 401 into a 302 redirect to the login URL.
            // That breaks AJAX: AngularJS would get the login page HTML instead of a 401.
            // For /api/* we switch that off so the client receives a real 401.
            if (Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api/", StringComparison.OrdinalIgnoreCase))
            {
                Response.SuppressFormsAuthenticationRedirect = true;
            }
        }
    }
}
