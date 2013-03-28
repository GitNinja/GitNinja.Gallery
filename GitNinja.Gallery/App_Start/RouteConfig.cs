using System.Web.Mvc;
using System.Web.Routing;

namespace GitNinja.Gallery.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            /* Browse ****************************************************************************/
            routes.MapRoute(
              name: "GotoDojo",
              url: "browse/{dojo}",
              defaults: new { controller = "Dojo", action = "Dojo" }
            );
            routes.MapRoute(
              name: "GotoRepo",
              url: "browse/{dojo}/{repo}",
              defaults: new { controller = "Repository", action = "Tree" }
            );

            routes.MapRoute(
                name: "tree",
                url: "browse/{dojo}/{repo}/tree/{reference}/{*path}",
                defaults: new { controller = "Repository", action = "Tree", reference = UrlParameter.Optional, path = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "blob",
                url: "browse/{dojo}/{repo}/blob/{reference}/{*path}",
                defaults: new { controller = "Repository", action = "Blob", reference = UrlParameter.Optional, path = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "commits",
                url: "browse/{dojo}/{repo}/commits",
                defaults: new { controller = "Repository", action = "Commits" }
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