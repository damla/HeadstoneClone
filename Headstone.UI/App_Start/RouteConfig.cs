using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Headstone.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Ignore special files
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // First add hardcoded routes
            routes.MapMvcAttributeRoutes();

            // Add the default router at the end
            routes.MapRoute(
                    name: "Default",
                    url: "{controller}/{action}/{id}",
                    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}