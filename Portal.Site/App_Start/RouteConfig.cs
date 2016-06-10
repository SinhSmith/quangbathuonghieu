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

            routes.MapRoute(name: "Company", url: "quan-ly-doanh-nghiep", defaults: new { controller = "Company", action = "Index" });
            routes.MapRoute(name: "Product", url: "quan-ly-san-pham", defaults: new { controller = "Product", action = "Index" });

            //routes.MapRoute(name: "User", url: "quan-ly-nguoi-dung", defaults: new { controller = "User", action = "Index" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
