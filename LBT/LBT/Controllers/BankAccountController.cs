using LBT.Cache;
using LBT.Filters;
using LBT.Models;
using LBT.ModelViews;
using PagedList;
using LBT.Resources;
using System;
using System.Web.Mvc;

namespace LBT.Controllers
{
    [InitializeSimpleMembership]
    [Authorize(Roles = AdminRoles)]
    public class BankAccountController : BaseController
    {
        public ActionResult Index(string sortOrder, int? page, int pageSize = PageSize)
        {
            int pageNumber;
            ProcessPaging(page, out pageNumber);

            var sortingNames = new[] { BaseCache.TitleField, BaseCache.BankAccountTypeField, BaseCache.CurrencyTypeField, BaseCache.ValidToField };
            ProcessSorting(sortOrder, sortingNames);

            PopulatePageSize(pageSize);

            BankAccountIndex[] bankAccounts = BankAccountIndex.GetIndexRows(Db, sortOrder);
            return View(bankAccounts.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            PopulateBankAccountType();
            PopulateCurrencyType();
            PopulateUserIds();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BankAccountEdit bankAccountEdit)
        {
            ModelState.Merge(bankAccountEdit.Validate(Db));

            if (ModelState.IsValid)
            {
                int[] userIds;
                bool saveOnlyUsers;
                BankAccount bankAccount = bankAccountEdit.GetModel(out userIds, out saveOnlyUsers);
                BankAccountCache.Insert(Db, bankAccount, userIds);
                return RedirectToAction("Index");
            }

            PopulateBankAccountType(bankAccountEdit.BankAccountType);
            PopulateCurrencyType(bankAccountEdit.CurrencyType);
            PopulateUserIds(bankAccountEdit.UserIds);

            return View(bankAccountEdit);
        }

        public ActionResult Edit(int id = 0)
        {
            BankAccount bankAccount = BankAccountCache.GetDetail(Db, id);
            if (!IsAccess(bankAccount))
            {
                return RedirectToAccessDenied();
            }

            BankAccountEdit bankAccountEdit = BankAccountEdit.GetModelView(bankAccount);

            PopulateBankAccountType(bankAccountEdit.BankAccountType);
            PopulateCurrencyType(bankAccountEdit.CurrencyType);
            PopulateUserIds(bankAccountEdit.UserIds);

            return View(bankAccountEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BankAccountEdit bankAccountEdit)
        {
            ModelState.Merge(bankAccountEdit.Validate(Db));

            if (ModelState.IsValid)
            {
                int[] userIds;
                bool saveOnlyUsersOrToken;
                BankAccount bankAccount = bankAccountEdit.GetModel(out userIds, out saveOnlyUsersOrToken);
                bool success = BankAccountCache.Update(Db, userIds, saveOnlyUsersOrToken, ref bankAccount);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                if (saveOnlyUsersOrToken)
                {
                    TempData[StatusMessageTempKey] = ViewResource.BankAccount_SaveOnlyUsers_Text;
                }

                return RedirectToAction("Index");
            }

            PopulateBankAccountType(bankAccountEdit.BankAccountType);
            PopulateCurrencyType(bankAccountEdit.CurrencyType);
            PopulateUserIds(bankAccountEdit.UserIds);

            return View(bankAccountEdit);
        }

        public ActionResult Details(int id = 0)
        {
            BankAccount bankAccount = BankAccountCache.GetDetail(Db, id);
            if (!IsAccess(bankAccount))
            {
                return RedirectToAccessDenied();
            }

            BankAccountDetails bankAccountDetails = BankAccountDetails.GetModelView(bankAccount);

            return View(bankAccountDetails);
        }

        public ActionResult Delete(int id = 0)
        {
            BankAccount bankAccount = BankAccountCache.GetDetail(Db, id);
            if (!IsAccess(bankAccount))
            {
                return RedirectToAccessDenied();
            }

            BankAccountDetails bankAccountDetails = BankAccountDetails.GetModelView(bankAccount);

            return View(bankAccountDetails);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccount bankAccount;
            DeleteResult deleteResult = BankAccountCache.Delete(Db, id, out bankAccount);

            BankAccountDetails bankAccountDetails;
            switch (deleteResult)
            {
                case DeleteResult.Ok:
                    return RedirectToAction("Index");

                case DeleteResult.AuthorizationFailed:
                    return RedirectToAccessDenied();

                case DeleteResult.DbFailed:
                    ModelState.AddModelError(BaseCache.TitleField, ValidationResource.Global_DeleteRecord_ErrorMessage);

                    bankAccountDetails = BankAccountDetails.GetModelView(bankAccount);

                    return View(bankAccountDetails);

                case DeleteResult.UnlinkFailed:
                    ModelState.AddModelError(BaseCache.TitleField, ValidationResource.BankAccount_DeleteDBLinks_ErrorMessage);

                    bankAccountDetails = BankAccountDetails.GetModelView(bankAccount);

                    return View(bankAccountDetails);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsAccess(BankAccount bankAccount)
        {
            return bankAccount != null;
        }
    }
}