using LBT.Filters;
using LBT.ModelViews;
using LBT.Resources;
using System.Web.Mvc;

namespace LBT.Controllers
{
    [InitializeSimpleMembership]
    [Authorize(Roles = AdminRoles)]
    public class BulkMailController : BaseController
    {
        public ActionResult Create()
        {
            var bulkMailCreate = new BulkMailCreate();

            return View(bulkMailCreate);
        }

        [HttpPost]
        public ActionResult Create(BulkMailCreate bulkMailCreate)
        {
            ModelState.Merge(bulkMailCreate.Validate());
            if (!ModelState.IsValid)
            {
                return View(bulkMailCreate);
            }

            ModelState.Merge(bulkMailCreate.SendEmails(Db));
            if (ModelState.IsValid)
            {
                TempData[StatusMessageTempKey] = ViewResource.BulkMail_EmailsWereSent_Text;
                return RedirectToAction("Create");
            }

            return View(bulkMailCreate);
        }
    }
}