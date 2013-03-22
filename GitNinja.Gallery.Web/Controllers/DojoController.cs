using System;
using System.Dynamic;
using System.IO;
using System.Web.Mvc;
using GitNinja.Gallery.Web.Models;

namespace GitNinja.Gallery.Web.Controllers
{
    public class DojoController : Controller
    {
        public ActionResult Index()
        {
            //list all dojos
            return View(GitNinja.Instance.GetDojoList());
        }

        public ActionResult Dojo(string dojo)
        {
            if (!GitNinja.Instance.DojoExists(dojo))
                return new HttpNotFoundResult();
            return View(new Dojo(dojo));
        }


        [HttpPost]
        //TODO: "Exists-check" is transactionally not safe, revisit
        public ActionResult Create(string dojo)
        {
            if (string.IsNullOrWhiteSpace(dojo)
                || GitNinja.Instance.DojoExists(dojo))
            {
                dynamic failureModel = new ExpandoObject();
                failureModel.Dojo = dojo;
                return View("CreationFailed", failureModel);
            }

            
            GitNinja.Instance.CreateDojo(dojo);
            
            return RedirectToAction("Dojo", new {dojo});
        }
    }
}
