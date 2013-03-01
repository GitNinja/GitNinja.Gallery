using System.Web.Mvc;
using GitNinja.Gallery.Web.Models;

namespace GitNinja.Gallery.Web.Controllers
{
    public class DojoController : Controller
    {
      public ActionResult Index()
      {
        //list all dojos
        return View();
      }

      public ActionResult Dojo(string dojo)
      {
        if (!GitNinja.Instance.DojoExists(dojo))
          return new HttpNotFoundResult();
        return View(new Dojo(dojo));
      }

      public ActionResult Repo(string dojo, string repo)
      {
        if (!GitNinja.Instance.RepoExists(dojo, repo))
          return new HttpNotFoundResult();
        return View(new Repo(dojo, repo));
      }

      public ActionResult Create(string dojo, string repo)
      {
        GitNinja.Instance.CreateDojo(dojo);
        if (!string.IsNullOrWhiteSpace(repo))
        {
          GitNinja.Instance.CreateRepo(dojo, repo);
          return RedirectToAction("Repo", new { dojo, repo });
        }
        return RedirectToAction("Dojo", new { dojo });
      }
    }
}
