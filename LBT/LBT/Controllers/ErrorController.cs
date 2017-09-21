using LBT.Filters;
using System.Web.Mvc;

namespace LBT.Controllers
{
    public class ErrorController : Controller
    {
        [PreventDirectAccess]
        public ActionResult ServerError()
        {
            return View("Error", "_ErrorLayout");
        }

        [PreventDirectAccess]
        public ActionResult AccessDenied()
        {
            return View("Error403", "_ErrorLayout");
        }

        public ActionResult NotFound()
        {
            return View("Error404", "_ErrorLayout");
        }

        [PreventDirectAccess]
        public ActionResult OtherHttpStatusCode(int httpStatusCode)
        {
            return View("GenericHttpError", "_ErrorLayout", httpStatusCode);
        }
    }
}
