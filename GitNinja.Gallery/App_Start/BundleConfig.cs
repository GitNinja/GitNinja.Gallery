using System.Web.Optimization;

namespace GitNinja.Gallery.App_Start
{
  public class BundleConfig
  {
    // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
    public static void RegisterBundles(BundleCollection bundles)
    {
      bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                  "~/Scripts/jquery-{version}.js").Include("~/Scripts/jquery.pjax.js"));

      bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                  "~/Scripts/jquery.unobtrusive*",
                  "~/Scripts/jquery.validate*"));

      // Use the development version of Modernizr to develop with and learn from. Then, when you're
      // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
      bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                  "~/Scripts/modernizr-*"));

      bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                  "~/Scripts/bootstrap.js"));

      bundles.Add(new ScriptBundle("~/bundles/gitninja").Include(
                  "~/Scripts/gitninja.js"));

      bundles.Add(new StyleBundle("~/Content/gn").Include("~/Content/css/gitninja.css"));
      bundles.Add(new StyleBundle("~/Content/bs").Include(
          "~/Content/css/bootstrap.css",
          "~/Content/css/bootstrap-responsive.css"));
    }
  }
}