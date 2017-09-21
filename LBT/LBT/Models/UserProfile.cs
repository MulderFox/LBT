// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-26-2014
//
// Last Modified By : zmikeska
// Last Modified On : 04-23-2014
// ***********************************************************************
// <copyright file="AccountModels.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.Helpers;
using LBT.ModelViews;
using LBT.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Security;

namespace LBT.Models
{
    public enum RoleType
    {
        /// <summary>
        /// Admin + leader
        /// </summary>
        AdminLeader = 1,

        /// <summary>
        /// Admin
        /// </summary>
        Admin = 4,

        /// <summary>
        /// Leader
        /// </summary>
        Leader = 2,

        AuthGuest = 5,

        Unknown = 0
    }

    public enum ClaAccessNotification
    {
        None = 0,
        Monthly,
        Fortnightly,
        ThreeDays,
        Current,
        Blocked
    }

    public enum AutomaticLogoutIntervalType
    {
        TenMinutes = 10,
        TwentyMinutes = 20,
        ThirtyMinutes = 30,
        SixtyMinutes = 60
    }

    public class UserProfile
    {
        public const string RoleLeader = "Lídr";
        public const string RoleAdminLeader = "AdminLídr";
        public const string RoleAdmin = "Admin";
        public const string RoleAuthGuest = "AuthGuest";

        public const string DefaultLyonessId = "000.000.000.000";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(40)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(40)]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName
        {
            get { return String.Format("{0} {1} ({2})", LastName, FirstName, LyonessId); }
        }

        [NotMapped]
        public string FullNameWithoutLyonessId
        {
            get { return String.Format("{0} {1}", LastName, FirstName); }
        }

        [NotMapped]
        public RoleType Role { get; set; }

        [NotMapped]
        public bool IsAdmin { get { return Role == RoleType.Admin || Role == RoleType.AdminLeader; } }

        [StringLength(20)]
        public string Title { get; set; }

        [Required]
        [StringLength(128)]
        public string City { get; set; }

        [NotMapped]
        public string CityIndexView { get { return Grammar.CutTextWithDots(City, 18); } }

        [ForeignKey("District")]
        [Required]
        public int DistrictId { get; set; }

        [Column("DistrictId")]
        public virtual District District { get; set; }

        [ForeignKey("PhoneNumberPrefix1")]
        [Required]
        public int PhoneNumberPrefix1Id { get; set; }

        [Column("PhoneNumberPrefix1Id")]
        public virtual PhoneNumberPrefix PhoneNumberPrefix1 { get; set; }

        [Required]
        [StringLength(40)]
        public string PhoneNumber1 { get; set; }

        [ForeignKey("PhoneNumberPrefix2")]
        public int? PhoneNumberPrefix2Id { get; set; }

        [Column("PhoneNumberPrefix2Id")]
        public virtual PhoneNumberPrefix PhoneNumberPrefix2 { get; set; }

        [StringLength(40)]
        public string PhoneNumber2 { get; set; }

        [Required]
        [StringLength(100)]
        public string Email1 { get; set; }

        [StringLength(100)]
        public string Email2 { get; set; }

        public string GoogleCredentialsJson { get; set; }

        public string GoogleCalendarId { get; set; }

        public DateTime? PremiumMembershipGranted { get; set; }

        public DateTime AccessGranted { get; set; }

        public decimal? RegistredPeopleQuota { get; set; }

        public decimal? RegistredPeopleQuotaLastMonth { get; set; }

        public decimal? PremiumPartnersQuota { get; set; }

        public decimal? PremiumPartnersQuotaLastMonth { get; set; }

        public decimal? BuyersQuota { get; set; }

        public int? ContactedPeopleCount { get; set; }

        public int? ContactedPeopleCountLastMonth { get; set; }

        public bool Ca { get; set; }

        public bool Presenting { get; set; }

        public bool MspCoach { get; set; }

        public DateTime? LastAccessed { get; set; }

        [ForeignKey("Registrar")]
        public int? RegistrarId { get; set; }

        [Column("RegistrarId")]
        public virtual UserProfile Registrar { get; set; }

        public bool Active { get; set; }

        [Required]
        [StringLength(15)]
        public string LyonessId { get; set; }

        public string Note { get; set; }

        public string Tasks { get; set; }

