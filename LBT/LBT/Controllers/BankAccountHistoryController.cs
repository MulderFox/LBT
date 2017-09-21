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
    [Authorize]
    public class BankAccountHistoryController : BaseController
    {
        public ActionResult Index(int meetingId, int? page, int pageSize = PageSize)
        {
            Meeting meeting = MeetingCache.GetDetail(Db, meetingId);
            if (!IsAccess(meeting))
            {
                return RedirectToAccessDenied();
            }

            var bankAccountHistories = BankAccountHistoryCache.GetIndex(Db, meeting.BankAccountId, meeting.SecondBankAccountId);

            ViewBag.MeetingId = meetingId;
            ViewBag.MeetingAction = MeetingCache.GetMeetingAction(meeting);

            int pageNumber;
            ProcessPaging(page, out pageNumber);
            
            PopulatePageSize(pageSize);

            BankAccountHistoryIndex[] bankAccountHistoryIndices = BankAccountHistoryIndex.GetModelView(bankAccountHistories);

            return View(bankAccountHistoryIndices.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = AdminRoles)]
        public ActionResult ClaAccessIndex(int? page, int pageSize = PageSize)
        {
            var bankAccountHistories = BankAccountHistoryCache.GetIndex(Db, BankAccountType.ApplicationAccess);

            int pageNumber;
            ProcessPaging(page, out pageNumber);

            PopulatePageSize(pageSize);

            BankAccountHistoryClaAccessIndex[] bankAccountHistoryClaAccessIndices = BankAccountHistoryClaAccessIndex.GetModelView(bankAccountHistories);

            return View(bankAccountHistoryClaAccessIndices.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Edit(int meetingId, int id = 0)
        {
            Meeting meeting = MeetingCache.GetDetail(Db, meetingId);
            BankAccountHistory bankAccountHistory = BankAccountHistoryCache.GetDetail(Db, id);
            if (!IsAccess(meeting, bankAccountHistory))
            {
                return RedirectToAccessDenied();
            }

            bankAccountHistory.AmmountView = Convert.ToInt32(bankAccountHistory.Ammount);

            ViewBag.MeetingId = meetingId;

            BankAccountHistoryEdit bankAccountHistoryEdit = BankAccountHistoryEdit.GetModelView(bankAccountHistory);

            return View(bankAccountHistoryEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int meetingId, BankAccountHistoryEdit bankAccountHistoryEdit)
        {
            BankAccountHistory newBankAccountHistory = BankAccountHistoryCache.GetDetail(Db, bankAccountHistoryEdit.BankAccountHistoryId);
            if (!IsAccess(newBankAccountHistory))
            {
                return RedirectToAccessDenied();
            }

            if (ModelState.IsValid)
            {
                newBankAccountHistory.Ss = bankAccountHistoryEdit.Ss;
                newBankAccountHistory.Vs = bankAccountHistoryEdit.Vs;
                newBankAccountHistory.Ammount = bankAccountHistoryEdit.AmmountView;
                newBankAccountHistory.Note = bankAccountHistoryEdit.Note;

                Db.SaveChanges();

                return RedirectToAction("Index", new { meetingId });
            }

            ViewBag.MeetingId = meetingId;

            BankAccountHistoryEdit newBankAccountHistoryEdit = BankAccountHistoryEdit.GetModelView(newBankAccountHistory);

            return View(newBankAccountHistoryEdit);
        }

        [Authorize(Roles = AdminRoles)]
        public ActionResult ClaAccessEdit(int id = 0)
        {
            BankAccountHistory bankAccountHistory = BankAccountHistoryCache.GetDetail(Db, id);
            if (!IsAccess(bankAccountHistory))
            {
                return RedirectToAccessDenied();
            }

            bankAccountHistory.AmmountView = Convert.ToInt32(bankAccountHistory.Ammount);
            BankAccountHistoryClaAccess bankAccountHistoryClaAccess = BankAccountHistoryClaAccess.GetModelView(bankAccountHistory);

            return View(bankAccountHistoryClaAccess);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles)]
        public ActionResult ClaAccessEdit(BankAccountHistoryClaAccess bankAccountHistoryClaAccess)
        {
            BankAccountHistory newBankAccountHistory = BankAccountHistoryCache.GetDetail(Db, bankAccountHistoryClaAccess.BankAccountHistoryId);
            if (!IsAccess(newBankAccountHistory))
            {
                return RedirectToAccessDenied();
            }

            if (ModelState.IsValid)
            {
                newBankAccountHistory.Ss = bankAccountHistoryClaAccess.Ss;
                newBankAccountHistory.Vs = bankAccountHistoryClaAccess.Vs;
                newBankAccountHistory.Ammount = bankAccountHistoryClaAccess.AmmountView;
                newBankAccountHistory.Note = bankAccountHistoryClaAccess.Note;

                Db.SaveChanges();

                return RedirectToAction("ClaAccessIndex");
            }

            BankAccountHistoryClaAccess newBankAccountHistoryClaAccess = BankAccountHistoryClaAccess.GetModelView(newBankAccountHistory);

            return View(newBankAccountHistoryClaAccess);
        }

        public ActionResult Details(int meetingId, int id = 0)
        {
            Meeting meeting = MeetingCache.GetDetail(Db, meetingId);
            BankAccountHistory bankAccountHistory = BankAccountHistoryCache.GetDetail(Db, id);
            if (!IsAccess(meeting, bankAccountHistory))
            {
                return RedirectToAccessDenied();
            }

            ViewBag.MeetingId = meetingId;

            BankAccountHistoryDetails bankAccountHistoryDetails = BankAccountHistoryDetails.GetModelView(bankAccountHistory);

            return View(bankAccountHistoryDetails);
        }

        [Authorize(Roles = AdminRoles)]
        public ActionResult ClaAccessDetails(int id)
        {
            BankAccountHistory bankAccountHistory = BankAccountHistoryCache.GetDetail(Db, id);
            if (!IsAccess(bankAccountHistory))
            {
                return RedirectToAccessDenied();
            }

            BankAccountHistoryClaAccess bankAccountHistoryClaAccess = BankAccountHistoryClaAccess.GetModelView(bankAccountHistory);

            return View(bankAccountHistoryClaAccess);
        }

        public ActionResult Delete(int meetingId, int id = 0)
        {
            Meeting meeting = MeetingCache.GetDetail(Db, meetingId);
            BankAccountHistory bankAccountHistory = BankAccountHistoryCache.GetDetail(Db, id);
            if (!IsAccess(meeting, bankAccountHistory))
            {
                return RedirectToAccessDenied();
            }

            ViewBag.MeetingId = meetingId;

            BankAccountHistoryDelete bankAccountHistoryDelete = BankAccountHistoryDelete.GetModelView(bankAccountHistory);

            return View(bankAccountHistoryDelete);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int meetingId, int id)
        {
            BankAccountHistory bankAccountHistory = BankAccountHistoryCache.GetDetail(Db, id);
            try
            {
                Db.BankAccountHistories.Remove(bankAccountHistory);
                Db.SaveChanges();

                return RedirectToAction("Index", new { meetingId });
            }
            catch (Exception)
            {
                ModelState.AddModelError(BaseCache.TitleField, ValidationResource.Global_DeleteRecord_ErrorMessage);

                BankAccountHistoryDelete bankAccountHistoryDelete = BankAccountHistoryDelete.GetModelView(bankAccountHistory);

                return View(bankAccountHistoryDelete);
            }
        }

        [Authorize(Roles = AdminRoles)]
        public ActionResult ClaAccessDelete(int id = 0)
        {
            BankAccountHistory bankAccountHistory = BankAccountHistoryCache.GetDetail(Db, id);
            if (!IsAccess(bankAccountHistory))
            {
                return RedirectToAccessDenied();
            }

            BankAccountHistoryClaAccess bankAccountHistoryClaAccess = BankAccountHistoryClaAccess.GetModelView(bankAccountHistory);

            return View(bankAccountHistoryClaAccess);
        }

        [Authorize(Roles = AdminRoles)]
        [HttpPost, ActionName("ClaAccessDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult ClaAccessDeleteConfirmed(int id)
        {
            BankAccountHistory bankAccountHistory = BankAccountHistoryCache.GetDetail(Db, id);
            try
            {
                Db.BankAccountHistories.Remove(bankAccountHistory);
                Db.SaveChanges();

                return RedirectToAction("ClaAccessIndex");
            }
            catch (Exception)
            {
                ModelState.AddModelError(BaseCache.TitleField, ValidationResource.Global_DeleteRecord_ErrorMessage);

                BankAccountHistoryClaAccess bankAccountHistoryClaAccess = BankAccountHistoryClaAccess.GetModelView(bankAccountHistory);

                return View(bankAccountHistoryClaAccess);
            }
        }

        private bool IsAccess(Meeting meeting)
        {
            return meeting != null && (IsAdmin || meeting.OrganizerId == UserId || meeting.SecondaryOrganizerId == UserId) && meeting.Finished > DateTime.Now;
        }

        private bool IsAccess(Meeting meeting, BankAccountHistory bankAccountHistory)
        {
            bool isAccess = IsAccess(meeting);
            isAccess &= bankAccountHistory != null;
            return isAccess;
        }

        private bool IsAccess(BankAccountHistory newBankAccountHistory)
        {
            return newBankAccountHistory != null;
        }
    }
}