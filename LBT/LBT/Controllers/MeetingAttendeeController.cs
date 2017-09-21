using LBT.Filters;
using LBT.ModelViews;
using LBT.Models;
using System;
using System.Data;
using System.Web.Mvc;

namespace LBT.Controllers
{
    [InitializeSimpleMembership]
    public class MeetingAttendeeController : BaseController
    {
        public ActionResult Edit(string returnController, string returnAction, int actionId, int id = 0)
        {
            MeetingAttendee meetingAttendee = Db.MeetingAttendees.Find(id);

            if (!IsAccess(meetingAttendee, returnController, returnAction))
            {
                return RedirectToAccessDenied();
            }

            ViewBag.ReturnController = returnController;
            ViewBag.ReturnAction = returnAction;
            ViewBag.ActionId = actionId;

            MeetingAttendeeEdit meetingAttendeeEdit = MeetingAttendeeEdit.GetModelView(meetingAttendee);

            return View(meetingAttendeeEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MeetingAttendee meetingAttendee, string returnController, string returnAction, int actionId)
        {
            MeetingAttendee newMeetingAttendee = Db.MeetingAttendees.Find(meetingAttendee.MeetingAttendeeId);

            if (!IsAccess(newMeetingAttendee, returnController, returnAction))
            {
                return RedirectToAccessDenied();
            }

            if (ModelState.IsValid)
            {
                newMeetingAttendee.Note = meetingAttendee.Note;

                Db.Entry(newMeetingAttendee).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction(returnAction, returnController, new { id = newMeetingAttendee.MeetingId });
            }

            ViewBag.ReturnController = returnController;
            ViewBag.ReturnAction = returnAction;
            ViewBag.ActionId = actionId;

            MeetingAttendeeEdit meetingAttendeeEdit = MeetingAttendeeEdit.GetModelView(meetingAttendee);

            return View(meetingAttendeeEdit);
        }

        public ActionResult Details(string returnController, string returnAction, int actionId, int id = 0)
        {
            MeetingAttendee meetingAttendee = Db.MeetingAttendees.Find(id);

            if (!IsAccess(meetingAttendee, returnController, returnAction))
            {
                return RedirectToAccessDenied();
            }

            ViewBag.ReturnController = returnController;
            ViewBag.ReturnAction = returnAction;
            ViewBag.ActionId = actionId;

            MeetingAttendeeDetails meetingAttendeeDetails = MeetingAttendeeDetails.GetModelView(meetingAttendee);

            return View(meetingAttendeeDetails);
        }

        private bool IsAccess(MeetingAttendee meetingAttendee, string returnController, string returnAction)
        {
            if (meetingAttendee == null)
            {
                return false;
            }

            if (!IsAdmin && meetingAttendee.Meeting.OrganizerId != UserId && meetingAttendee.Meeting.SecondaryOrganizerId != UserId)
            {
                return false;
            }

            return !String.IsNullOrEmpty(returnController) && !String.IsNullOrEmpty(returnAction);
        }
    }
}