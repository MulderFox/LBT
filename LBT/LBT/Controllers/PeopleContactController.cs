// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 12-20-2013
//
// Last Modified By : zmikeska
// Last Modified On : 01-13-2014
// ***********************************************************************
// <copyright file="PeopleContactController.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Google.Apis.Calendar.v3.Data;
using LBT.Cache;
using LBT.Filters;
using LBT.Helpers;
using LBT.Models;
using LBT.ModelViews;
using LBT.Services.LyconetService;
using PagedList;
using LBT.Resources;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Calendar = LBT.Services.GoogleApis.Calendar;

namespace LBT.Controllers
{
    /// <summary>
    /// Class PeopleContactController
    /// </summary>
    [InitializeSimpleMembership]
    [Authorize]
    public class PeopleContactController : BaseController
    {
        /// <summary>
        /// Enum CommandArgument
        /// </summary>
        public enum CommandArgument
        {
            /// <summary>
            /// The show formatting
            /// </summary>
            ShowFormatting,
            /// <summary>
            /// The hide formatting
            /// </summary>
            HideFormatting,
            /// <summary>
            /// The show death contacts
            /// </summary>
            ShowDeathContacts,
            /// <summary>
            /// The hide death contacts
            /// </summary>
            HideDeathContacts
        }

        //
        // GET: /PeopleContact/

        /// <summary>
        /// Indexes the specified sort order.
        /// </summary>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="currentFilter">The current filter.</param>
        /// <param name="currentFilterAccording">The current filter according.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="searchStringAccording">The search string according.</param>
        /// <param name="page">The page.</param>
        /// <param name="commandArgument">The command argument.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="filteredUserId">The filtered user id.</param>
        /// <returns>ActionResult.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public ActionResult Index(string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page, string commandArgument, int filteredUserId = 0, int pageSize = PageSize)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            int pageNumber;
            ProcessSearchFilterAndPaging(currentFilter, currentFilterAccording, ref searchString, ref searchStringAccording, ref page, out pageNumber);