        public string Skype { get; set; }

        [StringLength(100)]
        public string SmsEmail { get; set; }

        public bool UseMail { get; set; }

        public bool UseSms { get; set; }

        public bool UseGoogleCalendar { get; set; }

        public string ReminderTime { get; set; }

        public bool IsEventsPrivate { get; set; }

        public DateTime? ClaAccessExpired { get; set; }

        public ClaAccessNotification ClaAccessNotification { get; set; }

        public decimal ClaAccessAmount { get; set; }

        [NotMapped]
        public string ClaAccessAmountWithCurrency
        {
            get
            {
                return String.Format("{0} {1}", ClaAccessAmount, ClaAccessCurrency);
            }
        }

        public int ClaAccessYearlyAccessCZK { get; set; }

        public int ClaAccessYearlyAccessEUR { get; set; }

        public int ClaAccessYearlyAccessUSD { get; set; }

        [NotMapped]
        public int ClaAccessYearlyAccess
        {
            get
            {
                switch (ClaAccessCurrency)
                {
                    default:
                        return ClaAccessYearlyAccessCZK;

                    case CurrencyType.EUR:
                        return ClaAccessYearlyAccessEUR;

                    case CurrencyType.USD:
                        return ClaAccessYearlyAccessUSD;
                }
            }
        }

        [NotMapped]
        public string ClaAccessYearlyAccesWithCurrency
        {
            get { return String.Format("{0} {1}", ClaAccessYearlyAccess, ClaAccessCurrency); }
        }

        public CurrencyType ClaAccessCurrency { get; set; }

        [NotMapped]
        public CurrencyType ClaAccessChangedCurrency { get; set; }

        public bool ClaAccessTrial { get; set; }

        public bool ClaAccessFixCurrencyChange { get; set; }

        public bool IsPoliciesAccepted { get; set; }

        public int LCID { get; set; }

        public AutomaticLogoutIntervalType AutomaticLogoutInterval { get; set; }

        public string Address { get; set; }

        public string PSC { get; set; }

        public string DIC { get; set; }

        public string ICO { get; set; }

        public static string GetRoleTypeDisplayName(RoleType roleType)
        {
            switch (roleType)
            {
                case RoleType.AdminLeader:
                    return ListItemsResource.RoleType_AdminLeader;

                case RoleType.Admin:
                    return ListItemsResource.RoleType_Admin;

                case RoleType.Leader:
                    return ListItemsResource.RoleType_Leader;

                case RoleType.AuthGuest:
                    return ListItemsResource.RoleType_AuthGuest;

                case RoleType.Unknown:
                    return ListItemsResource.RoleType_Unknown;

                default:
                    throw new ArgumentOutOfRangeException("roleType");
            }
        }

        public static string GetAutomaticLogoutIntervalTypeDisplayName(AutomaticLogoutIntervalType automaticLogoutIntervalType)
        {
            switch (automaticLogoutIntervalType)
            {
                case AutomaticLogoutIntervalType.TenMinutes:
                    return ListItemsResource.AutomaticLogoutIntervalType_TenMinutes;

                case AutomaticLogoutIntervalType.TwentyMinutes:
                    return ListItemsResource.AutomaticLogoutIntervalType_TwentyMinutes;

                case AutomaticLogoutIntervalType.ThirtyMinutes:
                    return ListItemsResource.AutomaticLogoutIntervalType_ThirtyMinutes;

                case AutomaticLogoutIntervalType.SixtyMinutes:
                    return ListItemsResource.AutomaticLogoutIntervalType_SixtyMinutes;

                default:
                    throw new ArgumentOutOfRangeException("automaticLogoutIntervalType");
            }
        }

        public static string GetRoleTypeDbName(RoleType roleType)
        {
            switch (roleType)
            {
                case RoleType.AdminLeader:
                    return RoleAdminLeader;

                case RoleType.Admin:
                    return RoleAdmin;

                case RoleType.Leader:
                    return RoleLeader;

                case RoleType.AuthGuest:
                    return RoleAuthGuest;

                default:
                    throw new ArgumentOutOfRangeException("roleType");
            }
        }

