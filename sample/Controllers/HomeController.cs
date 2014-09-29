using QueryComposer.MvcHelper.Model;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WithHelper(QueryCompositionModel model)
        {
            return View();
        }

        [HttpPost]
        public ActionResult PostWithHelper(QueryCompositionModel model)
        {
            return RedirectToAction("WithHelper");
        }
    }
}