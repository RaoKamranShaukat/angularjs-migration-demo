using System;
using System.Web;

namespace EmployeeDemo.Web.Modules
{
    // Files in /app/*.html are view templates that AngularJS loads INTO index.html.
    // Opened directly in the browser they show without CSS, scripts or images (and the
    // login form would not work), so a direct browser visit is redirected to the real app.
    //
    // Why a module and not Global.asax: IIS serves .html files with its native static-file
    // handler, so Application_BeginRequest never sees them. A module registered in Web.config
    // (without preCondition="managedHandler") runs for every request, static files included.
    //
    // After migration: a few lines of ASP.NET Core middleware in Program.cs.
    public class TemplateRedirectModule : IHttpModule
    {
        public void Init(HttpApplication app)
        {
            app.BeginRequest += (sender, e) =>
            {
                var context = ((HttpApplication)sender).Context;
                var path = context.Request.AppRelativeCurrentExecutionFilePath;

                if (path.StartsWith("~/app/", StringComparison.OrdinalIgnoreCase)
                    && path.EndsWith(".html", StringComparison.OrdinalIgnoreCase)
                    && IsBrowserPageNavigation(context.Request))
                {
                    context.Response.Redirect("~/", endResponse: false);
                    context.ApplicationInstance.CompleteRequest();
                }
            };
        }

        // True when the browser is loading a page (address bar, link, F5), false for AngularJS's AJAX calls.
        private static bool IsBrowserPageNavigation(HttpRequest request)
        {
            var fetchDest = request.Headers["Sec-Fetch-Dest"];
            if (!string.IsNullOrEmpty(fetchDest))
                return fetchDest == "document"; // all modern browsers send this header

            var accept = request.Headers["Accept"] ?? "";
            return accept.StartsWith("text/html", StringComparison.OrdinalIgnoreCase); // older browsers
        }

        public void Dispose() { }
    }
}
