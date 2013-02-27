using ICSharpCode.SharpZipLib.GZip;
//using Microsoft.Practices.Unity;
using System;
using System.IO;
using System.Web.Mvc;
//using Bonobo.Git.Server.Security;

namespace GitNinja.Gallery.Web.Controllers
{
  public class GitController : Controller
  {
    //[Dependency]
    //public IRepositoryPermissionService RepositoryPermissionService { get; set; }

    //[BasicAuthorize]
    public ActionResult SecureGetInfoRefs(String project, String service)
    {
      return GetInfoRefs(project, service);
      //return RepositoryPermissionService.HasPermission(HttpContext.User.Identity.Name, project)
      //  ? GetInfoRefs(project, service);
      //  : UnauthorizedResult();
    }

    [HttpPost]
    //[BasicAuthorize]
    public ActionResult SecureUploadPack(String project)
    {
      return ExecuteUploadPack(project);
      //return RepositoryPermissionService.HasPermission(HttpContext.User.Identity.Name, project)
      //  ? ExecuteUploadPack(project)
      //  : UnauthorizedResult();
    }

    [HttpPost]
    //[BasicAuthorize]
    public ActionResult SecureReceivePack(String project)
    {
      return ExecuteReceivePack(project);
      //return RepositoryPermissionService.HasPermission(HttpContext.User.Identity.Name, project)
      //  ? ExecuteReceivePack(project)
      //  : UnauthorizedResult();
    }

    public ActionResult AnonymousGetInfoRefs(String project, String service)
    {
      return GetInfoRefs(project, service);
      //return RepositoryPermissionService.AllowsAnonymous(project) && (string.Equals("git-upload-pack", service, StringComparison.InvariantCultureIgnoreCase) || UserConfigurationManager.AllowAnonymousPush)
      //  ? GetInfoRefs(project, service)
      //  : UnauthorizedResult();
    }

    [HttpPost]
    public ActionResult AnonymousReceivePack(String project)
    {
      return ExecuteReceivePack(project);
      //return RepositoryPermissionService.AllowsAnonymous(project) && UserConfigurationManager.AllowAnonymousPush
      //         ? ExecuteReceivePack(project)
      //         : UnauthorizedResult();
    }

    [HttpPost]
    public ActionResult AnonymousUploadPack(String project)
    {
      return ExecuteUploadPack(project);
      //return RepositoryPermissionService.AllowsAnonymous(project)
      //  ? ExecuteUploadPack(project)
      //  : UnauthorizedResult();
    }

    private ActionResult ExecuteReceivePack(string project)
    {
      throw new NotImplementedException();
      //Response.ContentType = "application/x-git-receive-pack-result";
      //SetNoCache();
      //var directory = GetDirectoryInfo(project);
      //if (GitSharp.Repository.IsValid(directory.FullName, true))
      //{
      //  using (var repository = new GitSharp.Repository(directory.FullName))
      //  using (var pack = new ReceivePack(repository))
      //  {
      //    pack.setBiDirectionalPipe(false);
      //    pack.receive(GetInputStream(), Response.OutputStream, Response.OutputStream);
      //  }

      //  return new EmptyResult();
      //}
      //return new HttpNotFoundResult();
    }

    private ActionResult ExecuteUploadPack(string project)
    {
      throw new NotImplementedException();
      //Response.ContentType = "application/x-git-upload-pack-result";
      //SetNoCache();
      //var directory = GetDirectoryInfo(project);
      //if (GitSharp.Repository.IsValid(directory.FullName, true))
      //{
      //  using (var repository = new GitSharp.Repository(directory.FullName))
      //  using (var pack = new UploadPack(repository))
      //  {
      //    pack.setBiDirectionalPipe(false);
      //    pack.Upload(GetInputStream(), Response.OutputStream, Response.OutputStream);
      //  }

      //  return new EmptyResult();
      //}
      //return new HttpNotFoundResult();
    }

    private ActionResult GetInfoRefs(String project, String service)
    {
      throw new NotImplementedException();
      //Response.StatusCode = 200;
      //Response.ContentType = string.Format("application/x-{0}-advertisement", service);
      //SetNoCache();
      //Response.Write(FormatMessage(string.Format("# service={0}\n", service)));
      //Response.Write(FlushMessage());
      //var directory = GetDirectoryInfo(project);
      //if (GitSharp.Repository.IsValid(directory.FullName, true))
      //{
      //  using (var repository = new GitSharp.Repository(directory.FullName))
      //  {
      //    if (string.Equals("git-receive-pack", service, StringComparison.InvariantCultureIgnoreCase))
      //    {
      //      using (var pack = new ReceivePack(repository))
      //      {
      //        pack.SendAdvertisedRefs(new RefAdvertiser.PacketLineOutRefAdvertiser(new PacketLineOut(Response.OutputStream)));
      //      }
      //    }
      //    else if (String.Equals("git-upload-pack", service, StringComparison.InvariantCultureIgnoreCase))
      //    {
      //      using (var pack = new UploadPack(repository))
      //      {
      //        pack.SendAdvertisedRefs(new RefAdvertiser.PacketLineOutRefAdvertiser(new PacketLineOut(Response.OutputStream)));
      //      }
      //    }
      //  }
      //  return new EmptyResult();
      //}
      //return new HttpNotFoundResult();
    }

    private ActionResult UnauthorizedResult()
    {
      return new HttpStatusCodeResult(401);
    }

    private static String FormatMessage(String input)
    {
      return (input.Length + 4).ToString("X").PadLeft(4, '0') + input;
    }

    private static String FlushMessage()
    {
      return "0000";
    }

    private DirectoryInfo GetDirectoryInfo(String project)
    {
      throw new NotImplementedException();
      //return new DirectoryInfo(Path.Combine(UserConfigurationManager.Repositories, project));
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
