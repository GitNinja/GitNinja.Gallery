using System.Dynamic;
using System.Web.Mvc;
using GitNinja.Gallery.Models;

namespace GitNinja.Gallery.Controllers
{
    public class DojoController : Controller
    {
        public ActionResult Index()
        {
            //list all dojos
            return View(App_Start.GitNinja.Instance.GetDojoList());
        }

        public ActionResult Dojo(string dojo)
        {
            if (!App_Start.GitNinja.Instance.DojoExists(dojo))
                return new HttpNotFoundResult();
            return View(new Dojo(dojo));
        }


        [HttpPost]
        //TODO: "Exists-check" is transactionally not safe, revisit
        public ActionResult Create(string dojo)
        {
            if (string.IsNullOrWhiteSpace(dojo)
                || App_Start.GitNinja.Instance.DojoExists(dojo))
            {
                dynamic failureModel = new ExpandoObject();
                failureModel.Dojo = dojo;
                return View("CreationFailed", failureModel);
            }

            
            App_Start.GitNinja.Instance.CreateDojo(dojo);
            
            return RedirectToAction("Dojo", new {dojo});
        }
    }
}