            CommandArgument argument;
            bool showFormating = Cookie.GetCookie(Request, Cookie.ShowFormattingCookieKey) == Cookie.CookieTrue;
            bool hideDeathContacts = Cookie.GetCookie(Request, Cookie.HideDeathContactsCookieKey) == Cookie.CookieTrue;
            if (!String.IsNullOrEmpty(commandArgument) && Enum.TryParse(commandArgument, out argument))
            {
                switch (argument)
                {
                    case CommandArgument.ShowFormatting:
                        showFormating = true;
                        Cookie.SetNewCookie(Cookie.ShowFormattingCookieKey, Cookie.CookieTrue, Response);
                        break;

                    case CommandArgument.HideFormatting:
                        showFormating = false;
                        Cookie.SetNewCookie(Cookie.ShowFormattingCookieKey, Cookie.CookieFalse, Response);
                        break;

                    case CommandArgument.ShowDeathContacts:
                        hideDeathContacts = false;
                        Cookie.SetNewCookie(Cookie.HideDeathContactsCookieKey, Cookie.CookieFalse, Response);
                        break;

                    case CommandArgument.HideDeathContacts:
                        hideDeathContacts = true;
                        Cookie.SetNewCookie(Cookie.HideDeathContactsCookieKey, Cookie.CookieTrue, Response);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            IQueryable<PeopleContact> peopleContacts = PeopleContactCache.GetIndex(Db, searchString, searchStringAccording, sortOrder, filteredUserId, hideDeathContacts);

            ViewBag.ShowFormatting = showFormating;
            ViewBag.HideDeathContacts = hideDeathContacts;
            ViewBag.IsRegistrar = filteredUserId == UserId;
            ViewBag.ShowImportExport = filteredUserId == UserId;

            var sortingNames = new[]
                                   {
                                       BaseCache.LastNameField, BaseCache.FirstNameField, BaseCache.CityField,
                                       BaseCache.PhoneNumber1Field, BaseCache.Email1Field, BaseCache.SkypeField,
                                       BaseCache.PotentialField
                                   };
            ProcessSorting(sortOrder, sortingNames);

            PopulateSearchStringAccording(searchString, searchStringAccording);
            PopulateFilteredUserId(filteredUserId);
            PopulatePageSize(pageSize);

            PeopleContactIndex[] peopleContactIndices = PeopleContactIndex.GetModelView(peopleContacts.ToArray());

            return View(peopleContactIndices.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /PeopleContact/Details/5

        /// <summary>
        /// Detailses the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Details(int id = 0)
        {
            PeopleContact peoplecontact = PeopleContactCache.GetDetail(Db, id);
            if (!IsAccessDetails(peoplecontact))
            {
                return RedirectToAccessDenied();
            }

            ViewBag.IsRegistrar = peoplecontact.RegistrarId == UserId;
            
            PeopleContactDetails peopleContactDetails = PeopleContactDetails.GetModelView(peoplecontact);

            return View(peopleContactDetails);
        }

        /// <summary>
        /// Prints the details.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult PrintDetails(int id)
        {
            PeopleContact peoplecontact = PeopleContactCache.GetDetail(Db, id);
            if (peoplecontact == null || FilteredUser.GetFilteredUsers(Db, UserId).All(gfu => gfu.UserId != peoplecontact.RegistrarId))
            {
                return RedirectToAccessDenied();
            }

            ViewBag.IsRegistrar = peoplecontact.RegistrarId == UserId;

            PeopleContactDetails peopleContactDetails = PeopleContactDetails.GetModelView(peoplecontact);

            return View("PrintDetails", "_PrintLayout", peopleContactDetails);
        }

        /// <summary>
        /// Prints the index.
        /// </summary>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="currentFilter">The current filter.</param>
        /// <param name="currentFilterAccording">The current filter according.</param>
        /// <param name="filteredUserId">The filtered user id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult PrintIndex(string sortOrder, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            IQueryable<PeopleContact> peopleContacts = PeopleContactCache.GetIndex(Db, currentFilter, currentFilterAccording, sortOrder, filteredUserId, false);
            if (peopleContacts.Count() > 1000)
            {
                peopleContacts = peopleContacts.Take(1000);
                ViewBag.IsListCuttedMessage = ViewResource.Global_IsListCutted_Text;
            }

            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.CurrentFilterAccording = currentFilterAccording;
            ViewBag.FilterUserId = filteredUserId;

            PeopleContactIndex[] peopleContactIndices = PeopleContactIndex.GetModelView(peopleContacts.ToArray());

            return View("PrintIndex", "_PrintLayout", peopleContactIndices);
        }

        //
        // GET: /PeopleContact/Create

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Create()
        {
            PopulateDistrictId();
            PopulatePhoneNumberPrefix1Id();
            PopulatePhoneNumberPrefix2Id();

            return View();
        }

        //
        // POST: /PeopleContact/Create

        /// <summary>
        /// Creates the specified peopleContactEdit.
        /// </summary>
        /// <param name="peopleContactEdit">The peopleContactEdit.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PeopleContactEdit peopleContactEdit)
        {
            ModelState.Merge(peopleContactEdit.Validate(Db, UserId));

            if (ModelState.IsValid)
            {
                PeopleContact peopleContact = peopleContactEdit.GetModel();
                PeopleContactCache.Insert(Db, UserId, ref peopleContact);

                return RedirectToAction("Index");
            }

            PopulateDistrictId(peopleContactEdit.DistrictId);
            PopulatePhoneNumberPrefix1Id(peopleContactEdit.PhoneNumberPrefix1Id);
            PopulatePhoneNumberPrefix2Id(peopleContactEdit.PhoneNumberPrefix2Id);

            return View(peopleContactEdit);
        }

        //
        // GET: /PeopleContact/Edit/5

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Edit(int id = 0)
        {
            PeopleContact peopleContact = PeopleContactCache.GetDetail(Db, id);
            if (peopleContact == null || peopleContact.RegistrarId != UserId)
            {
                return RedirectToAccessDenied();
            }

            PopulateRegistrarId(peopleContact.RegistrarId);
            PopulateDistrictId(peopleContact.DistrictId);
            PopulatePhoneNumberPrefix1Id(peopleContact.PhoneNumberPrefix1Id);
            PopulatePhoneNumberPrefix2Id(peopleContact.PhoneNumberPrefix2Id);

            PeopleContactEdit peopleContactEdit = PeopleContactEdit.GetModelView(peopleContact);

            return View(peopleContactEdit);
        }

        //
        // POST: /PeopleContact/Edit/5

        /// <summary>
        /// Edits the specified peopleContactEdit.
        /// </summary>
        /// <param name="peopleContactEdit">The peopleContactEdit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AsyncTimeout(60000)]
        public async Task<ActionResult> Edit(PeopleContactEdit peopleContactEdit, CancellationToken cancellationToken)
        {
            if (!IsAccessEdit(peopleContactEdit))
                return RedirectToAccessDenied();

            ModelState.Merge(peopleContactEdit.Validate(Db, UserId));

            if (ModelState.IsValid)
            {
                PeopleContact peopleContact = peopleContactEdit.GetModel();
                Calendar calendar = await UserProfileCache.GetCalendar(Db, peopleContact.RegistrarId, this, cancellationToken);

                PeopleContact previousPeopleContact = PeopleContactCache.GetDetail(Db, peopleContact.PeopleContactId);
                CheckWorkflow(peopleContact, previousPeopleContact, calendar);

                bool success = PeopleContactCache.Update(Db, ref peopleContact);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("Index");
            }

            PopulateRegistrarId(peopleContactEdit.RegistrarId);
            PopulateDistrictId(peopleContactEdit.DistrictId);
            PopulatePhoneNumberPrefix1Id(peopleContactEdit.PhoneNumberPrefix1Id);
            PopulatePhoneNumberPrefix2Id(peopleContactEdit.PhoneNumberPrefix2Id);

            return View(peopleContactEdit);
        }

        //
        // GET: /PeopleContact/Delete/5

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Delete(int id = 0)
        {
            PeopleContact peoplecontact = PeopleContactCache.GetDetail(Db, id);
            if (peoplecontact == null || peoplecontact.RegistrarId != UserId)
            {
                return RedirectToAccessDenied();
            }

            PeopleContactDelete peopleContactDelete = PeopleContactDelete.GetModelView(peoplecontact);

            return View(peopleContactDelete);
        }

        //
        // POST: /PeopleContact/Delete/5

        /// <summary>
        /// Deletes the confirmed.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PeopleContact peopleContact;
            DeleteResult deleteResult = PeopleContactCache.Delete(Db, id, out peopleContact);
            switch (deleteResult)
            {
                case DeleteResult.Ok:
                    return RedirectToAction("Index");

                case DeleteResult.AuthorizationFailed:
                    return RedirectToAccessDenied();

                case DeleteResult.DbFailed:
                    ModelState.AddModelError(BaseCache.CityField, ValidationResource.Global_DeleteRecord_ErrorMessage);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            PeopleContactDelete peopleContactDelete = PeopleContactDelete.GetModelView(peopleContact);

            return View(peopleContactDelete);
        }

        /// <summary>
        /// Edits the task.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult EditTask(int id, string fieldName)
        {
            PeopleContact peopleContact = Db.PeopleContacts.Include(pc => pc.Registrar).Single(pc => pc.PeopleContactId == id);
            PeopleContactTask peopleContactTask = peopleContact.GetPeopleContactTasks(fieldName);

            if (peopleContactTask == null)
                return View("EditTaskEmpty", "_PopupLayout");

            if (peopleContactTask.RegistrarId != UserId)
                return RedirectToAccessDenied();

            return View("EditTask", "_PopupLayout", peopleContactTask);
        }

        /// <summary>
        /// Edits the task.
        /// </summary>
        /// <param name="peopleContactTask">The people contact task.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>ActionResult.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        [HttpPost]
        [AsyncTimeout(60000)]
        public async Task<ActionResult> EditTask(PeopleContactTask peopleContactTask, CancellationToken cancellationToken)
        {
            if (peopleContactTask.RegistrarId != UserId)
                return RedirectToAccessDenied();

            PeopleContact peopleContact = PeopleContactCache.GetDetailWithRegister(Db, peopleContactTask.PeopleContactId);
            if (peopleContact.RegistrarId != UserId)
                return RedirectToAccessDenied();

            var previousPeopleContact = new PeopleContact();
            previousPeopleContact.CopyFrom(peopleContact);

            PeopleContactTask tempPeopleContactTask = peopleContact.GetPeopleContactTasks(peopleContactTask.FieldName);
            peopleContactTask.Text = tempPeopleContactTask.Text;

            if (peopleContactTask.FieldValue == null)
            {
                ModelState.AddModelError(BaseCache.FieldValueField, String.Format(ValidationResource.Global_Required_ErrorMessage, FieldResource.Global_Date_Name));
            }
            else
            {
                switch (peopleContactTask.FieldName)
                {
                    case BaseCache.TrackingEmailSentField:
                        peopleContact.TrackingEmailSent = true;
                        if (peopleContact.Presented != null && (peopleContactTask.FieldValue < peopleContact.Presented.Value || peopleContactTask.FieldValue > DateTime.Today))
                        {
                            ModelState.AddModelError(BaseCache.FieldValueField, ValidationResource.PeopleContact_TrackingEmailSentAndPresentedComparing_ErrorMessage);
                        }
                        break;

                    case BaseCache.SecondContactedField:
                        peopleContact.SecondContacted = peopleContactTask.FieldValue;
                        if (peopleContact.Presented != null && (peopleContactTask.FieldValue < peopleContact.Presented.Value || peopleContactTask.FieldValue > DateTime.Today))
                        {
                            ModelState.AddModelError(BaseCache.FieldValueField, ValidationResource.PeopleContact_SecondContactedAndPresentedComparing_ErrorMessage);
                        }
                        break;

                    case BaseCache.SecondTrackingEmailSentField:
                        peopleContact.SecondTrackingEmailSent = true;
                        if (peopleContact.SecondContacted != null && (peopleContactTask.FieldValue < peopleContact.SecondContacted.Value || peopleContactTask.FieldValue > DateTime.Today))
                        {
                            ModelState.AddModelError(BaseCache.FieldValueField, ValidationResource.PeopleContact_SecondTrackingEmailSentAndSecondContactedComparing_ErrorMessage);
                        }
                        break;

                    case BaseCache.BusinessInfoParticipatedField:
                        peopleContact.BusinessInfoParticipated = peopleContactTask.FieldValue;
                        if (peopleContact.Presented != null && (peopleContactTask.FieldValue < peopleContact.Presented.Value || peopleContactTask.FieldValue > DateTime.Today))
                        {
                            ModelState.AddModelError(BaseCache.FieldValueField, ValidationResource.PeopleContact_BusinessInfoParticipatedAndPresentedComparing_ErrorMessage);
                        }
                        break;

                    case BaseCache.SecondMeetingField:
                        peopleContact.SecondMeeting = peopleContactTask.FieldValue;
                        if (peopleContact.SecondContacted != null && (peopleContactTask.FieldValue < peopleContact.SecondContacted.Value || peopleContactTask.FieldValue > DateTime.Today))
                        {
                            ModelState.AddModelError(BaseCache.FieldValueField, ValidationResource.PeopleContact_SecondMeetingAndSecondContactedComparing_ErrorMessage);
                        }
                        break;

                    case BaseCache.ThirdMeetingField:
                        peopleContact.ThirdMeeting = peopleContactTask.FieldValue;
                        if (peopleContact.BusinessInfoParticipated != null && (peopleContactTask.FieldValue < peopleContact.BusinessInfoParticipated.Value || peopleContactTask.FieldValue > DateTime.Today))
                        {
                            ModelState.AddModelError(BaseCache.FieldValueField, ValidationResource.PeopleContact_ThirdMeetingAndBusinessInfoParticipatedComparing_ErrorMessage);
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (ModelState.IsValid)
            {
                var calendar = new Calendar
                {
                    GoogleCredentialsJson = peopleContact.Registrar.GoogleCredentialsJson,
                    GoogleCalendarId = peopleContact.Registrar.GoogleCalendarId,
                    UseGoogleCalendarByUser = peopleContact.Registrar.UseGoogleCalendar,
                    UseMail = peopleContact.Registrar.UseMail,
                    EmailTo = peopleContact.Registrar.Email1,
                    ReminderTime = peopleContact.Registrar.ReminderTime,
                    IsEventsPrivate = peopleContact.Registrar.IsEventsPrivate
                };
                await calendar.AuthorizeAsync(this, cancellationToken);
                CheckWorkflow(peopleContact, previousPeopleContact, calendar);

                Db.Entry(peopleContact).State = EntityState.Modified;
                Db.SaveChanges();

                return View("EditTaskSummary", "_PopupLayout", peopleContactTask);
            }

            return View("EditTask", "_PopupLayout", peopleContactTask);
        }

        /// <summary>
        /// Edits the first contacted.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult EditFirstContacted(int id)
        {
            PeopleContact peopleContact = PeopleContactCache.GetDetail(Db, id);
            if (peopleContact == null || peopleContact.RegistrarId != UserId)
            {
                return RedirectToAccessDenied();
            }

            CreateEventsBeforePresented(peopleContact);
            Db.Entry(peopleContact).State = EntityState.Modified;
            Db.SaveChanges();

            return RedirectToAction("Edit", new { id });
        }

        /// <summary>
        /// Edits the presented.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult EditPresented(int id)
        {
            PeopleContact peopleContact = PeopleContactCache.GetDetail(Db, id);
            if (peopleContact == null || peopleContact.RegistrarId != UserId)
            {
                return RedirectToAccessDenied();
            }

            CreateEventsInPresented(peopleContact);
            Db.Entry(peopleContact).State = EntityState.Modified;
            Db.SaveChanges();

            return RedirectToAction("Edit", new { id });
        }

        /// <summary>
        /// Edits the tracking email sent.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult EditTrackingEmailSent(int id)
        {
            PeopleContact peopleContact = PeopleContactCache.GetDetail(Db, id);
            if (peopleContact == null || peopleContact.RegistrarId != UserId)
            {
                return RedirectToAccessDenied();
            }

            CreateEventsAfterBusinessInfoParticipated(peopleContact);

            Db.Entry(peopleContact).State = EntityState.Modified;
            Db.SaveChanges();

            return RedirectToAction("Edit", new { id });
        }

        /// <summary>
        /// Edits the second contacted.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult EditSecondContacted(int id)
        {
            PeopleContact peopleContact = PeopleContactCache.GetDetail(Db, id);
            if (peopleContact == null || peopleContact.RegistrarId != UserId)
            {
                return RedirectToAccessDenied();
            }

            CreateEventsAfterPresented(peopleContact);
            Db.Entry(peopleContact).State = EntityState.Modified;
            Db.SaveChanges();

            return RedirectToAction("Edit", new { id });
        }

        /// <summary>
        /// Edits the second meeting.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult EditSecondMeeting(int id)
        {
            PeopleContact peopleContact = PeopleContactCache.GetDetail(Db, id);
            if (peopleContact == null || peopleContact.RegistrarId != UserId)
            {
                return RedirectToAccessDenied();
            }

            CreateEventsAfterBusinessInfoParticipated(peopleContact);

            Db.Entry(peopleContact).State = EntityState.Modified;
            Db.SaveChanges();

            return RedirectToAction("Edit", new { id });
        }

        /// <summary>
        /// Edits the second tracking email sent.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult EditSecondTrackingEmailSent(int id)
        {
            PeopleContact peopleContact = PeopleContactCache.GetDetail(Db, id);
            if (peopleContact == null || peopleContact.RegistrarId != UserId)
            {
                return RedirectToAccessDenied();
            }

            CreateEventsAfterBusinessInfoParticipated(peopleContact);

            Db.Entry(peopleContact).State = EntityState.Modified;
            Db.SaveChanges();

            return RedirectToAction("Edit", new { id });
        }

        /// <summary>
        /// Edits the business info participated.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult EditBusinessInfoParticipated(int id)
        {
            PeopleContact peopleContact = PeopleContactCache.GetDetail(Db, id);
            if (peopleContact == null || peopleContact.RegistrarId != UserId)
            {
                return RedirectToAccessDenied();
            }

            CreateEventsInResetBusinessInfo(peopleContact);

            Db.Entry(peopleContact).State = EntityState.Modified;
            Db.SaveChanges();

            return RedirectToAction("Edit", new { id });
        }

        /// <summary>
        /// Edits the third meeting.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult EditThirdMeeting(int id)
        {
            PeopleContact peopleContact = PeopleContactCache.GetDetail(Db, id);
            if (peopleContact == null || peopleContact.RegistrarId != UserId)
            {
                return RedirectToAccessDenied();
            }

            CreateEventsAfterBusinessInfoParticipated(peopleContact);

            Db.Entry(peopleContact).State = EntityState.Modified;
            Db.SaveChanges();

            return RedirectToAction("Edit", new { id });
        }

        /// <summary>
        /// Multiples the create.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult MultipleCreate()
        {
            PopulatePhoneNumberPrefix1Id();

            return View();
        }

        /// <summary>
        /// Multiples the create.
        /// </summary>
        /// <param name="multiplePeopleContacts">The multiple people contacts.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MultipleCreate(MultiplePeopleContacts multiplePeopleContacts)
        {
            ModelState.Merge(multiplePeopleContacts.Validate());

            if (ModelState.IsValid)
            {
                PeopleContactCache.Insert(Db, UserId, ref multiplePeopleContacts);

                return RedirectToAction("Index");
            }

            PopulatePhoneNumberPrefix1Id();

            return View(multiplePeopleContacts);
        }

        /// <summary>
        /// Determines whether [is phone number1 or skype or email1 unique by user] [the specified people contact id].
        /// </summary>
        /// <param name="peopleContactId">The people contact id.</param>
        /// <param name="phoneNumberPrefix1Id">The phone number prefix1 id.</param>
        /// <param name="phoneNumber1">The phone number1.</param>
        /// <param name="skype">The skype.</param>
        /// <param name="email1">The email1.</param>
        public JsonResult IsPhoneNumber1OrSkypeOrEmail1UniqueByUser(int? peopleContactId, int? phoneNumberPrefix1Id,
                                                                    string phoneNumber1, string skype, string email1)
        {

            object data;
            PeopleContactCache.ValidateIsPhoneNumber1OrSkypeOrEmail1UniqueByUser(Db, UserId, peopleContactId,
                                                                                 phoneNumberPrefix1Id, phoneNumber1,
                                                                                 skype, email1, out data);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Checks the workflow.
        /// </summary>
        /// <param name="peopleContact">The peopleContactEdit.</param>
        /// <param name="previousPeopleContact">The previous people contact.</param>
        /// <param name="calendar">The calendar.</param>
        /// <returns>Task.</returns>
        private void CheckWorkflow(PeopleContact peopleContact, PeopleContact previousPeopleContact, Calendar calendar)
        {
            if (calendar.UseGoogleCalendar && !calendar.Authorized)
            {
                TempData[StatusMessageTempKey] = ValidationResource.Global_CannotConnectToGoogleCalendar_ErrorMessage;
            }

            if (!previousPeopleContact.Presented.HasValue && peopleContact.Presented.HasValue)
            {
                CreateEventsAfterPresentedAsync(peopleContact, calendar);
                return;
            }

            if (!previousPeopleContact.SecondContacted.HasValue && peopleContact.SecondContacted.HasValue)
            {
                CreateEventsAfterSecondContactedAsync(peopleContact, calendar);
            }

            if (!previousPeopleContact.BusinessInfoParticipated.HasValue && peopleContact.BusinessInfoParticipated.HasValue)
            {
                CreateEventsAfterBusinessInfoParticipatedAsync(peopleContact, calendar);
            }

            if (peopleContact.SecondMeeting.HasValue && peopleContact.ThirdMeeting.HasValue && peopleContact.TrackingEmailSent && peopleContact.SecondTrackingEmailSent)
            {
                peopleContact.WorkflowState = WorkflowState.Finished;
            }
        }

        /// <summary>
        /// Creates the events before presented.
        /// </summary>
        /// <param name="peopleContact">The people contact.</param>
        private void CreateEventsBeforePresented(PeopleContact peopleContact)
        {
            peopleContact.FirstContacted = null;
            peopleContact.Presented = null;
            peopleContact.TrackingEmailSent = false;
            peopleContact.SecondContacted = null;
            peopleContact.SecondMeeting = null;
            peopleContact.SecondTrackingEmailSent = false;
            peopleContact.BusinessInfoParticipated = null;
            peopleContact.ThirdMeeting = null;
            peopleContact.TeamMeetingParticipated = null;
            peopleContact.WorkflowState = WorkflowState.Idle;
        }

        /// <summary>
        /// Creates the events in presented.
        /// </summary>
        /// <param name="peopleContact">The people contact.</param>
        private void CreateEventsInPresented(PeopleContact peopleContact)
        {
            peopleContact.Presented = null;
            peopleContact.TrackingEmailSent = false;
            peopleContact.SecondContacted = null;
            peopleContact.SecondMeeting = null;
            peopleContact.SecondTrackingEmailSent = false;
            peopleContact.BusinessInfoParticipated = null;
            peopleContact.ThirdMeeting = null;
            peopleContact.TeamMeetingParticipated = null;
            peopleContact.WorkflowState = WorkflowState.Idle;
        }

        /// <summary>
        /// Creates the events after presented.
        /// </summary>
        /// <param name="peopleContact">The people contact.</param>
        private void CreateEventsAfterPresented(PeopleContact peopleContact)
        {
            peopleContact.TrackingEmailSent = false;
            peopleContact.SecondContacted = null;
            peopleContact.SecondMeeting = null;
            peopleContact.SecondTrackingEmailSent = false;
            peopleContact.BusinessInfoParticipated = null;
            peopleContact.ThirdMeeting = null;
            peopleContact.TeamMeetingParticipated = null;
            peopleContact.WorkflowState = WorkflowState.Presented;
        }

        /// <summary>
        /// Creates the events after presented.
        /// </summary>
        /// <param name="peopleContact">The people contact.</param>
        /// <param name="calendar">The calendar.</param>
        /// <returns>Task.</returns>
        private void CreateEventsAfterPresentedAsync(PeopleContact peopleContact, Calendar calendar)
        {
            CreateEventsAfterPresented(peopleContact);

            if (calendar == null || (calendar.UseGoogleCalendar && !calendar.Authorized))
                return;

            var events = new Event[2];
            DateTime startDate = peopleContact.Presented.GetValueOrDefault(DateTime.Today).AddDays(1);
            events[0] = new Event
                            {
                                Start = new EventDateTime { DateTime = startDate },
                                End = new EventDateTime { DateTime = startDate.AddDays(1) },
                                Summary = String.Format(GoogleCalendarResource.PeopleContactController_CreateEventsAfterPresentedAsync1_Summary, peopleContact.LastName, peopleContact.FirstName),
                                Description = String.Format(GoogleCalendarResource.PeopleContactController_CreateEventsAfterPresentedAsync1_Description, peopleContact.LastName, peopleContact.FirstName)
                            };

            events[1] = new Event
                            {
                                Start = new EventDateTime { DateTime = startDate },
                                End = new EventDateTime { DateTime = startDate.AddDays(14) },
                                Summary = String.Format(GoogleCalendarResource.PeopleContactController_CreateEventsAfterPresentedAsync2_Summary, peopleContact.LastName, peopleContact.FirstName),
                                Description = String.Format(GoogleCalendarResource.PeopleContactController_CreateEventsAfterPresentedAsync2_Description, peopleContact.LastName, peopleContact.FirstName)
                            };
            calendar.InsertEvents(events);
        }

        /// <summary>
        /// Creates the events after second contacted.
        /// </summary>
        /// <param name="peopleContact">The people contact.</param>
        private void CreateEventsAfterSecondContacted(PeopleContact peopleContact)
        {
            peopleContact.SecondMeeting = null;
            peopleContact.SecondTrackingEmailSent = false;
            peopleContact.TeamMeetingParticipated = null;
            peopleContact.WorkflowState = !peopleContact.BusinessInfoParticipated.HasValue
                                              ? WorkflowState.SecondContacted
                                              : WorkflowState.SecondContactedAndBusinessInfoParticipated;
        }

        /// <summary>
        /// Creates the events after second contacted.
        /// </summary>
        /// <param name="peopleContact">The people contact.</param>
        /// <param name="calendar">The calendar.</param>
        /// <returns>Task.</returns>
        private void CreateEventsAfterSecondContactedAsync(PeopleContact peopleContact, Calendar calendar)
        {
            CreateEventsAfterSecondContacted(peopleContact);

            if (calendar.UseGoogleCalendar && !calendar.Authorized)
                return;

            var events = new Event[1];
            DateTime startDate = peopleContact.SecondContacted.GetValueOrDefault(DateTime.Today).AddDays(1);
            events[0] = new Event
            {
                Start = new EventDateTime { DateTime = startDate },
                End = new EventDateTime { DateTime = startDate.AddDays(2) },
                Summary = String.Format(GoogleCalendarResource.PeopleContactController_CreateEventsAfterSecondContactedAsync_Summary, peopleContact.LastName, peopleContact.FirstName),
                Description = String.Format(GoogleCalendarResource.PeopleContactController_CreateEventsAfterSecondContactedAsync_Description, peopleContact.LastName, peopleContact.FirstName)
            };
            calendar.InsertEvents(events);
        }

        /// <summary>
        /// Creates the events in reset business info.
        /// </summary>
        /// <param name="peopleContact">The people contact.</param>
        private void CreateEventsInResetBusinessInfo(PeopleContact peopleContact)
        {
            if (peopleContact.WorkflowState == WorkflowState.BusinessInfoParticipated)
            {
                CreateEventsAfterPresented(peopleContact);
            }
            else
            {
                peopleContact.BusinessInfoParticipated = null;
                peopleContact.ThirdMeeting = null;
                peopleContact.TeamMeetingParticipated = null;
                peopleContact.WorkflowState = WorkflowState.SecondContacted;
            }
        }

        /// <summary>
        /// Creates the events after business info participated.
        /// </summary>
        /// <param name="peopleContact">The people contact.</param>
        private void CreateEventsAfterBusinessInfoParticipated(PeopleContact peopleContact)
        {
            peopleContact.ThirdMeeting = null;
            peopleContact.TeamMeetingParticipated = null;
            peopleContact.WorkflowState = !peopleContact.SecondContacted.HasValue
                                              ? WorkflowState.BusinessInfoParticipated
                                              : WorkflowState.SecondContactedAndBusinessInfoParticipated;
        }

        /// <summary>
        /// Creates the events after business info participated.
        /// </summary>
        /// <param name="peopleContact">The people contact.</param>
        /// <param name="calendar">The calendar.</param>
        /// <returns>Task.</returns>
        private void CreateEventsAfterBusinessInfoParticipatedAsync(PeopleContact peopleContact, Calendar calendar)
        {
            CreateEventsAfterBusinessInfoParticipated(peopleContact);

            if (calendar.UseGoogleCalendar && !calendar.Authorized)
                return;

            var events = new Event[1];
            DateTime startDate = peopleContact.BusinessInfoParticipated.GetValueOrDefault(DateTime.Today).AddDays(2);
            events[0] = new Event
            {
                Start = new EventDateTime { DateTime = startDate },
                End = new EventDateTime { DateTime = startDate.AddDays(3) },
                Summary = String.Format(GoogleCalendarResource.PeopleContactController_CreateEventsAfterBusinessInfoParticipatedAsync_Summary, peopleContact.LastName, peopleContact.FirstName),
                Description = String.Format(GoogleCalendarResource.PeopleContactController_CreateEventsAfterBusinessInfoParticipatedAsync_Description, peopleContact.LastName, peopleContact.FirstName)
            };
            calendar.InsertEvents(events);
        }

        /// <summary>
        /// Stops the workflow.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult StopWorkflow(int id)
        {
            PeopleContact peopleContact = Db.PeopleContacts.Single(pc => pc.PeopleContactId == id);
            if (peopleContact == null || peopleContact.WorkflowState == WorkflowState.Stopped)
                return View("StopWorkflowEmpty", "_PopupLayout");

            if (peopleContact.RegistrarId != UserId)
                return RedirectToAccessDenied();

            return View("StopWorkflow", "_PopupLayout");
        }

        /// <summary>
        /// Stops the workflow.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="peopleContactStopWorkflow">The people contact stop workflow.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        public ActionResult StopWorkflow(int id, PeopleContactStopWorkflow peopleContactStopWorkflow)
        {
            PeopleContact peopleContact = Db.PeopleContacts.Single(pc => pc.PeopleContactId == id);
            if (peopleContact == null || peopleContact.WorkflowState == WorkflowState.Stopped)
                return View("StopWorkflowEmpty", "_PopupLayout");

            if (peopleContact.RegistrarId != UserId)
                return RedirectToAccessDenied();

            if (ModelState.IsValid)
            {
                var note = new StringBuilder();
                note.AppendLine(peopleContactStopWorkflow.Note);
                if (!String.IsNullOrEmpty(peopleContact.Note))
                {
                    note.AppendLine();
                    note.AppendLine(peopleContact.Note);
                }

                peopleContact.Note = note.ToString();
                peopleContact.WorkflowStatePrevious = peopleContact.WorkflowState;
                peopleContact.WorkflowState = WorkflowState.Stopped;
                Db.Entry(peopleContact).State = EntityState.Modified;
                Db.SaveChanges();

                return View("StopWorkflowSummary", "_PopupLayout", peopleContactStopWorkflow);
            }

            return View("StopWorkflow", "_PopupLayout", peopleContactStopWorkflow);
        }

        /// <summary>
        /// Starts the workflow.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult StartWorkflow(int id)
        {
            PeopleContact peopleContact = Db.PeopleContacts.Single(pc => pc.PeopleContactId == id);
            if (peopleContact != null && peopleContact.WorkflowState == WorkflowState.Stopped && peopleContact.RegistrarId == UserId)
            {
                peopleContact.WorkflowState = peopleContact.WorkflowStatePrevious;
                Db.Entry(peopleContact).State = EntityState.Modified;
                Db.SaveChanges();
            }

            return RedirectToAction("Index", "Task");
        }

        public ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Import(Import import)
        {
            import.Validate(this);

            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.Success = false;

                    using (var lyconetService = new LyconetService(Db, UserId))
                    {
                        lyconetService.ReadDataFromCsvFile(import.File.InputStream);
                        lyconetService.UpdatePeopleContacts();
                        ViewBag.Message = lyconetService.SummaryMessage;
                    }

                    ViewBag.Success = true;
                }
                catch (Exception e)
                {
                    ViewBag.Message = String.Format(ValidationResource.PeopleContact_ParsingCsv_ErrorMessage, e.Message);
                }

                return View("ImportSummary");
            }

            return View(import);
        }

        public ActionResult SendVideo(int id = 0)
        {
            PeopleContact peopleContact = PeopleContactCache.GetDetail(Db, id);
            if (!IsAccessSendVideo(peopleContact))
            {
                return RedirectToAccessDenied();
            }

            PeopleContactSendVideo peopleContactSendVideo = PeopleContactSendVideo.GetModelView(peopleContact);

            PopulateVideoId(UserId, IsAdmin, peopleContactSendVideo.VideoId);

            return View(peopleContactSendVideo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendVideo(PeopleContactSendVideo peopleContactSendVideo)
        {
            ModelState.Merge(peopleContactSendVideo.Validate(Db, UserId, Url));

            if (ModelState.IsValid)
            {
                TempData[StatusMessageTempKey] = ViewResource.PeopleContact_VideoWasSuccessfullySent_Text;
                return RedirectToAction("Details", new {id = peopleContactSendVideo.PeopleContactId});
            }

            PopulateVideoId(UserId, IsAdmin, peopleContactSendVideo.VideoId);

            return View(peopleContactSendVideo);
        }

        public FileContentResult Export()
        {
            var csvBytes = new byte[0];
            try
            {
                using (var lyconetService = new LyconetService(Db, UserId))
                {
                    csvBytes = lyconetService.GetPeopleContacts();
                }
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
            }

            return File(csvBytes, ExportCsvContentType, ViewResource.PeopleContact_ExportFileName_Text);
        }

        private bool IsAccessDetails(PeopleContact peoplecontact)
        {
            bool isAccess = peoplecontact != null && FilteredUser.GetFilteredUsers(Db, UserId).Any(gfu => gfu.UserId == peoplecontact.RegistrarId);
            return isAccess;
        }

        private bool IsAccessEdit(PeopleContactEdit peopleContactEdit)
        {
            return peopleContactEdit.RegistrarId == UserId;
        }

        private bool IsAccessSendVideo(PeopleContact peopleContact)
        {
            bool isAccess = IsAccessDetails(peopleContact) && !String.IsNullOrEmpty(peopleContact.Email1);
            return isAccess;
        }
    }
}