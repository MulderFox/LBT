// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 12-20-2013
//
// Last Modified By : zmikeska
// Last Modified On : 01-12-2014
// ***********************************************************************
// <copyright file="UserProfileController.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.Cache;
using LBT.Filters;
using LBT.Helpers;
using LBT.Models;
using LBT.ModelViews;
using LBT.Services.GoogleApis;
using PagedList;
using LBT.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LBT.Controllers
{
    /// <summary>
    /// Class UserProfileController
    /// </summary>
    [InitializeSimpleMembership]
    [Authorize]
    public class UserProfileController : BaseController
    {
        //
        // GET: /UserProfile/

        /// <summary>
        /// Indexes the specified sort order.
        /// </summary>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="currentFilter">The current filter.</param>
        /// <param name="currentFilterAccording">The current filter according.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="searchStringAccording">The search string according.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>ActionResult.</returns>
        [Authorize(Roles = LeadingRoles)]
        public ActionResult Index(string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page, int pageSize = PageSize)
        {
            string cookie = Cookie.GetCookie(Request, Cookie.SwitchToUserProfileIndexTreeCookieKey);
            if (!String.IsNullOrEmpty(cookie) && cookie == Cookie.CookieTrue)
            {
                return RedirectToAction("IndexTree");
            }

            int pageNumber;
            ProcessSearchFilterAndPaging(currentFilter, currentFilterAccording, ref searchString, ref searchStringAccording, ref page, out pageNumber);

            var sortingNames = new[]
                                   {
                                       BaseCache.LastNameField, BaseCache.FirstNameField, BaseCache.CityField,
                                       BaseCache.LyonessIdField
                                   };
            ProcessSorting(sortOrder, sortingNames);

            PopulateSearchStringAccording(searchString, searchStringAccording);
            PopulatePageSize(pageSize);

            IEnumerable<UserProfileIndex> userProfiles = IsAdmin
                                                             ? UserProfileIndex.GetUserProfileIndexForAdmin(Db, UserId, searchString, searchStringAccording, sortOrder)
                                                             : UserProfileIndex.GetUserProfileIndex(Db, UserId, searchString, searchStringAccording, sortOrder);
            return View(userProfiles.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Indexes the tree.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>ActionResult.</returns>
        [Authorize(Roles = LeadingRoles)]
        public ActionResult IndexTree(int? page, int pageSize = PageSize)
        {
            int pageNumber;
            ProcessPaging(page, out pageNumber);

            PopulatePageSize(pageSize);

            IEnumerable<UserProfileIndexTree> userProfileIndexTree = IsAdmin
                                                                         ? UserProfileIndexTree.GetUserProfileIndexTreeForAdmin(Db, UserId)
                                                                         : UserProfileIndexTree.GetUserProfileIndexTree(Db, UserId);
            return View(userProfileIndexTree.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /UserProfile/Details/5

        /// <summary>
        /// Detailses the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        [Authorize(Roles = LeadingRoles)]
        public ActionResult Details(int id = 0)
        {
            UserProfile userProfile = UserProfileCache.GetDetail(Db, id);
            if (!IsAccess(userProfile, id))
            {
                return RedirectToAccessDenied();
            }

            UserProfileDetails userProfileDetails = UserProfileDetails.GetModelView(userProfile);

            return View(userProfileDetails);
        }

        //
        // GET: /UserProfile/Edit/5

        /// <summary>
        /// Edits the specified return URL.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        [Authorize(Roles = LeadingRoles)]
        public ActionResult Edit(int id = 0)
        {
            UserProfile userProfile = UserProfileCache.GetDetail(Db, id);
            if (!IsEditable(userProfile, id))
            {
                return RedirectToAccessDenied();
            }

            var userProfileEdit = new UserProfileEdit(userProfile);

            PopulateRole(userProfile.Role);
            PopulateDistrictId(userProfile.DistrictId);
            PopulatePhoneNumberPrefix1Id(userProfile.PhoneNumberPrefix1Id);
            PopulatePhoneNumberPrefix2Id(userProfile.PhoneNumberPrefix2Id);
            PopulateClaAccessChangedCurrency(userProfile.ClaAccessAmount, userProfile.ClaAccessCurrency, userProfile.ClaAccessCurrency);

            return View(userProfileEdit);
        }

        //
        // POST: /UserProfile/Edit/5

        /// <summary>
        /// Edits the specified userProfileEdit.
        /// </summary>
        /// <param name="userProfileEdit">The userProfileEdit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AsyncTimeout(60000)]
        [Authorize(Roles = LeadingRoles)]
        public async Task<ActionResult> Edit(UserProfileEdit userProfileEdit, CancellationToken cancellationToken)
        {
            if (!IsAccess(userProfileEdit.UserId))
            {
                return RedirectToAccessDenied();
            }

            ModelState.Merge(await userProfileEdit.Validate(Db, this, cancellationToken, IsAdmin));

            if (ModelState.IsValid)
            {
                UserProfile userProfile;
                bool success = UserProfileCache.Update(Db, userProfileEdit, IsAdmin, out userProfile);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("Index");
            }

            PopulateRole(userProfileEdit.Role);
            PopulateDistrictId(userProfileEdit.DistrictId);
            PopulatePhoneNumberPrefix1Id(userProfileEdit.PhoneNumberPrefix1Id);
            PopulatePhoneNumberPrefix2Id(userProfileEdit.PhoneNumberPrefix2Id);
            PopulateClaAccessChangedCurrency(userProfileEdit.ClaAccessAmount, userProfileEdit.ClaAccessCurrency, userProfileEdit.ClaAccessCurrency);

            return View(userProfileEdit);
        }

        //
        // GET: /UserProfile/Delete/5

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        [Authorize(Roles = AdminRoles)]
        public ActionResult Delete(int id = 0)
        {
            if (id == UserId)
                return RedirectToAccessDenied();

            UserProfile userprofile = Db.UserProfiles.Find(id);
            if (userprofile == null)
            {
                return RedirectToAccessDenied();
            }

            bool isUserLeader = UserProfileCache.GetLeadedUserIds(Db, UserId).Contains(userprofile.UserId);
            if (!IsAdmin && !isUserLeader)
            {
                return RedirectToAccessDenied();
            }

            UserProfileDelete userProfileDelete = UserProfileDelete.GetModelView(userprofile);

            return View(userProfileDelete);
        }

        //
        // POST: /UserProfile/Delete/5

        /// <summary>
        /// Deletes the confirmed.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AdminRoles)]
        public ActionResult DeleteConfirmed(int id)
        {
            UserProfile userProfile;
            DeleteResult deleteResult = UserProfileCache.Delete(Db, Url, id, UserId, IsAdmin, out userProfile);

            UserProfileDelete userProfileDelete;
            switch (deleteResult)
            {
                case DeleteResult.Ok:
                    return RedirectToAction("Index");

                case DeleteResult.AuthorizationFailed:
                    return RedirectToAccessDenied();

                case DeleteResult.AdminAccountProtection:
                    ModelState.AddModelError(BaseCache.TitleField, ViewResource.Account_AdminAccountProtection_Text);
                    userProfileDelete = UserProfileDelete.GetModelView(userProfile);
                    return View(userProfileDelete);

                case DeleteResult.DbFailed:
                    ModelState.AddModelError(BaseCache.TitleField, ValidationResource.Global_DeleteRecord_ErrorMessage);
                    userProfileDelete = UserProfileDelete.GetModelView(userProfile);
                    return View(userProfileDelete);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Lockeds the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        [Authorize(Roles = LeadingRoles)]
        public ActionResult Locked(int id = 0)
        {
            if (id == UserId)
                return RedirectToAccessDenied();

            if (!IsAdmin)
            {
                IEnumerable<int> leadedUserIds = UserProfileCache.GetLeadedUserIds(Db, UserId);
                if (!leadedUserIds.Contains(id))
                {
                    return RedirectToAccessDenied();
                }
            }

            UserProfile userprofile = UserProfileCache.GetDetail(Db, id);
            if (userprofile == null)
            {
                return RedirectToAccessDenied();
            }

            UserProfileLockedUnlocked userProfileLockedUnlocked = UserProfileLockedUnlocked.GetModelView(userprofile);

            return View(userProfileLockedUnlocked);
        }

        /// <summary>
        /// Lockeds the confirmed.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost, ActionName("Locked")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult LockedConfirmed(int id)
        {
            if (id == UserId)
                return RedirectToAccessDenied();

            if (!IsAdmin)
            {
                IEnumerable<int> leadedUserIds = UserProfileCache.GetLeadedUserIds(Db, UserId);
                if (!leadedUserIds.Contains(id))
                {
                    return RedirectToAccessDenied();
                }
            }

            UserProfile userProfile = UserProfileCache.GetDetail(Db, id);
            userProfile.Active = false;
            Db.Entry(userProfile).State = EntityState.Modified;
            Db.SaveChanges();

            Mail.SendEmail(userProfile.Email1, MailResource.UserProfileController_Locked_Subject, MailResource.UserProfileController_Locked_TextBody, true, true);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Unlockeds the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        [Authorize(Roles = LeadingRoles)]
        public ActionResult Unlocked(int id = 0)
        {
            if (id == UserId)
                return RedirectToAccessDenied();

            if (!IsAdmin)
            {
                IEnumerable<int> leadedUserIds = UserProfileCache.GetLeadedUserIds(Db, UserId);
                if (!leadedUserIds.Contains(id))
                {
                    return RedirectToAccessDenied();
                }
            }

            UserProfile userprofile = UserProfileCache.GetDetail(Db, id);
            if (userprofile == null)
            {
                return RedirectToAccessDenied();
            }

            UserProfileLockedUnlocked userProfileLockedUnlocked = UserProfileLockedUnlocked.GetModelView(userprofile);

            return View(userProfileLockedUnlocked);
        }

        /// <summary>
        /// Unlockeds the confirmed.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost, ActionName("Unlocked")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult UnlockedConfirmed(int id)
        {
            if (id == UserId)
                return RedirectToAccessDenied();

            if (!IsAdmin)
            {
                IEnumerable<int> leadedUserIds = UserProfileCache.GetLeadedUserIds(Db, UserId);
                if (!leadedUserIds.Contains(id))
                {
                    return RedirectToAccessDenied();
                }
            }

            UserProfile userProfile = UserProfileCache.GetDetail(Db, id);
            userProfile.Active = true;
            Db.Entry(userProfile).State = EntityState.Modified;
            Db.SaveChanges();

            Mail.SendEmail(userProfile.Email1, MailResource.UserProfileController_Unlocked_Subject, MailResource.UserProfileController_Unlocked_TextBody, true, true);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Validates the async.
        /// </summary>
        /// <param name="startValidation">if set to <c>true</c> [start validation].</param>
        /// <returns>ActionResult.</returns>
        public ActionResult ValidateAsync(bool startValidation = false)
        {
            if (startValidation)
            {
                ViewBag.GoogleCredentialsJson = String.Empty;
                string cookie = Cookie.GetCookie(Request, Cookie.TestGoogleCredentialsJsonCookieKey);
                if (!String.IsNullOrEmpty(cookie))
                {
                    ViewBag.GoogleCredentialsJson = Server.UrlDecode(cookie);
                }

                ViewBag.GoogleCalendarId = String.Empty;
                cookie = Cookie.GetCookie(Request, Cookie.TestGoogleCalendarIdCookieKey);
                if (!String.IsNullOrEmpty(cookie))
                {
                    ViewBag.GoogleCalendarId = Server.UrlDecode(cookie);
                }

                ViewBag.IsError = false;
                ViewBag.ShowForm = true;
            }
            else
            {
                ViewBag.Message = ViewResource.UserProfile_GoogleCredentialsJsonValidationSuccess_Text;
                ViewBag.IsError = false;
                ViewBag.ShowForm = false;
            }

            return View("Validate", "_PopupLayout");
        }

        /// <summary>
        /// Validates the async.
        /// </summary>
        /// <param name="googleCredentialsJson">The google credentials json.</param>
        /// <param name="googleCalendarId">The google calendar id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{ActionResult}.</returns>
        [HttpPost]
        [AsyncTimeout(60000)]
        public async Task<ActionResult> ValidateAsync(string googleCredentialsJson, string googleCalendarId, CancellationToken cancellationToken)
        {
            var calendar = new Calendar
                               {
                                   GoogleCredentialsJson = googleCredentialsJson,
                                   GoogleCalendarId = googleCalendarId,
                                   UseGoogleCalendarByUser = true
                               };
            await calendar.AuthorizeAsync(this, cancellationToken);

            if (calendar.NeedRedirectToGoogle)
                return new RedirectResult(calendar.RedirectUri);

            if (calendar.Authorized)
                return RedirectToAction("ValidateAsync", "UserProfile");

            ViewBag.Message = ValidationResource.UserProfile_GoogleCredentialsJsonValidationFailed_ErrorMessage;
            ViewBag.IsError = true;
            ViewBag.ShowForm = false;

            return View("Validate", "_PopupLayout");
        }

        /// <summary>
        /// Prints the index.
        /// </summary>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="currentFilter">The current filter.</param>
        /// <param name="currentFilterAccording">The current filter according.</param>
        /// <returns>ActionResult.</returns>
        [Authorize(Roles = LeadingRoles)]
        public ActionResult PrintIndex(string sortOrder, string currentFilter, string currentFilterAccording)
        {
            IEnumerable<UserProfilePrintIndex> userProfiles = UserProfileCache.GetLeadedUsers(Db, UserId, User, currentFilter, currentFilterAccording, sortOrder);
            if (userProfiles.Count() > 1000)
            {
                userProfiles = userProfiles.Take(1000);
                ViewBag.IsListCuttedMessage = ViewResource.Global_IsListCutted_Text;
            }

            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.CurrentFilterAccording = currentFilterAccording;

            return View("PrintIndex", "_PrintLayout", userProfiles);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult Promote(string returnUrl, int id = 0)
        {
            UserProfilePromote viewModel = UserProfilePromote.GetViewModel(this, id, returnUrl);
            return viewModel == null ? RedirectToAccessDenied() : RedirectToAction(viewModel.ReturnUrl);
        }

        private bool IsAccess(UserProfile userProfile, int id)
        {
            if (userProfile == null)
                return false;

            return IsAccess(id);
        }

        private bool IsEditable(UserProfile userProfile, int id)
        {
            if (userProfile == null || userProfile.IsPoliciesAccepted && !IsAdmin)
                return false;

            return IsAccess(id);
        }

        private bool IsAccess(int id)
        {
            if (UserId == id)
                return false;

            if (!IsAdmin)
            {
                bool isUserLeader = UserProfileCache.GetLeadedUserIds(Db, UserId).Contains(id);
                if (!isUserLeader)
                    return false;
            }

            return true;
        }
    }
}