        public static RoleType GetRoleType(string roleTypeDbName)
        {
            switch (roleTypeDbName)
            {
                case RoleAdminLeader:
                    return RoleType.AdminLeader;

                case RoleAdmin:
                    return RoleType.Admin;

                case RoleLeader:
                    return RoleType.Leader;

                case RoleAuthGuest:
                    return RoleType.AuthGuest;

                default:
                    throw new ArgumentOutOfRangeException("roleTypeDbName");
            }
        }

        /// <summary>
        /// Gets the role for user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>UserProfile.RoleType.</returns>
        /// <exception cref="System.Exception">User cannot have more than 1 role.</exception>
        public static RoleType GetRoleForUser(string userName)
        {
            string[] roles = Roles.GetRolesForUser(userName);
            if (roles.Length > 1)
                throw new Exception("User cannot have more than 1 role.");

            return roles.Length > 0 ? GetRoleType(roles[0]) : 0;
        }

        public static Dictionary<RoleType, string> GetTranslationDictionaryForRoleType()
        {
            var translationDictionary = new Dictionary<RoleType, string>();
            translationDictionary[RoleType.Leader] = GetRoleTypeDisplayName(RoleType.Leader);
            translationDictionary[RoleType.AdminLeader] = GetRoleTypeDisplayName(RoleType.AdminLeader);
            translationDictionary[RoleType.Admin] = GetRoleTypeDisplayName(RoleType.Admin);
            translationDictionary[RoleType.AuthGuest] = GetRoleTypeDisplayName(RoleType.AuthGuest);
            return translationDictionary;
        }

        public static Dictionary<AutomaticLogoutIntervalType, string> GetTranslationDictionaryForAutomaticLogoutIntervalType()
        {
            var translationDictionary = new Dictionary<AutomaticLogoutIntervalType, string>();
            translationDictionary[AutomaticLogoutIntervalType.TenMinutes] = GetAutomaticLogoutIntervalTypeDisplayName(AutomaticLogoutIntervalType.TenMinutes);
            translationDictionary[AutomaticLogoutIntervalType.TwentyMinutes] = GetAutomaticLogoutIntervalTypeDisplayName(AutomaticLogoutIntervalType.TwentyMinutes);
            translationDictionary[AutomaticLogoutIntervalType.ThirtyMinutes] = GetAutomaticLogoutIntervalTypeDisplayName(AutomaticLogoutIntervalType.ThirtyMinutes);
            translationDictionary[AutomaticLogoutIntervalType.SixtyMinutes] = GetAutomaticLogoutIntervalTypeDisplayName(AutomaticLogoutIntervalType.SixtyMinutes);
            return translationDictionary;
        }

        public static DateTime GetCheckDbDateForClaAccessNotification(ClaAccessNotification claAccessNotification, DateTime currentDateTime)
        {
            switch (claAccessNotification)
            {
                case ClaAccessNotification.Monthly:
                    return currentDateTime.AddMonths(1);

                case ClaAccessNotification.Fortnightly:
                    return currentDateTime.AddDays(14);

                case ClaAccessNotification.ThreeDays:
                    return currentDateTime.AddDays(3);

                case ClaAccessNotification.Current:
                    return currentDateTime.AddDays(0);

                default:
                    throw new ArgumentOutOfRangeException("claAccessNotification");
            }
        }

