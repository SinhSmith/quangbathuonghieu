using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Portal.CMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            // Account
            routes.MapRoute(name: "Login", url: "dang-nhap", defaults: new { controller = "Account", action = "Login" });
            routes.MapRoute(name: "Register", url: "dang-ky", defaults: new { controller = "Account", action = "Register" });
            routes.MapRoute(name: "ForgotPassword", url: "quen-mat-khau", defaults: new { controller = "Account", action = "ForgotPassword" });
            routes.MapRoute(name: "ChangePassword", url: "quan-ly-tai-khoan", defaults: new { controller = "Manage", action = "ChangePassword" });

            routes.MapRoute(name: "ResumeDetails", url: "ho-so-ung-vien/{email}", defaults: new { controller = "Resumes", action = "Details", email = UrlParameter.Optional });

            routes.MapRoute(name: "Job_Create", url: "dang-tuyen-dung", defaults: new { controller = "Job", action = "Create" });
            routes.MapRoute(name: "Job_Edit", url: "cap-nhat_{title}_{id}", defaults: new { controller = "Job", action = "Edit" });
            routes.MapRoute(name: "Job_List", url: "danh-sach-tuyen-dung", defaults: new { controller = "Job", action = "List" });
            routes.MapRoute(name: "CompanyDetails", url: "ho-so-doanh-nghiep", defaults: new { controller = "Company", action = "Details" });

            // Details Job
            routes.MapRoute(name: "Details", url: "{title}_{id}", defaults: new { controller = "Job", action = "Details" });

            // Admin
            routes.MapRoute(name: "Manage_Company_List", url: "quan-ly-nha-tuyen-dung", defaults: new { controller = "Company", action = "List" });
            routes.MapRoute(name: "Manage_Company_Create", url: "tao-moi-nha-tuyen-dung", defaults: new { controller = "Company", action = "Create" });
            routes.MapRoute(name: "Manage_Company_Edit", url: "cap-nhat-nha-tuyen-dung/{id}", defaults: new { controller = "Company", action = "Edit" });

            // Home
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Job", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
