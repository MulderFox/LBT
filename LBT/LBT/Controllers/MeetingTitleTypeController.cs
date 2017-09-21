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
    public class MeetingTitleTypeController : BaseController
    {
        public ActionResult Index(string sortOrder, int? page, int pageSize = PageSize)
        {
            int pageNumber;
            ProcessPaging(page, out pageNumber);

            var sortingNames = new[] { BaseCache.TitleField, BaseCache.MeetingTypeField };
            ProcessSorting(sortOrder, sortingNames);

            PopulatePageSize(pageSize);

            MeetingTitleType[] meetingTitleTypes = MeetingTitleTypeCache.GetIndex(Db, sortOrder);

            MeetingTitleTypeIndex[] meetingTitleTypeIndices = MeetingTitleTypeIndex.GetModelView(meetingTitleTypes);

            return View(meetingTitleTypeIndices.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            PopulateMeetingType();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MeetingTitleTypeEdit meetingTitleTypeEdit)
        {
            if (ModelState.IsValid)
            {
                MeetingTitleType meetingTitleType = meetingTitleTypeEdit.GetModel();
                MeetingTitleTypeCache.Insert(Db, meetingTitleType);
                return RedirectToAction("Index");
            }

            PopulateMeetingType(meetingTitleTypeEdit.MeetingType);

            return View(meetingTitleTypeEdit);
        }

        public ActionResult Edit(int id = 0)
        {
            MeetingTitleType meetingTitleType = MeetingTitleTypeCache.GetDetail(Db, id);
            if (!IsAccess(meetingTitleType))
            {
                return RedirectToAccessDenied();
            }

            PopulateMeetingType(meetingTitleType.MeetingType);

            MeetingTitleTypeEdit meetingTitleTypeEdit = MeetingTitleTypeEdit.GetModelView(meetingTitleType);

            return View(meetingTitleTypeEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MeetingTitleTypeEdit meetingTitleTypeEdit)
        {
            if (ModelState.IsValid)
            {
                MeetingTitleType meetingTitleType = meetingTitleTypeEdit.GetModel();
                bool success = MeetingTitleTypeCache.Update(Db, ref meetingTitleType);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("Index");
            }

            PopulateMeetingType(meetingTitleTypeEdit.MeetingType);

            return View(meetingTitleTypeEdit);
        }

        public ActionResult Delete(int id = 0)
        {
            MeetingTitleType meetingTitleType = MeetingTitleTypeCache.GetDetail(Db, id);
            if (!IsAccess(meetingTitleType))
            {
                return RedirectToAccessDenied();
            }

            MeetingTitleTypeDetails meetingTitleTypeDetails = MeetingTitleTypeDetails.GetModelView(meetingTitleType);

            return View(meetingTitleTypeDetails);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MeetingTitleType meetingTitleType;
            DeleteResult deleteResult = MeetingTitleTypeCache.Delete(Db, id, out meetingTitleType);

            MeetingTitleTypeDetails meetingTitleTypeDetails;
            switch (deleteResult)
            {
                case DeleteResult.Ok:
                    return RedirectToAction("Index");

                case DeleteResult.AuthorizationFailed:
                    return RedirectToAccessDenied();

                case DeleteResult.DbFailed:
                    ModelState.AddModelError(BaseCache.TitleField, ValidationResource.Global_DeleteRecord_ErrorMessage);

                    meetingTitleTypeDetails = MeetingTitleTypeDetails.GetModelView(meetingTitleType);

                    return View(meetingTitleTypeDetails);

                case DeleteResult.UnlinkFailed:
                    ModelState.AddModelError(BaseCache.TitleField, ValidationResource.Meeting_DeleteDBLinks_ErrorMessage);
                    
                    meetingTitleTypeDetails = MeetingTitleTypeDetails.GetModelView(meetingTitleType);

                    return View(meetingTitleTypeDetails);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsAccess(MeetingTitleType meetingTitleType)
        {
            return meetingTitleType != null;
        }
    }
}