        public int GetClaAccessYearlyAccess(CurrencyType currencyType)
        {
            switch (currencyType)
            {
                case CurrencyType.CZK:
                    return ClaAccessYearlyAccessCZK;

                case CurrencyType.EUR:
                    return ClaAccessYearlyAccessEUR;

                case CurrencyType.USD:
                    return ClaAccessYearlyAccessUSD;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetClaAccessYearlyAccess(CurrencyType currencyType, int amount)
        {
            switch (currencyType)
            {
                case CurrencyType.CZK:
                    ClaAccessYearlyAccessCZK = amount;
                    break;

                case CurrencyType.EUR:
                    ClaAccessYearlyAccessEUR = amount;
                    break;

                case CurrencyType.USD:
                    ClaAccessYearlyAccessUSD = amount;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void CopyFrom(UserProfileEditBase userProfileEditBase)
        {
            FirstName = userProfileEditBase.FirstName;
            LastName = userProfileEditBase.LastName;
            Title = userProfileEditBase.Title;
            City = userProfileEditBase.City;
            DistrictId = userProfileEditBase.DistrictId;
            PhoneNumberPrefix1Id = userProfileEditBase.PhoneNumberPrefix1Id;
            PhoneNumber1 = userProfileEditBase.PhoneNumber1;
            PhoneNumberPrefix2Id = userProfileEditBase.PhoneNumberPrefix2Id;
            PhoneNumber2 = userProfileEditBase.PhoneNumber2;
            Email1 = userProfileEditBase.Email1;
            Email2 = userProfileEditBase.Email2;
            GoogleCredentialsJson = userProfileEditBase.GoogleCredentialsJson;
            GoogleCalendarId = userProfileEditBase.GoogleCalendarId;
            LyonessId = userProfileEditBase.LyonessId;
            Note = userProfileEditBase.Note;
            Tasks = userProfileEditBase.Tasks;
            Skype = userProfileEditBase.Skype;
            SmsEmail = userProfileEditBase.SmsEmail;
            UseMail = userProfileEditBase.UseMail;
            UseSms = userProfileEditBase.UseSms;
            UseGoogleCalendar = userProfileEditBase.UseGoogleCalendar;
        }

        public void CopyFrom(UserProfileEdit userProfileEdit, bool isAdmin)
        {
            CopyFrom(userProfileEdit);

            PremiumMembershipGranted = userProfileEdit.PremiumMembershipGranted;
            Ca = userProfileEdit.Ca;
            MspCoach = userProfileEdit.MspCoach;
            Presenting = userProfileEdit.Presenting;

            if (!isAdmin)
                return;

            ClaAccessAmount = userProfileEdit.ClaAccessAmount;
            ClaAccessCurrency = userProfileEdit.ClaAccessCurrency;
            SetClaAccessYearlyAccess(userProfileEdit.ClaAccessCurrency, userProfileEdit.ClaAccessYearlyAccess);
        }

        public void CopyFrom(SettingsProfile settingsProfile)
        {
            CopyFrom((UserProfileEditBase)settingsProfile);

            Address = settingsProfile.Address;
            PSC = settingsProfile.PSC;
            DIC = settingsProfile.DIC;
            ICO = settingsProfile.ICO;
            ReminderTime = settingsProfile.ReminderTime;
            IsEventsPrivate = settingsProfile.IsEventsPrivate;
            ClaAccessAmount = settingsProfile.ClaAccessAmount;
            ClaAccessCurrency = settingsProfile.ClaAccessCurrency;
            SetClaAccessYearlyAccess(settingsProfile.ClaAccessCurrency, settingsProfile.ClaAccessYearlyAccess);
            LCID = settingsProfile.LCIDDropDownList;
            AutomaticLogoutInterval = settingsProfile.AutomaticLogoutInterval;
        }

        public void CopyFrom(CheckBillingInformation checkBillingInformation)
        {
            FirstName = checkBillingInformation.FirstName;
            LastName = checkBillingInformation.LastName;
            Address = checkBillingInformation.Address;
            City = checkBillingInformation.City;
            PSC = checkBillingInformation.PSC;
            DIC = checkBillingInformation.DIC;
            ICO = checkBillingInformation.ICO;
        }
    }

    public class UserProfileWithRole
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool UseMail { get; set; }
        public string Email1 { get; set; }
        public string LyonessId { get; set; }
        public DateTime? ClaAccessExpired { get; set; }
        public ClaAccessNotification ClaAccessNotification { get; set; }
        public decimal ClaAccessAmount { get; set; }
        public int ClaAccessYearlyAccessCZK { get; set; }
        public int ClaAccessYearlyAccessEUR { get; set; }
        public int ClaAccessYearlyAccessUSD { get; set; }
        public CurrencyType ClaAccessCurrency { get; set; }
        public bool ClaAccessTrial { get; set; }

        [NotMapped]
        public int ClaAccessYearlyAccess
        {
            get
            {
                switch (ClaAccessCurrency)
                {
                    default:
                        return ClaAccessYearlyAccessCZK;

                    case CurrencyType.EUR:
                        return ClaAccessYearlyAccessEUR;

                    case CurrencyType.USD:
                        return ClaAccessYearlyAccessUSD;
                }
            }
        }

        public RoleType UserProfileRole { get; set; }

        public bool IsAdmin { get { return UserProfileRole == RoleType.Admin || UserProfileRole == RoleType.AdminLeader; } }
    }
}