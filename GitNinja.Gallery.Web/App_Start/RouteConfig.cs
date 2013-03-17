using System.Web.Mvc;
using System.Web.Routing;

namespace GitNinja.Gallery.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            /* Browse ****************************************************************************/
            routes.MapRoute(
              name: "GotoDojo",
              url: "Browse/{dojo}",
              defaults: new { controller = "Dojo", action = "Dojo" }
            );
            routes.MapRoute(
              name: "GotoRepo",
              url: "Browse/{dojo}/{repo}",
              defaults: new { controller = "Dojo", action = "Repo" }
            );

            routes.MapRoute(
                name: "FileBrowse",
                url: "FileBrowser/{dojo}/{repo}/{branch}/{id}",
                defaults: new { controller = "FileBrowser", action = "Lookup", branch = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            routes.MapRoute(
              name: "Default",
              url: "{controller}/{action}/{id}",
              defaults: new { controller = "Dojo", action = "Index", id = UrlParameter.Optional }
            );

            /* Git ******************************************************************************/
            routes.MapRoute(
              name: "GitInfoRefs",
              url: "git/{dojo}/{repo}/info/refs",
              defaults: new { controller = "Git", action = "GetInfoRefs" },
              constraints: new { method = new HttpMethodConstraint("GET") });
            routes.MapRoute(
              name: "GitUploadPack",
              url: "git/{dojo}/{repo}/git-upload-pack",
              defaults: new { controller = "Git", action = "UploadPack" },
              constraints: new { method = new HttpMethodConstraint("POST") });
            routes.MapRoute(
              name: "GitReceivePack",
              url: "git/{dojo}/{repo}/git-receive-pack",
              defaults: new { controller = "Git", action = "ReceivePack" },
              constraints: new { method = new HttpMethodConstraint("POST") });
        }
    }
}