using QueryComposer.MvcHelper.Model;
using QueryComposer.MvcHelper;
using System.Web.Mvc;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SampleApp.Models;
using System;
using System.Web;

namespace SampleApp.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            using (var context = new Data.SampleDatabaseEntities())
            {
                var model = new IndexModel();

                model.Areas = await context.Areas.ToListAsync();
                model.Iterations = await context.Iterations.ToListAsync();
                model.Statuses = await context.Status.ToListAsync();

                return View(model);
            }
        }

        [HttpPost]
        [CustomHandleError]
        public async Task<ActionResult> Index(QueryCompositionModel model)
        {
            try
            {
                using (var context = new Data.SampleDatabaseEntities())
                {
                    var query = context.Tasks
                        .Include(t => t.Area)
                        .Include(t => t.Iteration)
                        .Include(t => t.Status)
                        .AsQueryable();
                    query = query.FilterByQueries(model.Queries);

                    var tasks = await query.ToListAsync();
                    return PartialView("_GridResult", tasks);
                }
            }
            catch (Exception e)
            {
                throw new HttpException(400, e.Message);
            }
        }

        public async Task<ActionResult> WithHelper()
        {
            using (var context = new Data.SampleDatabaseEntities())
            {
                var model = new IndexModel();

                model.Areas = await context.Areas.ToListAsync();
                model.Iterations = await context.Iterations.ToListAsync();
                model.Statuses = await context.Status.ToListAsync();

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult PostWithHelper(QueryCompositionModel model)
        {
            return RedirectToAction("WithHelper");
        }
    }
}