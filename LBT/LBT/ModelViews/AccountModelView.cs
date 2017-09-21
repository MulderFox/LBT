using LBT.Cache;
using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using LBT.Properties;
using LBT.Resources;
using LBT.Services.GoogleApis;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebMatrix.WebData;
using Regex = LBT.Helpers.Regex;

namespace LBT.ModelViews
{
    /// <summary>
    /// Class Login
    /// </summary>
    public class Login : BaseModelView
    {
        //[Display(Name = "UserProfile_LCID_Name", ResourceType = typeof(FieldResource))]
        [Display(Name = "UserProfile_LCID_Name", ResourceType = typeof(FieldResource))]
        public int LCID { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "UserProfile_UserName_Name", ResourceType = typeof(FieldResource))]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [DataType(DataType.Password)]
        [Display(Name = "Account_Password_Name", ResourceType = typeof(FieldResource))]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [remember me].
        /// </summary>
        /// <value><c>true</c> if [remember me]; otherwise, <c>false</c>.</value>
        [Display(Name = "Account_RememberMe_Name", ResourceType = typeof(FieldResource))]
        public bool RememberMe { get; set; }

        public int UserId { get; set; }

        public ModelStateDictionary Validate(DefaultContext db)
        {
            var modelStateDictionary = new ModelStateDictionary();

            ValidateUserExists(ref modelStateDictionary);
            ValidateUnlockedOutAccount(ref modelStateDictionary);

            UserProfile userProfile = UserProfileCache.GetDetail(db, UserName);
            if (userProfile == null)
            {
                return modelStateDictionary;
            }

            ValidateActiveAccount(ref modelStateDictionary, userProfile);
            ValidateClaAccess(ref modelStateDictionary, db, userProfile);
            ProcessLogin(ref modelStateDictionary, userProfile);

            return modelStateDictionary;
        }

        private void ValidateUserExists(ref ModelStateDictionary modelStateDictionary)
        {
            // Kontrola na existujícího uživatele
            if (WebSecurity.UserExists(UserName))
                return;

            modelStateDictionary.AddModelError(String.Empty, ValidationResource.MembershipCreateStatus_InvalidUserName_ErrorMessage);
        }

        private void ValidateUnlockedOutAccount(ref ModelStateDictionary modelStateDictionary)
        {
            // Kontrola na zamknutí účtu kvůli špatným pokusům o přístup.
            if (!WebSecurity.IsAccountLockedOut(UserName, Settings.Default.AllowedPasswordAttempts - 2, 1800))
                return;

            modelStateDictionary.AddModelError(String.Empty, ValidationResource.Account_TooManyBadPasswords_ErrorMessage);
        }

        private void ValidateActiveAccount(ref ModelStateDictionary modelStateDictionary, UserProfile userProfile)
        {
            // Kontrola na zamknutí účtu kvůli neaktivnímu účtu.
            if (userProfile.Active)
                return;

            modelStateDictionary.AddModelError(String.Empty, ValidationResource.Account_AccountIsLocked_ErrorMessage);
        }

        private void ValidateClaAccess(ref ModelStateDictionary modelStateDictionary, DefaultContext db, UserProfile userProfile)
        {
            // Kontrola na platnost přístup s platbou za přístup
            if (userProfile.IsAdmin ||
                userProfile.ClaAccessExpired >= DateTime.Now)
                return;

            decimal amountRemains = userProfile.ClaAccessYearlyAccess - userProfile.ClaAccessAmount;
            string paymentInformation = BankAccountCache.GetPaymentInfo(db, userProfile.ClaAccessCurrency, userProfile.LyonessId, amountRemains);
            string errorMessage = String.Format(ValidationResource.Account_AccountAccessIsExpired_ErrorMessage, paymentInformation);

            modelStateDictionary.AddModelError(String.Empty, errorMessage);
        }

