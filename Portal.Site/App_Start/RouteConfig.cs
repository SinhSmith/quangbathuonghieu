using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Portal.Site
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(name: "ForgotPassword", url: "quen-mat-khau", defaults: new { controller = "Account", action = "ForgotPassword" });
            routes.MapRoute(name: "ChangePassword", url: "quan-ly-tai-khoan", defaults: new { controller = "Manage", action = "ChangePassword" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
