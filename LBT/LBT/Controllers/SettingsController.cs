using LBT.Filters;
using System.Web.Mvc;

namespace LBT.Controllers
{
    [InitializeSimpleMembership]
    [Authorize(Roles = AdminRoles)]
    public class SettingsController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}