using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GitNinja.Gallery.Web
{
  public class RouteConfig
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
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
          name: "Default",
          url: "{controller}/{action}/{id}",
          defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}