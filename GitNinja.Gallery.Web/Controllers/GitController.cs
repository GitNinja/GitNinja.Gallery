using System.IO;
using System.Web.Mvc;
using GitSharp.Core.Transport;
using ICSharpCode.SharpZipLib.GZip;
//using Microsoft.Practices.Unity;
//using Bonobo.Git.Server.Security;

namespace GitNinja.Gallery.Web.Controllers
{
  public class GitController : Controller
  {
    //[Dependency]
    //public IRepositoryPermissionService RepositoryPermissionService { get; set; }

    //[BasicAuthorize]
    public ActionResult SecureGetInfoRefs(string dojo, string repo, string service)
    {
      return GetInfoRefs(dojo, repo, service);
      //return RepositoryPermissionService.HasPermission(HttpContext.User.Identity.Name, project)
      //  ? GetInfoRefs(project, service);
      //  : new HttpStatusCodeResult(401);
    }

    [HttpPost]
    //[BasicAuthorize]
    public ActionResult SecureUploadPack(string dojo, string repo)
    {
      return ExecuteUploadPack(dojo, repo);
      //return RepositoryPermissionService.HasPermission(HttpContext.User.Identity.Name, project)
      //  ? ExecuteUploadPack(project)
      //  : new HttpStatusCodeResult(401);
    }

    [HttpPost]
    //[BasicAuthorize]
    public ActionResult SecureReceivePack(string dojo, string repo)
    {
      return ExecuteReceivePack(dojo, repo);
      //return RepositoryPermissionService.HasPermission(HttpContext.User.Identity.Name, project)
      //  ? ExecuteReceivePack(project)
      //  : new HttpStatusCodeResult(401);
    }

    public ActionResult AnonymousGetInfoRefs(string dojo, string repo, string service)
    {
      return GetInfoRefs(dojo, repo, service);
      //return RepositoryPermissionService.AllowsAnonymous(project) && (string.Equals("git-upload-pack", service, StringComparison.InvariantCultureIgnoreCase) || UserConfigurationManager.AllowAnonymousPush)
      //  ? GetInfoRefs(project, service)
      //  : new HttpStatusCodeResult(401);
    }

    [HttpPost]
    public ActionResult AnonymousReceivePack(string dojo, string repo)
    {
      return ExecuteReceivePack(dojo, repo);
      //return RepositoryPermissionService.AllowsAnonymous(project) && UserConfigurationManager.AllowAnonymousPush
      //         ? ExecuteReceivePack(project)
      //         : new HttpStatusCodeResult(401);
    }

    [HttpPost]
    public ActionResult AnonymousUploadPack(string dojo, string repo)
    {
      return ExecuteUploadPack(dojo, repo);
      //return RepositoryPermissionService.AllowsAnonymous(project)
      //  ? ExecuteUploadPack(project)
      //  : new HttpStatusCodeResult(401);
    }

    private ActionResult ExecuteReceivePack(string dojo, string repo)
    {
      Response.ContentType = "application/x-git-receive-pack-result";
      SetNoCache();
      if (GitNinja.IsValidRepository(dojo, repo))
      {
        using (var receivePack = new ReceivePack(new GitSharp.Repository(GitNinja.GetRepositoryPath(dojo, repo))))
        {
          receivePack.setBiDirectionalPipe(false);
          receivePack.receive(GetInputStream(), Response.OutputStream, Response.OutputStream);
          //receivePack.receive(GetInputStream(), Response.OutputStream);
        }
        return new EmptyResult();
      }
      return new HttpNotFoundResult();
    }

    private ActionResult ExecuteUploadPack(string dojo, string repo)
    {
      Response.ContentType = "application/x-git-upload-pack-result";
      SetNoCache();
      if (GitNinja.IsValidRepository(dojo, repo))
      {
        using (var uploadPack = new UploadPack(new GitSharp.Repository(GitNinja.GetRepositoryPath(dojo, repo))))
        {
          uploadPack.setBiDirectionalPipe(false);
          uploadPack.Upload(GetInputStream(), Response.OutputStream, Response.OutputStream);
        }
        return new EmptyResult();
      }
      return new HttpNotFoundResult();
    }

    private ActionResult GetInfoRefs(string dojo, string repo, string service)
    {
      Response.StatusCode = 200;
      Response.ContentType = string.Format("application/x-{0}-advertisement", service);
      SetNoCache();
      Response.Write(FormatMessage(string.Format("# service={0}\n", service)));
      Response.Write(FlushMessage());
      if (GitNinja.IsValidRepository(dojo, repo))
      {
        using (var repository = new GitSharp.Repository(GitNinja.GetRepositoryPath(dojo, repo)))
        {
          switch (service.ToLower())
          {
            case "git-receive-pack":
              using (var pack = new ReceivePack(repository))
                pack.SendAdvertisedRefs(new RefAdvertiser.PacketLineOutRefAdvertiser(new PacketLineOut(Response.OutputStream)));
              return new EmptyResult();
            case "git-upload-pack":
              using (var pack = new UploadPack(repository))
                pack.SendAdvertisedRefs(new RefAdvertiser.PacketLineOutRefAdvertiser(new PacketLineOut(Response.OutputStream)));
                //pack.sendAdvertisedRefs(new RefAdvertiser.PacketLineOutRefAdvertiser(new PacketLineOut(Response.OutputStream)));
              return new EmptyResult();
            default:
              return new HttpNotFoundResult();
          }
        }
      }
      return new HttpNotFoundResult();
    }

    private static string FormatMessage(string input)
    {
      return (input.Length + 4).ToString("X").PadLeft(4, '0') + input;
    }

    private static string FlushMessage()
    {
      return "0000";
    }

    private Stream GetInputStream()
    {
      if (Request.Headers["Content-Encoding"] == "gzip")
      {
        return new GZipInputStream(Request.InputStream);
      }
      return Request.InputStream;
    }

    private void SetNoCache()
    {
      Response.AddHeader("Expires", "Fri, 01 Jan 1980 00:00:00 GMT");
      Response.AddHeader("Pragma", "no-cache");
      Response.AddHeader("Cache-Control", "no-cache, max-age=0, must-revalidate");
    }
  }
}
