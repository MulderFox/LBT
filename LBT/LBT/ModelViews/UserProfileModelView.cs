// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 05-27-2014
//
// Last Modified By : zmikeska
// Last Modified On : 06-05-2014
// ***********************************************************************
// <copyright file="UserProfile.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.Cache;
using LBT.Controllers;
using LBT.DAL;
using LBT.Extensions;
using LBT.Helpers;
using LBT.Models;
using LBT.Resources;
using LBT.Services.GoogleApis;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LBT.ModelViews
{
    public class UserProfileIndexBase : BaseModelView
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>The user id.</value>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>The city.</value>
        public string City { get; set; }

        /// <summary>
        /// Gets the city short view.
        /// </summary>
        /// <value>The city short view.</value>
        public string CityIndexView { get { return Grammar.CutTextWithDots(City, 18); } }

        /// <summary>
        /// Gets or sets the lyoness id.
        /// </summary>
        /// <value>The lyoness id.</value>
        public string LyonessId { get; set; }

        /// <summary>
        /// Gets or sets the primary phone number.
        /// </summary>
        /// <value>The primary phone number.</value>
        public string PrimaryPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the email1.
        /// </summary>
        /// <value>The email1.</value>
        public string Email1 { get; set; }

        /// <summary>
        /// Gets the email short view.
        /// </summary>
        /// <value>The email short view.</value>
        public string Email1IndexView { get { return Grammar.CutTextWithDots(Email1, 28); } }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>
        public RoleType Role { get; set; }

        /// <summary>
        /// Gets the role acronym.
        /// </summary>
        /// <value>The role acronym.</value>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public string RoleAcronym
        {
            get
            {
                switch (Role)
                {
                    case RoleType.AdminLeader:
                        return ListItemsResource.RoleAcronym_AdminLeader;

                    case RoleType.Admin:
                        return ListItemsResource.RoleAcronym_Admin;

                    case RoleType.Leader:
                        return ListItemsResource.RoleAcronym_Leader;

                    default:
                        return ListItemsResource.RoleAcronym_AuthGuest;
                }
            }
        }

        /// <summary>
        /// Gets or sets the people contact count.
        /// </summary>
        /// <value>The people contact count.</value>
        public int PeopleContactCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UserProfileIndex" /> is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        public bool Active { get; set; }

        public DateTime? ClaAccessExpired { get; set; }

        protected DateTime? CurrentDateTime { get; set; }

        public string FormattingClass
        {
            get
            {
                if (CurrentDateTime.HasValue && ClaAccessExpired.HasValue && ClaAccessExpired < CurrentDateTime)
                    return ClaAccessExpiredClass;

                return String.Empty;
            }
        }

        public bool IsPoliciesAccepted { get; set; }

        public bool IsEditable { get; set; }

        public bool IsPromotable
        {
            get { return Role == RoleType.AuthGuest; }
        }
    }

    /// <summary>
    /// Class UserProfileIndex
    /// </summary>
    public class UserProfileIndex : UserProfileIndexBase
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="UserProfileIndex" /> class from being created.
        /// </summary>
        private UserProfileIndex()
        {
        }

        public static IEnumerable<UserProfileIndex> GetUserProfileIndex(DefaultContext db, int userId)
        {
            return GetUserProfileIndex(db, userId, String.Empty, String.Empty, String.Empty);
        }

        public static IEnumerable<UserProfileIndex> GetUserProfileIndex(DefaultContext db, int userId, string searchString, string searchStringAccording, string sortOrder)
        {
            // Nesmí být použity SqlParameters
            UserProfileIndex[] userProfiles = UserProfileCache.GetUserProfileIndex(db, userId, searchString, searchStringAccording, sortOrder);

            DateTime currentDateTime = DateTime.Now;
            foreach (UserProfileIndex userProfileIndex in userProfiles)
            {
                userProfileIndex.CurrentDateTime = currentDateTime;
            }

            return userProfiles;
        }

        public static IEnumerable<UserProfileIndex> GetUserProfileIndexForAdmin(DefaultContext db, int userId)
        {
            return GetUserProfileIndexForAdmin(db, userId, String.Empty, String.Empty, String.Empty);
        }

        /// <summary>
        /// Gets the user profile index for admin.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="searchStringAccording">The search string according.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>IEnumerable{UserProfileIndex}.</returns>
        public static IEnumerable<UserProfileIndex> GetUserProfileIndexForAdmin(DefaultContext db, int userId, string searchString, string searchStringAccording, string sortOrder)
        {
            // Nesmí být použity SqlParameters
            UserProfileIndex[] userProfiles = UserProfileCache.GetUserProfileIndexForAdmin(db, userId, searchString, searchStringAccording, sortOrder);

            DateTime currentDateTime = DateTime.Now;
            foreach (UserProfileIndex userProfileIndex in userProfiles)
            {
                userProfileIndex.CurrentDateTime = currentDateTime;
                userProfileIndex.IsEditable = true;
            }

            return userProfiles;
        }
    }

    /// <summary>
    /// Class UserProfileIndexTree
    /// </summary>
    public class UserProfileIndexTree : UserProfileIndexBase
    {
        /// <summary>
        /// Gets or sets the user level.
        /// </summary>
        /// <value>The user level.</value>
        public int UserLevel { get; set; }

        /// <summary>
        /// Gets the user level view.
        /// </summary>
        /// <value>The user level view.</value>
        public string UserLevelView
        {
            get { return String.Concat(Enumerable.Repeat(". ", UserLevel)); }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="UserProfileIndexTree" /> class from being created.
        /// </summary>
        private UserProfileIndexTree()
        {
        }

        public static IEnumerable<UserProfileIndexTree> GetUserProfileIndexTree(DefaultContext db, int userId)
        {
            // Nesmí být použity SqlParameters
            UserProfileIndexTree[] userProfiles = UserProfileCache.GetUserProfileIndexTree(db, userId);

            DateTime currentDateTime = DateTime.Now;
            foreach (UserProfileIndexTree userProfileIndexTree in userProfiles)
            {
                userProfileIndexTree.CurrentDateTime = currentDateTime;
            }

            return userProfiles;
        }

        public static IEnumerable<UserProfileIndexTree> GetUserProfileIndexTreeForAdmin(DefaultContext db, int userId)
        {
            // Nesmí být použity SqlParameters
            IEnumerable<UserProfileIndexTree> userProfiles = UserProfileCache.GetUserProfileIndexTreeForAdmin(db, userId);

            DateTime currentDateTime = DateTime.Now;
            foreach (UserProfileIndexTree userProfileIndexTree in userProfiles)
            {
                userProfileIndexTree.CurrentDateTime = currentDateTime;
                userProfileIndexTree.IsEditable = true;
            }

            return userProfiles;
        }
    }

    public class UserProfilePrintIndex
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string City { get; set; }

        public string PhoneNumber1 { get; set; }

        public string Email1 { get; set; }

        public string LyonessId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = BaseModelView.NullDisplayText)]
        public DateTime? PremiumMembershipGranted { get; set; }
    }

    /// <summary>
    /// Class FilteredUser
    /// </summary>
    public class FilteredUser
    {
        /// <summary>
        /// The user id
        /// </summary>
        /// <value>The user id.</value>
        public int UserId { get; set; }

        /// <summary>
        /// The title
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets the filtered users.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>IEnumerable{GetFilteredUser}.</returns>
        public static IEnumerable<FilteredUser> GetFilteredUsers(DefaultContext db, int userId)
        {
            IQueryable<UserProfile> sharedFromUsers = UserProfileCache.GetSharedFromUsers(db, userId);

            // Ponechat Zápis FilteredUser.Title v tomto tvaru - potřebný pro vytvoření dotazu do databáze. Nesmí se použít getter sc.FullName!
            IEnumerable<FilteredUser> filteredUserIds = new[] { new FilteredUser { UserId = userId, Title = ListItemsResource.UserProfile_MineItemTitle } }
                .Union(sharedFromUsers.Select(sc => new FilteredUser { UserId = sc.UserId, Title = sc.LastName + " " + sc.FirstName + " (" + sc.LyonessId + ")" }));
            return filteredUserIds;
        }
    }

    /// <summary>
    /// Class PopulatedUser
    /// </summary>
    public class PopulatedUser
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>The user id.</value>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        private PopulatedUser()
        { }

        public static IEnumerable<PopulatedUser> GetUplineUsersWithoutSharedContacts(DefaultContext db, int userId)
        {
            IEnumerable<PopulatedUser> uplineUsersWithoutSharedContacts = UserProfileCache.GetUplineUsersWithoutSharedContacts(db, userId);
            return uplineUsersWithoutSharedContacts;
        }

        public static IEnumerable<PopulatedUser> GetDownlineUsersWithoutSharedContacts(DefaultContext db, int userId)
        {
            IEnumerable<PopulatedUser> downlineUsersWithoutSharedContacts = UserProfileCache.GetDownlineUsersWithoutSharedContacts(db, userId);
            return downlineUsersWithoutSharedContacts;
        }

        public static IEnumerable<PopulatedUser> GetAnyUsersWithoutSharedContacts(DefaultContext db, int userId)
        {
            IEnumerable<PopulatedUser> anyUsersWithoutSharedContacts = UserProfileCache.GetAnyUsersWithoutSharedContacts(db, userId);
            return anyUsersWithoutSharedContacts;
        }

        public static IEnumerable<PopulatedUser> GetDownlineUsersWithoutTopTenContacts(DefaultContext db, int userId)
        {
            IEnumerable<PopulatedUser> downlineUsersWithoutTopTenContacts = UserProfileCache.GetDownlineUsersWithoutTopTenContacts(db, userId);
            return downlineUsersWithoutTopTenContacts;
        }

        public static IEnumerable<PopulatedUser> GetUplineUsers(DefaultContext db, int userId)
        {
            IEnumerable<UserProfileCache.UserProfileForPopulated> uplineUsers = UserProfileCache.GetUplineUsers(db, userId);
            IEnumerable<PopulatedUser> uplineUsersForPopulate = uplineUsers
                .Select(udu => new PopulatedUser
                {
                    UserId = udu.UserId,
                    Title = String.Format("{0} {1} ({2})", udu.LastName, udu.FirstName, udu.LyonessId)
                }).Reverse().ToArray();
            return uplineUsersForPopulate;
        }

        public static IEnumerable<PopulatedUser> GetDownlineUsers(DefaultContext db, params int[] userIds)
        {
            if (userIds == null || userIds.Length == 0)
                return new PopulatedUser[0];

            var downlineUsers = new List<UserProfileCache.UserProfileForPopulated>();
            foreach (int userId in userIds)
            {
                downlineUsers.AddRange(UserProfileCache.GetDownlineUsers(db, userId));
            }

            IEnumerable<PopulatedUser> downlineUsersForPopulate = downlineUsers
                .Select(udu => new PopulatedUser
                {
                    UserId = udu.UserId,
                    Title = String.Format("{0} {1} ({2})", udu.LastName, udu.FirstName, udu.LyonessId)
                }).DistinctBy(udu => udu.UserId).OrderBy(udu => udu.Title).ToArray();
            return downlineUsersForPopulate;
        }

        public static IEnumerable<PopulatedUser> GetUsers(DefaultContext db)
        {
            IEnumerable<UserProfileCache.UserProfileForPopulated> users = UserProfileCache.GetUsers(db);
            IEnumerable<PopulatedUser> usersForPopulate = users.Select(u => new PopulatedUser
            {
                UserId = u.UserId,
                Title = String.Format("{0} {1} ({2})", u.LastName, u.FirstName, u.LyonessId)
            }).OrderBy(u => u.Title, NameComparer.CzechCaseInsensitive).ToArray();
            return usersForPopulate;
        }
    }

    public abstract class UserProfileEditBase : BaseModelView
    {
        public int UserId { get; set; }

        [Display(Name = "UserProfile_UserName_Name", ResourceType = typeof(FieldResource))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [StringLength(15, MinimumLength = 15, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [RegularExpression(Regex.LyonessId, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexLyonessId_ErrorMessage")]
        [Display(Name = "Global_LyonessId_Name", ResourceType = typeof(FieldResource))]
        public string LyonessId { get; set; }

        [Display(Name = "Global_Role_Name", ResourceType = typeof(FieldResource))]
        public string RoleDisplayName { get { return UserProfile.GetRoleTypeDisplayName(Role); } }

        [Display(Name = "Global_Role_Name", ResourceType = typeof(FieldResource))]
        public RoleType Role { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "User_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [StringLength(40, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_LastName_Name", ResourceType = typeof(FieldResource))]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [StringLength(40, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_FirstName_Name", ResourceType = typeof(FieldResource))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [StringLength(128, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_City_Name", ResourceType = typeof(FieldResource))]
        public string City { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_District_Name", ResourceType = typeof(FieldResource))]
        public int DistrictId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        public int PhoneNumberPrefix1Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [RegularExpression(Regex.OnlyNumberCharacters, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexOnlyNumbers_ErrorMessage")]
        [StringLength(40, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_PhoneNumber1_Name", ResourceType = typeof(FieldResource))]
        public string PhoneNumber1 { get; set; }

        public int? PhoneNumberPrefix2Id { get; set; }

        [RegularExpression(Regex.OnlyNumberCharacters, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexOnlyNumbers_ErrorMessage")]
        [StringLength(40, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_PhoneNumber2_Name", ResourceType = typeof(FieldResource))]
        public string PhoneNumber2 { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [RegularExpression(Regex.Email, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexEmail_ErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_Email1_Name", ResourceType = typeof(FieldResource))]
        public string Email1 { get; set; }

        [RegularExpression(Regex.Email, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexEmail_ErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_Email2_Name", ResourceType = typeof(FieldResource))]
        public string Email2 { get; set; }

        [Display(Name = "Global_Skype_Name", ResourceType = typeof(FieldResource))]
        public string Skype { get; set; }

        [RegularExpression(Regex.Email, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexEmail_ErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "UserProfile_SmsEmail_Name", ResourceType = typeof(FieldResource))]
        public string SmsEmail { get; set; }

        [Display(Name = "UserProfile_UserMail_Name", ResourceType = typeof(FieldResource))]
        public bool UseMail { get; set; }

        [Display(Name = "UserProfile_UseSms_Name", ResourceType = typeof(FieldResource))]
        public bool UseSms { get; set; }

        [Display(Name = "UserProfile_GoogleCredentialsJson_Name", ResourceType = typeof(FieldResource))]
        public string GoogleCredentialsJson { get; set; }

        [Display(Name = "UserProfile_GoogleCalendarId_Name", ResourceType = typeof(FieldResource))]
        public string GoogleCalendarId { get; set; }

        [Display(Name = "UserProfile_UseGoogleCalendar_Name", ResourceType = typeof(FieldResource))]
        public bool UseGoogleCalendar { get; set; }

        [Display(Name = "Global_PremiumMembershipGranted_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? PremiumMembershipGranted { get; set; }

        [Display(Name = "UserProfile_MspCoach_Name", ResourceType = typeof(FieldResource))]
        public bool MspCoach { get; set; }

        [Display(Name = "UserProfile_Ca_Name", ResourceType = typeof(FieldResource))]
        public bool Ca { get; set; }

        [Display(Name = "UserProfile_Presenting_Name", ResourceType = typeof(FieldResource))]
        public bool Presenting { get; set; }

        [AllowHtml]
        [Display(Name = "Global_Tasks_Name", ResourceType = typeof(FieldResource))]
        public string Tasks { get; set; }

        [AllowHtml]
        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        public string Note { get; set; }

        [Display(Name = "UserProfile_ClaAccessAmount_Name", ResourceType = typeof(FieldResource))]
        public string ClaAccessAmountWithCurrency { get; set; }

        [Display(Name = "UserProfile_ClaAccessYearlyAccess_Name", ResourceType = typeof(FieldResource))]
        public string ClaAccessYearlyAccesWithCurrency { get; set; }

        [Display(Name = "UserProfile_ClaAccessExpired_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? ClaAccessExpired { get; set; }

        [Display(Name = "Global_Currency_Name", ResourceType = typeof(FieldResource))]
        public CurrencyType? ClaAccessChangedCurrency { get; set; }

        public decimal ClaAccessAmount { get; set; }

        public CurrencyType ClaAccessCurrency { get; set; }

        public decimal OriginalClaAccessAmount { get; set; }

        public CurrencyType OriginalClaAccessCurrency { get; set; }

        public int ClaAccessYearlyAccess { get; set; }

        [Display(Name = "UserProfile_LCID_Name", ResourceType = typeof(FieldResource))]
        public int LCIDDropDownList { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "UserProfile_AutomaticLogoutInterval_Name", ResourceType = typeof(FieldResource))]
        public AutomaticLogoutIntervalType AutomaticLogoutInterval { get; set; }

        protected UserProfileEditBase()
        {

        }

        protected UserProfileEditBase(UserProfile userProfile)
        {
            Role = userProfile.Role;
            UserId = userProfile.UserId;
            UserName = userProfile.UserName;
            LyonessId = userProfile.LyonessId;
            Title = userProfile.Title;
            LastName = userProfile.LastName;
            FirstName = userProfile.FirstName;
            City = userProfile.City;
            DistrictId = userProfile.DistrictId;
            PhoneNumberPrefix1Id = userProfile.PhoneNumberPrefix1Id;
            PhoneNumber1 = userProfile.PhoneNumber1;
            PhoneNumberPrefix2Id = userProfile.PhoneNumberPrefix2Id;
            PhoneNumber2 = userProfile.PhoneNumber2;
            Email1 = userProfile.Email1;
            Email2 = userProfile.Email2;
            Skype = userProfile.Skype;
            SmsEmail = userProfile.SmsEmail;
            UseMail = userProfile.UseMail;
            UseSms = userProfile.UseSms;
            GoogleCredentialsJson = userProfile.GoogleCredentialsJson;
            GoogleCalendarId = userProfile.GoogleCalendarId;
            UseGoogleCalendar = userProfile.UseGoogleCalendar;
            PremiumMembershipGranted = userProfile.PremiumMembershipGranted;
            MspCoach = userProfile.MspCoach;
            Ca = userProfile.Ca;
            Presenting = userProfile.Presenting;
            Tasks = userProfile.Tasks;
            Note = userProfile.Note;
            LCIDDropDownList = userProfile.LCID;
            AutomaticLogoutInterval = userProfile.AutomaticLogoutInterval;

            ClaAccessAmountWithCurrency = userProfile.ClaAccessAmountWithCurrency;
            ClaAccessYearlyAccesWithCurrency = userProfile.ClaAccessYearlyAccesWithCurrency;
            ClaAccessExpired = userProfile.IsAdmin ? null : userProfile.ClaAccessExpired;
        }

        protected async Task<ModelStateDictionary> ValidateGoogleCredentialsJson(Controller controller, CancellationToken cancellationToken)
        {
            var modelStateDictionary = new ModelStateDictionary();

            var calendar = new Calendar
            {
                GoogleCredentialsJson = GoogleCredentialsJson,
                GoogleCalendarId = GoogleCalendarId,
                UseGoogleCalendarByUser = UseGoogleCalendar
            };
            if (!calendar.UseGoogleCalendar)
                return modelStateDictionary;

            await calendar.AuthorizeAsync(controller, cancellationToken);
            if (calendar.Authorized)
                return modelStateDictionary;

            modelStateDictionary.AddModelError(BaseCache.GoogleCredentialsJsonField, ValidationResource.GoogleAPI_CannotAuthentizedUser_ErrorMessage);
            return modelStateDictionary;
        }

        protected void ValidateChangeClaAccessCurrency(UserProfile userProfile, Currency[] currencies, ref ModelStateDictionary modelStateDictionary)
        {
            ClaAccessAmountWithCurrency = userProfile.ClaAccessAmountWithCurrency;
            ClaAccessYearlyAccesWithCurrency = userProfile.ClaAccessYearlyAccesWithCurrency;
            ClaAccessExpired = userProfile.IsAdmin ? null : userProfile.ClaAccessExpired;
            OriginalClaAccessAmount = userProfile.ClaAccessAmount;
            OriginalClaAccessCurrency = userProfile.ClaAccessCurrency;

            if (ClaAccessChangedCurrency == null || ClaAccessChangedCurrency == userProfile.ClaAccessCurrency)
            {
                ClaAccessAmount = userProfile.ClaAccessAmount;
                ClaAccessYearlyAccess = userProfile.ClaAccessYearlyAccess;
                ClaAccessCurrency = userProfile.ClaAccessCurrency;
                return;
            }

            try
            {
                ClaAccessAmount = CurrencyHelper.ConvertTo(userProfile.ClaAccessAmount, userProfile.ClaAccessCurrency, ClaAccessChangedCurrency.Value, currencies);
                ClaAccessYearlyAccess = userProfile.ClaAccessFixCurrencyChange
                                            ? userProfile.GetClaAccessYearlyAccess(ClaAccessChangedCurrency.Value)
                                            : (int)
                                              Math.Round(CurrencyHelper.ConvertTo(userProfile.ClaAccessYearlyAccess,
                                                                                  userProfile.ClaAccessCurrency,
                                                                                  ClaAccessChangedCurrency.Value,
                                                                                  currencies));
            }
            catch (Exception e)
            {
                modelStateDictionary.AddModelError(BaseCache.ClaAccessChangedCurrencyField, e.Message);
                return;
            }

            ClaAccessCurrency = ClaAccessChangedCurrency.Value;
        }

        protected void ValidateLyonessId(DefaultContext db, ref ModelStateDictionary modelStateDictionary)
        {
            bool isLyonessIdValid = !String.IsNullOrEmpty(LyonessId) && !LyonessId.Equals(UserProfile.DefaultLyonessId, StringComparison.InvariantCultureIgnoreCase);
            if (!isLyonessIdValid)
            {
                string errorMessage = String.Format(ValidationResource.Global_RegexLyonessId_ErrorMessage, FieldResource.Global_LyonessId_Name);
                modelStateDictionary.AddModelError(BaseCache.LyonessIdField, errorMessage);
                return;
            }

            bool isLyonessIdUnique = UserProfileCache.IsLyonessIdUnique(db, UserId, LyonessId);
            if (!isLyonessIdUnique)
            {
                modelStateDictionary.AddModelError(BaseCache.LyonessIdField, ValidationResource.Account_LyonessIdIsNotUnique_ErrorMessage);
            }
        }
    }

    public class UserProfileEdit : UserProfileEditBase
    {
        public UserProfileEdit()
        {

        }

        public UserProfileEdit(UserProfile userProfile)
            : base(userProfile)
        {

        }

        public async Task<ModelStateDictionary> Validate(DefaultContext db, Controller controller, CancellationToken cancellationToken, bool isAdmin)
        {
            var modelStateDictionary = new ModelStateDictionary();

            modelStateDictionary.Merge(await ValidateGoogleCredentialsJson(controller, cancellationToken));

            ValidateLyonessId(db, ref modelStateDictionary);

            UserProfile userProfile = UserProfileCache.GetDetail(db, UserId);
            if (isAdmin)
            {
                Currency[] currencies = CurrencyCache.GetIndex(db);
                ValidateChangeClaAccessCurrency(userProfile, currencies, ref modelStateDictionary);
                ValidateChangeRole(userProfile, ref modelStateDictionary);
            }

            AddAdditionalInformation(isAdmin, userProfile);

            return modelStateDictionary;
        }

        private void ValidateChangeRole(UserProfile userProfile, ref ModelStateDictionary modelStateDictionary)
        {
            if (userProfile.Role == RoleType.AuthGuest && Role != RoleType.AuthGuest)
            {
                modelStateDictionary.AddModelError(BaseCache.RoleField, ValidationResource.Account_CannotChangeRole_ErrorMessage);
            }

            if (userProfile.Role != RoleType.AuthGuest && Role == RoleType.AuthGuest)
            {
                modelStateDictionary.AddModelError(BaseCache.RoleField, ValidationResource.Account_CannotChangeRoleToAuthGuest_ErrorMessage);
            }
        }

        private void AddAdditionalInformation(bool isAdmin, UserProfile userProfile)
        {
            if (!isAdmin)
            {
                Role = userProfile.Role;
            }

            UserName = userProfile.UserName;
        }
    }

    public sealed class UserProfileDetails : BaseModelView
    {
        public int UserId { get; set; }

        [Display(Name = "UserProfile_UserName_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string UserName { get; set; }

        [Display(Name = "Global_FirstName_Name", ResourceType = typeof(FieldResource))]
        public string FirstName { get; set; }

        [Display(Name = "Global_LastName_Name", ResourceType = typeof(FieldResource))]
        public string LastName { get; set; }

        [Display(Name = "Global_BillingInformation_Name", ResourceType = typeof(FieldResource))]
        public string BillingInformation { get; set; }

        [Display(Name = "Global_Role_Name", ResourceType = typeof(FieldResource))]
        public string Role { get; set; }

        public bool IsAdmin { get; set; }

        [Display(Name = "User_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Display(Name = "Global_City_Name", ResourceType = typeof(FieldResource))]
        public string City { get; set; }

        [Display(Name = "Global_District_Name", ResourceType = typeof(FieldResource))]
        public DistrictDetails District { get; set; }

        public PhoneNumberPrefixDetails PhoneNumberPrefix1 { get; set; }

        [Display(Name = "Global_PhoneNumber1_Name", ResourceType = typeof(FieldResource))]
        public string PhoneNumber1 { get; set; }

        public PhoneNumberPrefixDetails PhoneNumberPrefix2 { get; set; }

        [Display(Name = "Global_PhoneNumber2_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string PhoneNumber2 { get; set; }

        [Display(Name = "Global_Email1_Name", ResourceType = typeof(FieldResource))]
        public string Email1 { get; set; }

        [Display(Name = "Global_Email2_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Email2 { get; set; }

        [Display(Name = "UserProfile_GoogleCredentialsJson_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string GoogleCredentialsJson { get; set; }

        [Display(Name = "UserProfile_GoogleCalendarId_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string GoogleCalendarId { get; set; }

        [Display(Name = "Global_PremiumMembershipGranted_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? PremiumMembershipGranted { get; set; }

        [Display(Name = "Global_AccessGranted_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime AccessGranted { get; set; }

        [Display(Name = "Global_RegistredPeopleQuota_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(HtmlEncode = false, NullDisplayText = NullDisplayTextForInteger)]
        public decimal? RegistredPeopleQuota { get; set; }

        [Display(Name = "Global_RegistredPeopleQuotaLastMonth_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(HtmlEncode = false, NullDisplayText = NullDisplayTextForInteger)]
        public decimal? RegistredPeopleQuotaLastMonth { get; set; }

        [Display(Name = "Global_PremiumPartnersQuota_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(HtmlEncode = false, NullDisplayText = NullDisplayTextForInteger)]
        public decimal? PremiumPartnersQuota { get; set; }

        [Display(Name = "Global_PremiumPartnersQuotaLastMonth_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(HtmlEncode = false, NullDisplayText = NullDisplayTextForInteger)]
        public decimal? PremiumPartnersQuotaLastMonth { get; set; }

        [Display(Name = "Global_BuyersQuota_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(HtmlEncode = false, NullDisplayText = NullDisplayTextForInteger)]
        public decimal? BuyersQuota { get; set; }

        [Display(Name = "Global_ContactedPeopleCount_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(HtmlEncode = false, NullDisplayText = NullDisplayTextForInteger)]
        public int? ContactedPeopleCount { get; set; }

        [Display(Name = "Global_ContactedPeopleCountLastMonth_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(HtmlEncode = false, NullDisplayText = NullDisplayTextForInteger)]
        public int? ContactedPeopleCountLastMonth { get; set; }

        [Display(Name = "UserProfile_Ca_Name", ResourceType = typeof(FieldResource))]
        public bool Ca { get; set; }

        [Display(Name = "UserProfile_Presenting_Name", ResourceType = typeof(FieldResource))]
        public bool Presenting { get; set; }

        [Display(Name = "UserProfile_MspCoach_Name", ResourceType = typeof(FieldResource))]
        public bool MspCoach { get; set; }

        [Display(Name = "UserProfile_LastAccessed_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? LastAccessed { get; set; }

        public bool Active { get; set; }

        [Display(Name = "Global_LyonessId_Name", ResourceType = typeof(FieldResource))]
        public string LyonessId { get; set; }

        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Note { get; set; }

        [Display(Name = "Global_Tasks_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Tasks { get; set; }

        [Display(Name = "Global_Skype_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Skype { get; set; }

        [Display(Name = "UserProfile_ClaAccessExpired_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? ClaAccessExpired { get; set; }

        [Display(Name = "UserProfile_ClaAccessAmount_Name", ResourceType = typeof(FieldResource))]
        public string ClaAccessAmountWithCurrency { get; set; }

        [Display(Name = "UserProfile_ClaAccessYearlyAccess_Name", ResourceType = typeof(FieldResource))]
        public string ClaAccessYearlyAccesWithCurrency { get; set; }

        public bool IsPoliciesAccepted { get; set; }

        public UserProfileDetails(UserProfile userProfile)
        {
            UserId = userProfile.UserId;
            UserName = userProfile.UserName;
            FirstName = userProfile.FirstName;
            LastName = userProfile.LastName;
            BillingInformation = SetBillingInformation(userProfile);
            Role = UserProfile.GetRoleTypeDisplayName(userProfile.Role);
            IsAdmin = userProfile.IsAdmin;
            Title = userProfile.Title;
            City = userProfile.City;
            District = DistrictDetails.GetModelView(userProfile.District);
            PhoneNumberPrefix1 = PhoneNumberPrefixDetails.GetModelView(userProfile.PhoneNumberPrefix1);
            PhoneNumber1 = userProfile.PhoneNumber1;
            PhoneNumberPrefix2 = PhoneNumberPrefixDetails.GetModelView(userProfile.PhoneNumberPrefix2);
            PhoneNumber2 = userProfile.PhoneNumber2;
            Email1 = userProfile.Email1;
            Email2 = userProfile.Email2;
            GoogleCredentialsJson = userProfile.GoogleCredentialsJson;
            GoogleCalendarId = userProfile.GoogleCalendarId;
            PremiumMembershipGranted = userProfile.PremiumMembershipGranted;
            AccessGranted = userProfile.AccessGranted;
            RegistredPeopleQuota = userProfile.RegistredPeopleQuota;
            RegistredPeopleQuotaLastMonth = userProfile.RegistredPeopleQuotaLastMonth;
            PremiumPartnersQuota = userProfile.PremiumPartnersQuota;
            PremiumPartnersQuotaLastMonth = userProfile.PremiumPartnersQuotaLastMonth;
            BuyersQuota = userProfile.BuyersQuota;
            ContactedPeopleCount = userProfile.ContactedPeopleCount;
            ContactedPeopleCountLastMonth = userProfile.ContactedPeopleCountLastMonth;
            Ca = userProfile.Ca;
            Presenting = userProfile.Presenting;
            MspCoach = userProfile.MspCoach;
            LastAccessed = userProfile.LastAccessed;
            Active = userProfile.Active;
            LyonessId = userProfile.LyonessId;
            Note = userProfile.Note;
            Tasks = userProfile.Tasks;
            Skype = userProfile.Skype;
            ClaAccessExpired = userProfile.ClaAccessExpired;
            ClaAccessAmountWithCurrency = userProfile.ClaAccessAmountWithCurrency;
            ClaAccessYearlyAccesWithCurrency = userProfile.ClaAccessYearlyAccesWithCurrency;
            IsPoliciesAccepted = userProfile.IsPoliciesAccepted;
        }

        public static UserProfileDetails GetModelView(UserProfile userProfile)
        {
            if (userProfile == null)
                return null;

            var userProfileDetails = new UserProfileDetails(userProfile);
            return userProfileDetails;
        }

        private string SetBillingInformation(UserProfile userProfile)
        {
            var billingInformation = new StringBuilder();
            billingInformation.AppendLine(String.Format("{0} {1}", userProfile.FirstName, userProfile.LastName));
            billingInformation.AppendLine(userProfile.Address);
            billingInformation.AppendLine(String.Format("{0} {1}\n", userProfile.PSC, userProfile.City));
            billingInformation.AppendLine(String.Format("{0}: {1}", FieldResource.Global_ICO_Name, userProfile.ICO));
            billingInformation.AppendLine(String.Format("{0}: {1}", FieldResource.Global_DIC_Name, userProfile.DIC));
            return billingInformation.ToString();
        }
    }

    public class UserProfileDelete : BaseModelView
    {
        [Display(Name = "UserProfile_UserName_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string UserName { get; set; }

        [Display(Name = "Global_FirstName_Name", ResourceType = typeof(FieldResource))]
        public string FirstName { get; set; }

        [Display(Name = "Global_LastName_Name", ResourceType = typeof(FieldResource))]
        public string LastName { get; set; }

        [Display(Name = "Global_LyonessId_Name", ResourceType = typeof(FieldResource))]
        public string LyonessId { get; set; }

        public UserProfileDelete(UserProfile userProfile)
        {
            UserName = userProfile.UserName;
            FirstName = userProfile.FirstName;
            LastName = userProfile.LastName;
            LyonessId = userProfile.LyonessId;
        }

        public static UserProfileDelete GetModelView(UserProfile userProfile)
        {
            if (userProfile == null)
                return null;

            var userProfileDelete = new UserProfileDelete(userProfile);
            return userProfileDelete;
        }
    }

    public class UserProfileLockedUnlocked : BaseModelView
    {
        [Display(Name = "UserProfile_UserName_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string UserName { get; set; }

        [Display(Name = "Global_FirstName_Name", ResourceType = typeof(FieldResource))]
        public string FirstName { get; set; }

        [Display(Name = "Global_LastName_Name", ResourceType = typeof(FieldResource))]
        public string LastName { get; set; }

        public UserProfileLockedUnlocked(UserProfile userProfile)
        {
            UserName = userProfile.UserName;
            FirstName = userProfile.FirstName;
            LastName = userProfile.LastName;
        }

        public static UserProfileLockedUnlocked GetModelView(UserProfile userProfile)
        {
            if (userProfile == null)
                return null;

            var userProfileLocked = new UserProfileLockedUnlocked(userProfile);
            return userProfileLocked;
        }
    }

    public sealed class UserProfilePromote : BaseModelView
    {
        public string ReturnUrl { get; private set; }

        private readonly int _userId;

        private UserProfile _userProfile;

        private UserProfilePromote(BaseController baseController, int id, string returnUrl)
        {
            BaseController = baseController;
            _userId = id;
            ReturnUrl = returnUrl;
        }

        public static UserProfilePromote GetViewModel(BaseController baseController, int id, string returnUrl)
        {
            var allowedReturnUrl = new[] { "Index", "IndexTree" };
            if (baseController == null || id == 0 || !allowedReturnUrl.Contains(returnUrl))
                return null;

            var userProfilePromote = new UserProfilePromote(baseController, id, returnUrl);
            userProfilePromote.Validate();
            userProfilePromote.Process();

            return userProfilePromote;
        }

        protected override bool OnIsValid()
        {
            string statusMessage = GetStatusMessage();
            return String.IsNullOrEmpty(statusMessage);
        }

        private void Validate()
        {
            if (_userId == BaseController.UserId)
            {
                SetStatusMessage(ValidationResource.UserProfile_CannotPromoteYourself_ErrorMessage);
                return;
            }

            _userProfile = UserProfileCache.GetDetail(BaseController.Db, _userId);
            if (_userProfile != null && _userProfile.Role == RoleType.AuthGuest)
                return;

            SetStatusMessage(ValidationResource.UserProfile_CannotPromoteUser_ErrorMessage);
        }

        /// <summary>
        /// Processes this instance.
        /// </summary>
        private void Process()
        {
            if (!IsValid)
                return;

            bool success = UserProfileCache.Promote(BaseController.Db, _userProfile);
            if (!success)
            {
                SetStatusMessage(ValidationResource.Global_DbCommunicationFailed_ErrorMessage);
            }

            SetSuccessMessage(String.Format(ViewResource.UserProfile_UserWasPromoted_Text, _userProfile.FullNameWithoutLyonessId));
        }
    }
}