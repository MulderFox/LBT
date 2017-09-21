using LBT.Cache;
using LBT.Filters;
using LBT.ModelViews;
using PagedList;
using System;
using System.Web.Mvc;

namespace LBT.Controllers
{
    [InitializeSimpleMembership]
    [Authorize]
    public class ManualController : BaseController
    {
        [Authorize(Roles = AdminRoles)]
        public ActionResult Index(int? page, int pageSize = PageSize)
        {
            IPagedList<ManualIndex> viewModel = ManualIndex.GetViewModel(this, page, pageSize);
            return View(viewModel);
        }

        [Authorize(Roles = AdminRoles)]
        public ActionResult Create()
        {
            ManualCreate viewModel = ManualCreate.GetViewModel(this);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles)]
        public ActionResult Create(ManualCreate manualCreate)
        {
            ManualCreate viewModel = ManualCreate.GetViewModel(this, manualCreate);
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        [Authorize(Roles = AdminRoles)]
        public ActionResult Edit(int id = 0)
        {
            ManualEdit viewModel = ManualEdit.GetViewModel(this, id);
            if (viewModel == null)
            {
                return RedirectToAccessDenied();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles)]
        public ActionResult Edit(ManualEdit manualEdit)
        {
            ManualEdit viewModel = ManualEdit.GetViewModel(this, manualEdit);
            if (viewModel == null)
            {
                return RedirectToAccessDenied();
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        [Authorize(Roles = AdminRoles)]
        public ActionResult Details(int id= 0)
        {
            ManualDetail viewModel = ManualDetail.GetViewModel(this, id);
            if (viewModel == null)
            {
                return RedirectToAccessDenied();
            }

            return View(viewModel);
        }

        [Authorize(Roles = AdminRoles)]
        public ActionResult Delete(int id = 0)
        {
            ManualDelete viewModel = ManualDelete.GetViewModel(this, id);
            if (viewModel == null)
            {
                return RedirectToAccessDenied();
            }

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles)]
        public ActionResult DeleteConfirmed(int id)
        {
            DeleteResult deleteResult;
            ManualDelete viewModel = ManualDelete.GetViewModel(this, id, out deleteResult);
            if (viewModel == null)
            {
                return RedirectToAccessDenied();
            }
            
            if (ModelState.IsValid)
            {
                switch (deleteResult)
                {
                    case DeleteResult.Ok:
                        return RedirectToAction("Index");

                    case DeleteResult.AuthorizationFailed:
                        return RedirectToAccessDenied();

                    case DeleteResult.DbFailed:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return View(viewModel);
        }

        [Authorize(Roles = AdminRoles)]
        public ActionResult Player(int id = 0)
        {
            ManualPlayer manualPlayer = ManualPlayer.GetViewModel(this, id);
            if (manualPlayer == null || !manualPlayer.IsValid)
                return RedirectToAccessDenied();

            if (manualPlayer.NeedPlayer)
                return View(manualPlayer.PlayerViewName, manualPlayer);

            manualPlayer.SetContentToView();
            return File(manualPlayer.AbsoluteFilePath, manualPlayer.MimeContentType);
        }

        [AllowAnonymous]
        public ActionResult PlayerData(int id = 0)
        {
            ManualPlayer manualPlayer = ManualPlayer.GetViewModel(this, id);
            if (manualPlayer == null)
                return RedirectToAccessDenied();

            return !manualPlayer.IsValid ? manualPlayer.PlayerDataDefaultActionResult : manualPlayer.PlayerDataActionResult;
        }

        public ActionResult ManualsByType(int? page, int id = 0, int pageSize = PageSize)
        {
            ManualsByTypeIndex viewModel = ManualsByTypeIndex.GetViewModel(this, id, page, pageSize);
            return View(viewModel);
        }

        public ActionResult PlayerToken(string token)
        {
            ManualPlayer manualPlayer = ManualPlayer.GetViewModel(this, token);
            if (manualPlayer == null || !manualPlayer.IsValid)
                return RedirectToAccessDenied();

            if (manualPlayer.NeedPlayer)
                return View(manualPlayer.PlayerViewName, manualPlayer);

            // TODO: zmikeska: Špatně se zobrazují názvy souborů na IE.

            manualPlayer.SetContentToView();
            return File(manualPlayer.AbsoluteFilePath, manualPlayer.MimeContentType);
        }

        public ActionResult DownloadToken(string token)
        {
            ManualPlayer manualPlayer = ManualPlayer.GetViewModel(this, token);
            if (manualPlayer == null || !manualPlayer.IsValid)
                return RedirectToAccessDenied();

            // TODO: zmikeska: Špatně se zobrazují názvy souborů na IE.

            manualPlayer.SetContentToDownload();
            return File(manualPlayer.AbsoluteFilePath, manualPlayer.MimeContentType);
        }
    }
}