using System.Web;
using System.Web.Optimization;

namespace Portal.Site
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/bootstrap-datepicker.min.js",
                      "~/Scripts/bootstrap-datepicker.vi.min.js",
                      "~/Scripts/respond.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-datepicker3.min.css",
                      "~/Content/select2.css",
                      "~/Content/theme.css",
                      "~/Content/custom.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/main").Include(
                      "~/Scripts/main.js",
                      "~/Scripts/select2/select2.js",
                      "~/Scripts/select2/i18n/vi.js"));

            bundles.Add(new StyleBundle("~/Content/Error").Include(
                      "~/Content/error404.css"));

            //Minimize css and js files
            //BundleTable.EnableOptimizations = true;
        }
    }
}
