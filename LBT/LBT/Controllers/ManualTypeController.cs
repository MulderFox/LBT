using LBT.Cache;
using LBT.Filters;
using LBT.Models;
using LBT.ModelViews;
using LBT.Resources;
using System;
using System.Web.Mvc;

namespace LBT.Controllers
{
    [InitializeSimpleMembership]
    [Authorize(Roles = AdminRoles)]
    public class ManualTypeController : BaseController
    {
        public ActionResult Index()
        {
            ManualTypeIndex[] manualTypes = ManualTypeIndex.GetIndex(Db);
            return View(manualTypes);
        }

        public ActionResult Create()
        {
            var manualTypeCreate = new ManualTypeCreate();
            return View(manualTypeCreate);
        }

        [HttpPost]
        public ActionResult Create(ManualTypeCreate manualTypeCreate)
        {
            ModelState.Merge(manualTypeCreate.Validate(Db));

            if (ModelState.IsValid)
            {
                ManualType manualType = manualTypeCreate.GetModel();
                ManualTypeCache.Insert(Db, manualType);
                return RedirectToAction("Index");
            }

            return View(manualTypeCreate);
        }

        public ActionResult Edit(int id = 0)
        {
            ManualType manualType = ManualTypeCache.GetDetail(Db, id);
            if (!IsAccess(manualType))
            {
                return RedirectToAccessDenied();
            }

            ManualTypeEdit manualTypeEdit = ManualTypeEdit.GetModelView(manualType);

            return View(manualTypeEdit);
        }

        [HttpPost]
        public ActionResult Edit(ManualTypeEdit manualTypeEdit)
        {
            ModelState.Merge(manualTypeEdit.Validate(Db));

            if (ModelState.IsValid)
            {
                ManualType manualType = manualTypeEdit.GetModel();
                bool success = ManualTypeCache.Update(Db, manualType);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("Index");
            }

            return View(manualTypeEdit);
        }

        public ActionResult Delete(int id = 0)
        {
            ManualType manualType = ManualTypeCache.GetDetail(Db, id);
            if (!IsAccess(manualType))
            {
                return RedirectToAccessDenied();
            }

            ManualTypeDelete manualTypeDelete = ManualTypeDelete.GetModelView(manualType);

            return View(manualTypeDelete);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ManualType manualType = ManualTypeCache.GetDetail(Db, id);
            if (!IsAccess(manualType))
            {
                return RedirectToAccessDenied();
            }

            ManualTypeDelete manualTypeDelete = ManualTypeDelete.GetModelView(manualType);
            ModelState.Merge(manualTypeDelete.Validate(Db));

            if (ModelState.IsValid)
            {
                DeleteResult deleteResult = ManualTypeCache.Delete(Db, manualType);

                switch (deleteResult)
                {
                    case DeleteResult.Ok:
                        return RedirectToAction("Index");

                    case DeleteResult.AuthorizationFailed:
                        return RedirectToAccessDenied();

                    case DeleteResult.DbFailed:
                        ModelState.AddModelError(BaseCache.TitleField, ValidationResource.Global_DeleteRecord_ErrorMessage);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }

            return View(manualTypeDelete);
        }

        private bool IsAccess(ManualType manualType)
        {
            return manualType != null;
        }
    }
}