        private void ProcessLogin(ref ModelStateDictionary modelStateDictionary, UserProfile userProfile)
        {
            if (WebSecurity.IsAuthenticated)
            {
                WebSecurity.Logout();
            }

            if (modelStateDictionary.Count > 0)
                return;

            if (WebSecurity.Login(UserName, Password, RememberMe))
            {
                UserId = userProfile.UserId;
                return;
            }

            if (modelStateDictionary.Count > 0)
                return;

            int passwordFailuresSinceLastSuccess = Settings.Default.AllowedPasswordAttempts - WebSecurity.GetPasswordFailuresSinceLastSuccess(UserName);
            modelStateDictionary.AddModelError(String.Empty, String.Format(ValidationResource.Account_LoginFailed_ErrorMessage, passwordFailuresSinceLastSuccess));
        }
    }

    /// <summary>
    /// Class LocalPassword
    /// </summary>
    public class LocalPassword : BaseModelView
    {
        /// <summary>
        /// Gets or sets the old password.
        /// </summary>
        /// <value>The old password.</value>
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [StringLength(100, MinimumLength = 6, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLengthMinMax2_ErrorMessage")]
        [DataType(DataType.Password)]
        [Display(Name = "Account_OldPassword_Name", ResourceType = typeof(FieldResource))]
        public string OldPassword { get; set; }

        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        /// <value>The new password.</value>
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [StringLength(100, MinimumLength = 6, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLengthMinMax2_ErrorMessage")]
        [RegularExpression(Regex.PasswordCharacters, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Account_RegexPasswordCharacters_ErrorMessage")]
        [DataType(DataType.Password)]
        [Display(Name = "Account_NewPassword_Name", ResourceType = typeof(FieldResource))]
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets the confirm password.
        /// </summary>
        /// <value>The confirm password.</value>
        [DataType(DataType.Password)]
        [Display(Name = "Account_ConfirmPassword_Name", ResourceType = typeof(FieldResource))]
        [System.ComponentModel.DataAnnotations.Compare(BaseCache.NewPasswordField, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Account_ConfirmPassword_ErrorMessage")]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// Class RecoveryPassword
    /// </summary>
    public class RecoveryPassword : BaseModelView
    {
        [Display(Name = "UserProfile_LCID_Name", ResourceType = typeof(FieldResource))]
        public int LCID { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "UserProfile_UserName_Name", ResourceType = typeof(FieldResource))]
        public string UserName { get; set; }

        public ModelStateDictionary Validate(DefaultContext db)
        {
            var modelStateDictionary = new ModelStateDictionary();

            ValidateExistUser(ref modelStateDictionary);
            if (modelStateDictionary.Count > 0)
                return modelStateDictionary;

            UserProfile userProfile = UserProfileCache.GetDetail(db, UserName);
            ValidateActiveUser(userProfile, ref modelStateDictionary);

            string newPassword = Cryptography.GetRandomPassword();
            ResetPassword(userProfile, newPassword, ref modelStateDictionary);
            SendEmail(userProfile, newPassword, ref modelStateDictionary);

            return modelStateDictionary;
        }

        private void ValidateExistUser(ref ModelStateDictionary modelStateDictionary)
        {
            if (WebSecurity.UserExists(UserName))
                return;

            modelStateDictionary.AddModelError(BaseCache.UserNameField, ValidationResource.Account_UserDoesNotExist_ErrorMessage);
        }

        private void ValidateActiveUser(UserProfile userProfile, ref ModelStateDictionary modelStateDictionary)
        {
            if (userProfile != null && userProfile.Active)
                return;

            modelStateDictionary.AddModelError(BaseCache.UserNameField, ValidationResource.Account_LockedAccount_ErrorMessage);
        }

        private void ResetPassword(UserProfile userProfile, string newPassword, ref ModelStateDictionary modelStateDictionary)
        {
            if (userProfile == null)
                return;

            string passwordResetToken = WebSecurity.GeneratePasswordResetToken(userProfile.UserName);
            if (WebSecurity.ResetPassword(passwordResetToken, newPassword))
                return;

            modelStateDictionary.AddModelError(BaseCache.UserNameField, ValidationResource.Account_PasswordRecoveryFailed_ErrorMessage);
        }

        private void SendEmail(UserProfile userProfile, string newPassword, ref ModelStateDictionary modelStateDictionary)
        {
            if (userProfile == null)
                return;

            string textBody = String.Format(MailResource.AccountController_PasswordRecovery_TextBody, newPassword);
            if (Mail.SendEmail(userProfile.Email1, MailResource.AccountController_PasswordRecovery_Subject, textBody, true, true))
                return;

            modelStateDictionary.AddModelError(BaseCache.UserNameField, ValidationResource.Account_CannotSendEmailForPasswordRecovery_ErrorMessage);
        }
    }

    /// <summary>
    /// Class Register
    /// </summary>
    public class Register : BaseModelView
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "UserProfile_UserName_Name", ResourceType = typeof(FieldResource))]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [StringLength(40, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_FirstName_Name", ResourceType = typeof(FieldResource))]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [StringLength(40, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_LastName_Name", ResourceType = typeof(FieldResource))]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Role_Name", ResourceType = typeof(FieldResource))]
        public RoleType Role { get; set; }

        /// <summary>
        /// Gets or sets the title1.
        /// </summary>
        /// <value>The title1.</value>
        [StringLength(20, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "User_Title_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>The city.</value>
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [StringLength(128, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_City_Name", ResourceType = typeof(FieldResource))]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the district id.
        /// </summary>
        /// <value>The district id.</value>
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_District_Name", ResourceType = typeof(FieldResource))]
        public int DistrictId { get; set; }

        /// <summary>
        /// Gets or sets the phone number prefix1 id.
        /// </summary>
        /// <value>The phone number prefix1 id.</value>
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        public int PhoneNumberPrefix1Id { get; set; }

        /// <summary>
        /// Gets or sets the phone number1.
        /// </summary>
        /// <value>The phone number1.</value>
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [RegularExpression(Regex.OnlyNumberCharacters, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexOnlyNumbers_ErrorMessage")]
        [StringLength(40, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_PhoneNumber1_Name", ResourceType = typeof(FieldResource))]
        public string PhoneNumber1 { get; set; }

        /// <summary>
        /// Gets or sets the phone number prefix2 id.
        /// </summary>
        /// <value>The phone number prefix2 id.</value>
        [ForeignKey("PhoneNumberPrefix2")]
        public int? PhoneNumberPrefix2Id { get; set; }

        /// <summary>
        /// Gets or sets the phone number2.
        /// </summary>
        /// <value>The phone number2.</value>
        [RegularExpression(Regex.OnlyNumberCharacters, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexOnlyNumbers_ErrorMessage")]
        [StringLength(40, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_PhoneNumber2_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string PhoneNumber2 { get; set; }

        /// <summary>
        /// Gets or sets the email1.
        /// </summary>
        /// <value>The email1.</value>
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [RegularExpression(Regex.Email, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexEmail_ErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_Email1_Name", ResourceType = typeof(FieldResource))]
        public string Email1 { get; set; }

        /// <summary>
        /// Gets or sets the email2.
        /// </summary>
        /// <value>The email2.</value>
        [RegularExpression(Regex.Email, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexEmail_ErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_Email2_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Email2 { get; set; }

        /// <summary>
        /// Gets or sets the google credentials JSON.
        /// </summary>
        /// <value>The google calendar URL.</value>
        [Display(Name = "UserProfile_GoogleCredentialsJson_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string GoogleCredentialsJson { get; set; }

        /// <summary>
        /// Gets or sets the google calendar id.
        /// </summary>
        /// <value>The google calendar id.</value>
        [Display(Name = "UserProfile_GoogleCalendarId_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string GoogleCalendarId { get; set; }

        /// <summary>
        /// Gets or sets the premium membership granted.
        /// </summary>
        /// <value>The premium membership granted.</value>
        [Display(Name = "Global_PremiumMembershipGranted_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? PremiumMembershipGranted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Register" /> is ca.
        /// </summary>
        /// <value><c>true</c> if ca; otherwise, <c>false</c>.</value>
        [Display(Name = "UserProfile_Ca_Name", ResourceType = typeof(FieldResource))]
        public bool Ca { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Register" /> is presenting.
        /// </summary>
        /// <value><c>true</c> if presenting; otherwise, <c>false</c>.</value>
        [Display(Name = "UserProfile_Presenting_Name", ResourceType = typeof(FieldResource))]
        public bool Presenting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Register" /> is MPS.
        /// </summary>
        /// <value><c>true</c> if MPS; otherwise, <c>false</c>.</value>
        [Display(Name = "UserProfile_MspCoach_Name", ResourceType = typeof(FieldResource))]
        public bool MspCoach { get; set; }

        /// <summary>
        /// Gets or sets the lyoness id.
        /// </summary>
        /// <value>The lyoness id.</value>
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [StringLength(15, MinimumLength = 15, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [RegularExpression(Regex.LyonessId, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexLyonessId_ErrorMessage")]
        [Display(Name = "Global_LyonessId_Name", ResourceType = typeof(FieldResource))]
        public string LyonessId { get; set; }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        /// <value>The note.</value>
        [AllowHtml]
        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the tasks.
        /// </summary>
        /// <value>The tasks.</value>
        [AllowHtml]
        [Display(Name = "Global_Tasks_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Tasks { get; set; }

        /// <summary>
        /// Gets or sets the skype.
        /// </summary>
        /// <value>The skype.</value>
        [Display(Name = "Global_Skype_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Skype { get; set; }

        /// <summary>
        /// Gets or sets the SMS email.
        /// </summary>
        /// <value>The SMS email.</value>
        [RegularExpression(Regex.Email, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexEmail_ErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "UserProfile_SmsEmail_Name", ResourceType = typeof(FieldResource))]
        public string SmsEmail { get; set; }

        public int RegistrarId { get; set; }

        public DateTime AccessGranted { get; set; }

        public DateTime ClaAccessExpired { get; set; }

        public int ClaAccessYearlyAccessCZK { get; set; }

        public int ClaAccessYearlyAccessEUR { get; set; }

        public int ClaAccessYearlyAccessUSD { get; set; }

        public bool ClaAccessTrial { get; set; }

        public bool ClaAccessFixCurrencyChange { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Currency_Name", ResourceType = typeof(FieldResource))]
        public CurrencyType CurrencyType { get; set; }

        public int LCID { get; set; }

        public void CopyFrom(PeopleContact peopleContact)
        {
            Title = peopleContact.Title;
            LastName = peopleContact.LastName;
            FirstName = peopleContact.FirstName;
            City = peopleContact.City;
            PhoneNumber1 = peopleContact.PhoneNumber1;
            PhoneNumber2 = peopleContact.PhoneNumber2;
            Email1 = peopleContact.Email1;
            Email2 = peopleContact.Email2;
            Skype = peopleContact.Skype;
            PremiumMembershipGranted = peopleContact.PremiumMembershipGranted;
            LyonessId = peopleContact.LyonessId;
            Tasks = peopleContact.Tasks;
            Note = peopleContact.Note;
        }

        public async Task<ModelStateDictionary> Validate(Controller controller, CancellationToken cancellationToken, DefaultContext db, int userId)
        {
            var modelStateDictionary = new ModelStateDictionary();

            ModelStateDictionary modelState = await ValidateGoogleCredentialsJson(controller, cancellationToken);
            modelStateDictionary.Merge(modelState);

            ValidateUniquePhone1(db, ref modelStateDictionary);
            ValidateLyonessId(db, ref modelStateDictionary);
            UserProfile registrar = UserProfileCache.GetDetail(db, userId);
            SetDefaultData(registrar);

            return modelStateDictionary;
        }

        private async Task<ModelStateDictionary> ValidateGoogleCredentialsJson(Controller controller, CancellationToken cancellationToken)
        {
            var modelStateDictionary = new ModelStateDictionary();

            if (String.IsNullOrEmpty(GoogleCredentialsJson))
                return modelStateDictionary;

            var calendar = new Calendar
                               {
                                   GoogleCredentialsJson = GoogleCredentialsJson,
                                   GoogleCalendarId = GoogleCalendarId,
                                   UseGoogleCalendarByUser = true
                               };
            await calendar.AuthorizeAsync(controller, cancellationToken);
            if (!calendar.UseGoogleCalendar || calendar.Authorized)
                return modelStateDictionary;

            modelStateDictionary.AddModelError(BaseCache.GoogleCredentialsJsonField, ValidationResource.GoogleAPI_CannotAuthentizedUser_ErrorMessage);
            return modelStateDictionary;
        }

        private void ValidateUniquePhone1(DefaultContext db, ref ModelStateDictionary modelStateDictionary)
        {
            bool isPhone1Unique = UserProfileCache.IsPhone1Unique(db, PhoneNumberPrefix1Id, PhoneNumber1);
            if (isPhone1Unique)
                return;

            modelStateDictionary.AddModelError(BaseCache.PhoneNumberPrefix1IdField, ValidationResource.Account_Phone1IsNotUnique_ErrorMessage);
        }

        private void ValidateLyonessId(DefaultContext db, ref ModelStateDictionary modelStateDictionary)
        {
            bool isLyonessIdValid = !String.IsNullOrEmpty(LyonessId) && !LyonessId.Equals(UserProfile.DefaultLyonessId, StringComparison.InvariantCultureIgnoreCase);
            if (!isLyonessIdValid)
            {
                string errorMessage = String.Format(ValidationResource.Global_RegexLyonessId_ErrorMessage, FieldResource.Global_LyonessId_Name);
                modelStateDictionary.AddModelError(BaseCache.LyonessIdField, errorMessage);
                return;
            }

            bool isLyonessIdUnique = UserProfileCache.IsLyonessIdUnique(db, LyonessId);
            if (!isLyonessIdUnique)
            {
                modelStateDictionary.AddModelError(BaseCache.LyonessIdField, ValidationResource.Account_LyonessIdIsNotUnique_ErrorMessage);
            }
        }

        private void SetDefaultData(UserProfile registrar)
        {
            DateTime currentDateTime = DateTime.Now;
            AccessGranted = currentDateTime;
            RegistrarId = registrar.UserId;
            ClaAccessExpired = DateTime.MaxValue;
            ClaAccessYearlyAccessCZK = registrar.ClaAccessYearlyAccessCZK;
            ClaAccessYearlyAccessEUR = registrar.ClaAccessYearlyAccessEUR;
            ClaAccessYearlyAccessUSD = registrar.ClaAccessYearlyAccessUSD;
            ClaAccessTrial = false;
            ClaAccessFixCurrencyChange = registrar.ClaAccessFixCurrencyChange;
            LCID = registrar.LCID;
        }
    }

    public class SettingsProfile : UserProfileEditBase
    {
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Address_Name", ResourceType = typeof(FieldResource))]
        public string Address { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_PSC_Name", ResourceType = typeof(FieldResource))]
        public string PSC { get; set; }

        [Display(Name = "Global_DIC_Name", ResourceType = typeof(FieldResource))]
        public string DIC { get; set; }

        [Display(Name = "Global_ICO_Name", ResourceType = typeof(FieldResource))]
        public string ICO { get; set; }

        [Display(Name = "UserProfile_ReminderTime_Name", ResourceType = typeof(FieldResource))]
        [RegularExpression(Regex.Time, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexTime_ErrorMessage")]
        public string ReminderTime { get; set; }

        [Display(Name = "UserProfile_IsEventsPrivate_Name", ResourceType = typeof(FieldResource))]
        public bool IsEventsPrivate { get; set; }

        public SettingsProfile()
        {

        }

        public SettingsProfile(UserProfile userProfile)
            : base(userProfile)
        {
            Address = userProfile.Address;
            PSC = userProfile.PSC;
            DIC = userProfile.DIC;
            ICO = userProfile.ICO;
            ReminderTime = userProfile.ReminderTime;
            IsEventsPrivate = userProfile.IsEventsPrivate;
        }

        public async Task<ModelStateDictionary> Validate(DefaultContext db, Controller controller, CancellationToken cancellationToken, bool isAdmin)
        {
            var modelStateDictionary = new ModelStateDictionary();

            modelStateDictionary.Merge(await ValidateGoogleCredentialsJson(controller, cancellationToken));

            UserProfile userProfile = UserProfileCache.GetDetail(db, UserId);
            Currency[] currencies = CurrencyCache.GetIndex(db);
            ValidateChangeClaAccessCurrency(userProfile, currencies, ref modelStateDictionary);
            ValidateLyonessId(db, ref modelStateDictionary);
            AddAdditionalInformation(userProfile, isAdmin);
            FixAddress();

            return modelStateDictionary;
        }

        private void AddAdditionalInformation(UserProfile userProfile, bool isAdmin)
        {
            Role = userProfile.Role;
            UserName = userProfile.UserName;

            if (!isAdmin)
            {
                PremiumMembershipGranted = userProfile.PremiumMembershipGranted;
                Ca = userProfile.Ca;
                Presenting = userProfile.Presenting;
                MspCoach = userProfile.MspCoach;
            }
        }

        private void FixAddress()
        {
            Address = FixMultilines(Address);
        }
    }

    public class CheckPolicies : BaseModelView
    {
        public UserProfileDetails UserProfile { get; set; }

        public int ViewData { get; set; }

        [Display(Name = "Global_ConfirmTermsAndConditions_Name", ResourceType = typeof(FieldResource))]
        public bool ConfirmTermsAndConditions { get; set; }

        [Display(Name = "Global_ConfirmOwnPersonalData_Name", ResourceType = typeof(FieldResource))]
        public bool ConfirmPersonalData { get; set; }

        [Display(Name = "AccountModelView_ConfirmContacts_Name", ResourceType = typeof(FieldResource))]
        public bool ConfirmContacts { get; set; }

        [Display(Name = "AccountModelView_ConfirmDeleteAccount_Name", ResourceType = typeof(FieldResource))]
        public bool ConfirmDeleteAccount { get; set; }

        public CheckPolicies()
        {

        }

        public CheckPolicies(DefaultContext db, int userId, int viewData)
        {
            UserProfile = UserProfileDetails.GetModelView(UserProfileCache.GetDetail(db, userId));
            ViewData = viewData;
        }

        protected override bool OnIsValid()
        {
            return UserProfile != null;
        }
    }

    public class CheckBillingInformation : BaseModelView
    {
        public int UserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_FirstName_Name", ResourceType = typeof(FieldResource))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_LastName_Name", ResourceType = typeof(FieldResource))]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Address_Name", ResourceType = typeof(FieldResource))]
        public string Address { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_City_Name", ResourceType = typeof(FieldResource))]
        public string City { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_PSC_Name", ResourceType = typeof(FieldResource))]
        public string PSC { get; set; }

        [Display(Name = "Global_DIC_Name", ResourceType = typeof(FieldResource))]
        public string DIC { get; set; }

        [Display(Name = "Global_ICO_Name", ResourceType = typeof(FieldResource))]
        public string ICO { get; set; }

        private readonly UserProfile _userProfile;

        public CheckBillingInformation()
        {
            
        }

        public CheckBillingInformation(DefaultContext db, int userId)
        {
            _userProfile = UserProfileCache.GetDetail(db, userId);
            if (_userProfile == null)
                return;

            UserId = _userProfile.UserId;
            FirstName = _userProfile.FirstName;
            LastName = _userProfile.LastName;
            Address = _userProfile.Address;
            City = _userProfile.City;
            PSC = _userProfile.PSC;
            DIC = _userProfile.DIC;
            ICO = _userProfile.ICO;
        }

        protected override bool OnIsValid()
        {
            return UserId != 0;
        }

        public ModelStateDictionary Validate()
        {
            var modelStateDictionary = new ModelStateDictionary();

            FixAddress();

            return modelStateDictionary;
        }

        private void FixAddress()
        {
            Address = FixMultilines(Address);
        }
    }
}