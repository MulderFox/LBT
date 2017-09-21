// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 12-20-2013
//
// Last Modified By : zmikeska
// Last Modified On : 01-16-2014
// ***********************************************************************
// <copyright file="AccountController.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.Cache;
using LBT.Filters;
using LBT.Helpers;
using LBT.Models;
using LBT.ModelViews;
using LBT.Resources;
using LBT.Services.GoogleApis;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace LBT.Controllers
{
    /// <summary>
    /// Class AccountController
    /// </summary>
    [InitializeSimpleMembership]
    [Authorize]
    public class AccountController : BaseController
    {
        //
        // GET: /Account/Login

        /// <summary>
        /// Logins the specified return URL.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <param name="lcid">The lcid.</param>
        /// <returns>ActionResult.</returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, int? lcid = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            PopulateLanguage(lcid);

            return View();
        }

        //
        // POST: /Account/Login

        /// <summary>
        /// Logins by the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <param name="lcid">The lcid.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Login model, string returnUrl, int? lcid = null)
        {
            ModelState.Merge(model.Validate(Db));

            if (ModelState.IsValid)
            {
                bool getGoogleToken;
                UserProfileCache.Login(Db, model, out getGoogleToken);

                if (getGoogleToken)
                {
                    ViewBag.ReturnUrl = returnUrl;
                    return RedirectToAction("GetGoogleToken", new { returnUrl, model.UserId });
                }

                return RedirectToLocal(returnUrl);
            }

            PopulateLanguage(lcid);

            return View(model);
        }

        //
        // POST: /Account/LogOff

        /// <summary>
        /// Logs off.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        /// <summary>
        /// Registers a user.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [Authorize(Roles = LeadingRoles)]
        public ActionResult Register()
        {
            PopulateDistrictId();
            PopulatePhoneNumberPrefix1Id();
            PopulatePhoneNumberPrefix2Id();

            UserProfile userProfile = UserProfileCache.GetDetail(Db, UserId);
            PopulateCurrencyType(userProfile.ClaAccessCurrency);

            return View();
        }

        //
        // POST: /Account/Register

        /// <summary>
        /// Registers a user by the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>ActionResult.</returns>
        //[HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [HttpPost]
        [Authorize(Roles = LeadingRoles)]
        [ValidateAntiForgeryToken]
        [AsyncTimeout(60000)]
        public async Task<ActionResult> Register(Register model, CancellationToken cancellationToken)
        {
            ModelState.Merge(await model.Validate(this, cancellationToken, Db, UserId));

            if (ModelState.IsValid)
            {
                UserProfileCache.RegisterResult registerResult;
                ModelState.Merge(UserProfileCache.Register(Db, model, out registerResult));

                switch (registerResult)
                {
                    case UserProfileCache.RegisterResult.Ok:
                        return RedirectToAction("Index", "UserProfile");

                    case UserProfileCache.RegisterResult.CannotSendRegisterEmail:
                        TempData[StatusMessageTempKey] = ValidationResource.Account_CannotSendRegisterEmail_ErrorMessage;
                        return RedirectToAction("Index", "UserProfile");

                    case UserProfileCache.RegisterResult.Failed:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            PopulateDistrictId(model.DistrictId);
            PopulatePhoneNumberPrefix1Id(model.PhoneNumberPrefix1Id);
            PopulatePhoneNumberPrefix2Id(model.PhoneNumberPrefix2Id);
            PopulateCurrencyType(model.CurrencyType);

            return View("Register", model);
        }

        /// <summary>
        /// Registers the people contact.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        [Authorize(Roles = LeadingRoles)]
        public ActionResult RegisterPeopleContact(int id)
        {
            PeopleContact peopleContact = PeopleContactCache.GetDetail(Db, id);
            if (!IsAccess(peopleContact))
            {
                return RedirectToAccessDenied();
            }

            var model = new Register();
            model.CopyFrom(peopleContact);

            PopulateDistrictId(peopleContact.DistrictId);
            PopulatePhoneNumberPrefix1Id(peopleContact.PhoneNumberPrefix1Id);
            PopulatePhoneNumberPrefix2Id(peopleContact.PhoneNumberPrefix2Id);

            UserProfile userProfile = UserProfileCache.GetDetail(Db, UserId);
            PopulateCurrencyType(userProfile.ClaAccessCurrency);

            return View("Register", model);
        }

        /// <summary>
        /// Registers the people contact.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{ActionResult}.</returns>
        [HttpPost, ActionName("RegisterPeopleContact")]
        [Authorize(Roles = LeadingRoles)]
        [ValidateAntiForgeryToken]
        [AsyncTimeout(60000)]
        public async Task<ActionResult> RegisterPeopleContact(Register model, CancellationToken cancellationToken)
        {
            return await Register(model, cancellationToken);
        }

        //
        // GET: /Account/Manage

        /// <summary>
        /// Manages user with the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? ViewResource.Account_ChangePasswordSuccess_Text
                : message == ManageMessageId.SetPasswordSuccess ? ViewResource.Account_SetPasswordSuccess_Text
                : message == ManageMessageId.RemoveLoginSuccess ? ViewResource.Account_RemoveLoginSuccess_Text
                : String.Empty;
            ViewBag.ReturnUrl = Url.Action("Manage");

            return View();
        }

        //
        // POST: /Account/Manage

        /// <summary>
        /// Manages the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPassword model)
        {
            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                try
                {
                    bool changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch (Exception) { }
                // ReSharper restore EmptyGeneralCatchClause

                ModelState.AddModelError(String.Empty, ValidationResource.Account_ChangePasswordUnsucceeded_ErrorMessage);
            }

            ViewBag.ReturnUrl = Url.Action("Manage");
            return View(model);
        }

        //
        // GET: /Account/Settings/5

        /// <summary>
        /// Edits the specified return URL.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Settings()
        {
            UserProfile userProfile = UserProfileCache.GetDetail(Db, UserId);
            if (!IsAccess(userProfile))
            {
                return RedirectToAccessDenied();
            }

            var settingsProfile = new SettingsProfile(userProfile);

            PopulateLanguage(settingsProfile.LCIDDropDownList);
            PopulateAutomaticLogoutInterval(settingsProfile.AutomaticLogoutInterval);
            PopulateDistrictId(settingsProfile.DistrictId);
            PopulatePhoneNumberPrefix1Id(settingsProfile.PhoneNumberPrefix1Id);
            PopulatePhoneNumberPrefix2Id(settingsProfile.PhoneNumberPrefix2Id);
            PopulateClaAccessChangedCurrency(userProfile.ClaAccessAmount, userProfile.ClaAccessCurrency, userProfile.ClaAccessCurrency);

            return View(settingsProfile);
        }

        //
        // POST: /UserProfile/Edit/5

        /// <summary>
        /// Edits the specified settingsProfile.
        /// </summary>
        /// <param name="settingsProfile">The settingsProfile.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AsyncTimeout(60000)]
        public async Task<ActionResult> Settings(SettingsProfile settingsProfile, CancellationToken cancellationToken)
        {
            if (!IsAccess(settingsProfile, UserId))
            {
                return RedirectToAccessDenied();
            }

            ModelState.Merge(await settingsProfile.Validate(Db, this, cancellationToken, IsAdmin));

            if (ModelState.IsValid)
            {
                UserProfile userProfile;
                bool success = UserProfileCache.Update(Db, settingsProfile, out userProfile);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("Index", "Home");
            }

            PopulateLanguage(settingsProfile.LCIDDropDownList);
            PopulateAutomaticLogoutInterval(settingsProfile.AutomaticLogoutInterval);
            PopulateDistrictId(settingsProfile.DistrictId);
            PopulatePhoneNumberPrefix1Id(settingsProfile.PhoneNumberPrefix1Id);
            PopulatePhoneNumberPrefix2Id(settingsProfile.PhoneNumberPrefix2Id);
            PopulateClaAccessChangedCurrency(settingsProfile.OriginalClaAccessAmount, settingsProfile.OriginalClaAccessCurrency, settingsProfile.ClaAccessChangedCurrency);

            return View(settingsProfile);
        }

        /// <summary>
        /// Passwords the recovery.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [AllowAnonymous]
        public ActionResult PasswordRecovery(int? lcid = null)
        {
            PopulateLanguage(lcid);

            return View();
        }

        /// <summary>
        /// Passwords the recovery.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="lcid">The lcid.</param>
        /// <returns>System.Web.Mvc.ActionResult.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordRecovery(RecoveryPassword model, int? lcid = null)
        {
            if (ModelState.IsValid)
            {
                ModelState.Merge(model.Validate(Db));
            }

            if (ModelState.IsValid)
            {
                TempData[StatusMessageTempKey] = ViewResource.Account_NewPasswordWasSent_Text;
                return RedirectToAction("Login", "Account");
            }

            PopulateLanguage(lcid);

            return View(model);
        }

        /// <summary>
        /// Gets the google token.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{ActionResult}.</returns>
        /// <exception cref="System.Exception">Cannot receive Google Calendar token or connect to the calendar.</exception>
        public async Task<ActionResult> GetGoogleToken(string returnUrl, int userId, CancellationToken cancellationToken)
        {
            try
            {
                UserProfile userProfile = Db.UserProfiles.Find(userId);
                var calendar = new Calendar
                {
                    GoogleCredentialsJson = userProfile.GoogleCredentialsJson,
                    GoogleCalendarId = userProfile.GoogleCalendarId,
                    UseGoogleCalendarByUser = userProfile.UseGoogleCalendar
                };
                await calendar.AuthorizeAsync(this, cancellationToken);

                if (calendar.NeedRedirectToGoogle)
                    return new RedirectResult(calendar.RedirectUri);
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
            }

            return RedirectToLocal(returnUrl);
        }

        public ActionResult CheckBillingInformation()
        {
            var checkBillingInformation = new CheckBillingInformation(Db, UserId);
            if (!IsAccess(checkBillingInformation))
            {
                return RedirectToAccessDenied();
            }

            return View(checkBillingInformation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckBillingInformation(CheckBillingInformation checkBillingInformation)
        {
            if (!IsAccess(checkBillingInformation))
            {
                return RedirectToAccessDenied();
            }

            ModelState.Merge(checkBillingInformation.Validate());

            if (ModelState.IsValid)
            {
                UserProfile userProfile;
                bool success = UserProfileCache.Update(Db, checkBillingInformation, out userProfile);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("CheckIsPoliciesAccepted", "Account");
            }

            return View(checkBillingInformation);
        }

        public ActionResult CheckIsPoliciesAccepted()
        {
            var checkPolicies = new CheckPolicies(Db, UserId, 0);
            if (!IsAccess(checkPolicies))
            {
                return RedirectToAccessDenied();
            }

            return View("CheckPolicies", checkPolicies);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckIsPoliciesAccepted(CheckPolicies checkPolicies)
        {
            switch (checkPolicies.ViewData)
            {
                default:
                    if (checkPolicies.ConfirmTermsAndConditions && checkPolicies.ConfirmPersonalData && checkPolicies.ConfirmContacts)
                    {
                        UserProfileCache.SetAcceptedPrivacy(Db, UserId);
                        return RedirectToAction("Index", "Home");
                    }

                    checkPolicies.ViewData = 1;
                    return View("DeleteAccount", checkPolicies);

                case 1:
                    int userId = UserId;
                    WebSecurity.Logout();

                    if (checkPolicies.ConfirmDeleteAccount)
                    {
                        DeleteResult deleteResult = UserProfileCache.Delete(Db, Url, userId);
                        switch (deleteResult)
                        {
                            case DeleteResult.Ok:
                                TempData[StatusMessageTempKey] = ViewResource.Account_AccountWasDeleted_Text;
                                break;

                            case DeleteResult.AdminAccountProtection:
                                TempData[StatusMessageTempKey] = ViewResource.Account_AdminAccountProtection_Text;
                                break;

                            default:
                                TempData[StatusMessageTempKey] = ValidationResource.Global_DbCommunicationFailed_ErrorMessage;
                                break;
                        }
                    }
                    else
                    {
                        TempData[StatusMessageTempKey] = ViewResource.Account_CannotContinue_Text;
                    }

                    return RedirectToAction("Login", "Account");
            }
        }

        /// <summary>
        /// Enum ManageMessageId
        /// </summary>
        public enum ManageMessageId
        {
            /// <summary>
            /// The change password success
            /// </summary>
            ChangePasswordSuccess,

            /// <summary>
            /// The set password success
            /// </summary>
            SetPasswordSuccess,

            /// <summary>
            /// The remove login success
            /// </summary>
            RemoveLoginSuccess,
        }

        private bool IsAccess(UserProfile userProfile)
        {
            return userProfile != null;
        }

        private bool IsAccess(SettingsProfile settingsProfile, int userId)
        {
            return settingsProfile.UserId == userId;
        }

        private bool IsAccess(PeopleContact peopleContact)
        {
            return peopleContact != null && FilteredUser.GetFilteredUsers(Db, UserId).Any(gfu => gfu.UserId == peopleContact.RegistrarId);
        }

        private bool IsAccess(BaseModelView baseModelView)
        {
            return baseModelView.IsValid;
        }
    }
}
