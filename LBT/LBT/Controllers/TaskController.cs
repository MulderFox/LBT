using LBT.Filters;
using LBT.ModelViews;
using System.Web.Mvc;

namespace LBT.Controllers
{
    [InitializeSimpleMembership]
    [Authorize]
    public class TaskController : BaseController
    {
        //
        // GET: /Task/

        public ActionResult Index()
        {
            TaskIndex viewModel = TaskIndex.GetViewModel(this);
            return View(viewModel);
        }
    }
}
