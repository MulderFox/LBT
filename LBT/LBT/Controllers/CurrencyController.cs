using LBT.Cache;
using LBT.Filters;
using LBT.Models;
using PagedList;
using LBT.Resources;
using System;
using System.Web.Mvc;

namespace LBT.Controllers
{
    [InitializeSimpleMembership]
    [Authorize(Roles = AdminRoles)]
    public class CurrencyController : BaseController
    {
        public ActionResult Index(int? page, int pageSize = PageSize)
        {
            int pageNumber;
            ProcessPaging(page, out pageNumber);

            PopulatePageSize(pageSize);

            Currency[] currencies = CurrencyCache.GetIndex(Db);
            return View(currencies.ToPagedList(pageNumber, PageSize));
        }

        public ActionResult Create()
        {
            PopulateCurrencyTypeExceptCZK();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Currency currency)
        {
            ModelState.Merge(currency.Validate(Db));

            if (ModelState.IsValid)
            {
                CurrencyCache.Insert(Db, currency);
                return RedirectToAction("Index");
            }

            PopulateCurrencyTypeExceptCZK(currency.CurrencyType);

            return View(currency);
        }

        public ActionResult Edit(int id = 0)
        {
            Currency currency = CurrencyCache.GetDetail(Db, id);
            if (!IsAccess(currency))
            {
                return RedirectToAccessDenied();
            }

            return View(currency);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Currency currency)
        {
            ModelState.Merge(currency.Validate(Db));

            if (ModelState.IsValid)
            {
                bool success = CurrencyCache.Update(Db, ref currency);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("Index");
            }

            return View(currency);
        }

        public ActionResult Delete(int id = 0)
        {
            Currency currency = CurrencyCache.GetDetail(Db, id);
            if (!IsAccess(currency))
            {
                return RedirectToAccessDenied();
            }

            return View(currency);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Currency currency;
            DeleteResult deleteResult = CurrencyCache.Delete(Db, id, out currency);

            switch (deleteResult)
            {
                case DeleteResult.Ok:
                    return RedirectToAction("Index");

                case DeleteResult.AuthorizationFailed:
                    return RedirectToAccessDenied();

                case DeleteResult.DbFailed:
                    ModelState.AddModelError(BaseCache.CurrencyTypeField, ValidationResource.Global_DeleteRecord_ErrorMessage);
                    return View(currency);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsAccess(Currency currency)
        {
            return currency != null;
        }
    }
}