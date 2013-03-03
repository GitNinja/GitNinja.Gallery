using System.Web.Mvc;
using System.Web.Routing;

// ReSharper disable CheckNamespace
namespace GitNinja.Gallery.Web
// ReSharper restore CheckNamespace
{
  public class RouteConfig
  {
    public const string GitRepositoryPrefix = "git/";
    public const string AnonymousGitRepositoryPrefix = "git/anon/";

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      /* git routes ********************************************************/

      routes.MapRoute(
        name: "SecureInfoRefs",
        url: GitRepositoryPrefix + "{dojo}/{repo}/info/refs",
        defaults: new { controller = "Git", action = "SecureGetInfoRefs" },
        constraints: new { method = new HttpMethodConstraint("GET") });

      routes.MapRoute(
        name: "SecureUploadPack",
        url: GitRepositoryPrefix + "{dojo}/{repo}/git-upload-pack",
        defaults: new { controller = "Git", action = "SecureUploadPack" },
        constraints: new { method = new HttpMethodConstraint("POST") });

      routes.MapRoute(
        name: "SecureReceivePack",
        url: GitRepositoryPrefix + "{dojo}/{repo}/git-receive-pack",
        defaults: new { controller = "Git", action = "SecureReceivePack" },
        constraints: new { method = new HttpMethodConstraint("POST") });

      routes.MapRoute(
        name: "AnonymousInfoRefs",
        url: AnonymousGitRepositoryPrefix + "{dojo}/{repo}/info/refs",
        defaults: new { controller = "Git", action = "AnonymousGetInfoRefs" },
        constraints: new { method = new HttpMethodConstraint("GET") });

      routes.MapRoute(
        name: "AnonymousReceivePack",
        url: AnonymousGitRepositoryPrefix + "{dojo}/{repo}/git-receive-pack",
        defaults: new { controller = "Git", action = "AnonymousReceivePack" },
        constraints: new { method = new HttpMethodConstraint("POST") });

      routes.MapRoute(
        name: "AnonymousUploadPack",
        url: AnonymousGitRepositoryPrefix + "{dojo}/{repo}/git-upload-pack",
        defaults: new { controller = "Git", action = "AnonymousUploadPack" },
        constraints: new { method = new HttpMethodConstraint("POST") });

      /*
      routes.MapRoute(
       name: "IndexRoute",
       url: "{controller}/Index/",
        defaults: new { action = "Index" });

      routes.MapRoute(
        name: "CreateRoute",
        url: "{controller}/Create/",
        defaults: new { action = "Create" });

      routes.MapRoute(
        name: "RepositoryTree",
        url: "Repository/{id}/Tree/{name}/{*path}",
        defaults: new { controller = "Repository", action = "Tree" });

      routes.MapRoute(
        name: "RepositoryCommits",
        url: "Repository/{id}/Commits/{name}/",
        defaults: new { controller = "Repository", action = "Commits" });

      routes.MapRoute(
        name: "RepositoryCommit",
        url: "Repository/{id}/Commit/{commit}/",
        defaults: new { controller = "Repository", action = "Commit" });

      routes.MapRoute(
        name: "Repository",
        url: "Repository/{id}/{action}/",
        defaults: new { controller = "Repository", action = "Detail" });

      routes.MapRoute(
        name: "Account",
        url: "Account/{id}/{action}/",
        defaults: new { controller = "Account", action = "Detail" });

      routes.MapRoute(
        name: "Team",
        url: "Team/{id}/{action}/",
        defaults: new { controller = "Team", action = "Detail" });
      */

      /* end git routes ****************************************************/

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
        defaults: new { controller = "Dojo", action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}