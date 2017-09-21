using LBT.Cache;
using LBT.Controllers;
using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using LBT.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LBT.ModelViews
{
    public class PeopleContactEdit : BaseModelView
    {
        public int PeopleContactId { get; set; }

        [Display(Name = "Global_District_Name", ResourceType = typeof(FieldResource))]
        public int? DistrictId { get; set; }

        public int? PhoneNumberPrefix1Id { get; set; }

        public int? PhoneNumberPrefix2Id { get; set; }

        public int RegistrarId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [StringLength(40, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_FirstName_Name", ResourceType = typeof(FieldResource))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [StringLength(40, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_LastName_Name", ResourceType = typeof(FieldResource))]
        public string LastName { get; set; }

        [StringLength(128, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_City_Name", ResourceType = typeof(FieldResource))]
        public string City { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "User_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [RegularExpression(Regex.OnlyNumberCharacters, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexOnlyNumbers_ErrorMessage")]
        [StringLength(40, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_PhoneNumber2_Name", ResourceType = typeof(FieldResource))]
        public string PhoneNumber2 { get; set; }

        [RegularExpression(Regex.Email, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexEmail_ErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_Email1_Name", ResourceType = typeof(FieldResource))]
        public string Email1 { get; set; }

        [RegularExpression(Regex.Email, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexEmail_ErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_Email2_Name", ResourceType = typeof(FieldResource))]
        public string Email2 { get; set; }

        public DateTime Created { get; set; }

        [Display(Name = "Contact_FirstContacted_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? FirstContacted { get; set; }

        [Display(Name = "Contact_Presented_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? Presented { get; set; }

        [Display(Name = "Contact_BusinessInfoParticipated_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? BusinessInfoParticipated { get; set; }

        [Display(Name = "Contact_TeamMeetingParticipated_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? TeamMeetingParticipated { get; set; }

        [Display(Name = "Contact_Registrated_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? Registrated { get; set; }

        [Display(Name = "Global_PremiumMembershipGranted_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? PremiumMembershipGranted { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RangeIntPositive_ErrorMessage")]
        [Display(Name = "Contact_Potential_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public int? Potential { get; set; }

        [Display(Name = "Contact_MobileApplicationInstalledAndTrained_Name", ResourceType = typeof(FieldResource))]
        public bool MobileApplicationInstalledAndTrained { get; set; }

        [Display(Name = "Contact_ContactDead_Name", ResourceType = typeof(FieldResource))]
        public bool ContactDead { get; set; }

        [RegularExpression(Regex.OnlyNumberCharacters, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexOnlyNumbers_ErrorMessage")]
        [StringLength(40, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_PhoneNumber1_Name", ResourceType = typeof(FieldResource))]
        public string PhoneNumber1 { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_Skype_Name", ResourceType = typeof(FieldResource))]
        public string Skype { get; set; }

        [Display(Name = "PeopleContact_TrackingEmailSent_Name", ResourceType = typeof(FieldResource))]
        public bool TrackingEmailSent { get; set; }

        [Display(Name = "PeopleContact_SecondContacted_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? SecondContacted { get; set; }

        [Display(Name = "PeopleContact_SecondMeeting_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? SecondMeeting { get; set; }

        [Display(Name = "PeopleContact_SecondTrackingEmailSent_Name", ResourceType = typeof(FieldResource))]
        public bool SecondTrackingEmailSent { get; set; }

        [Display(Name = "PeopleContact_ThirdMeeting_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? ThirdMeeting { get; set; }

        [Display(Name = "PeopleContact_LoyaltySystemExplained_Name", ResourceType = typeof(FieldResource))]
        public bool LoyaltySystemExplained { get; set; }

        [Display(Name = "Global_AccessGranted_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public DateTime? AccessGranted { get; set; }

        [Display(Name = "PeopleContact_MoneyToPurchaseAccountSended_Name", ResourceType = typeof(FieldResource))]
        public bool MoneyToPurchaseAccountSended { get; set; }

        [Display(Name = "PeopleContact_AbleToPurchase_Name", ResourceType = typeof(FieldResource))]
        public bool AbleToPurchase { get; set; }

        [Display(Name = "PeopleContact_AutoCashback_Name", ResourceType = typeof(FieldResource))]
        public bool AutoCashback { get; set; }

        [Display(Name = "PeopleContact_ShoppingPlanBackSet_Name", ResourceType = typeof(FieldResource))]
        public bool ShoppingPlanBackSet { get; set; }

        [Display(Name = "PeopleContact_OwnUnitsContained_Name", ResourceType = typeof(FieldResource))]
        public bool OwnUnitsContained { get; set; }

        [AllowHtml]
        [Display(Name = "Global_Tasks_Name", ResourceType = typeof(FieldResource))]
        public string Tasks { get; set; }

        [AllowHtml]
        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        public string Note { get; set; }

        [StringLength(15, MinimumLength = 15, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLengthMinMax2_ErrorMessage")]
        [RegularExpression(Regex.LyonessId, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexLyonessId_ErrorMessage")]
        [Display(Name = "Global_LyonessId_Name", ResourceType = typeof(FieldResource))]
        public string LyonessId { get; set; }

        public WorkflowState WorkflowState { get; set; }

        [Display(Name = "Global_ConfirmTermsAndConditions_Name", ResourceType = typeof(FieldResource))]
        public bool ConfirmTermsAndConditions { get; set; }

        [Display(Name = "Global_ConfirmPersonalData_Name", ResourceType = typeof(FieldResource))]
        public bool ConfirmPersonalData { get; set; }

        public PeopleContactEdit()
        {

        }

        public PeopleContactEdit(PeopleContact peopleContact)
        {
            PeopleContactId = peopleContact.PeopleContactId;
            DistrictId = peopleContact.DistrictId;
            PhoneNumberPrefix1Id = peopleContact.PhoneNumberPrefix1Id;
            PhoneNumberPrefix2Id = peopleContact.PhoneNumberPrefix2Id;
            RegistrarId = peopleContact.RegistrarId;
            FirstName = peopleContact.FirstName;
            LastName = peopleContact.LastName;
            City = peopleContact.City;
            Title = peopleContact.Title;
            PhoneNumber2 = peopleContact.PhoneNumber2;
            Email1 = peopleContact.Email1;
            Email2 = peopleContact.Email2;
            Created = peopleContact.Created;
            FirstContacted = peopleContact.FirstContacted;
            Presented = peopleContact.Presented;
            BusinessInfoParticipated = peopleContact.BusinessInfoParticipated;
            TeamMeetingParticipated = peopleContact.TeamMeetingParticipated;
            Registrated = peopleContact.Registrated;
            PremiumMembershipGranted = peopleContact.PremiumMembershipGranted;
            Potential = peopleContact.Potential;
            MobileApplicationInstalledAndTrained = peopleContact.MobileApplicationInstalledAndTrained;
            ContactDead = peopleContact.ContactDead;
            PhoneNumber1 = peopleContact.PhoneNumber1;
            Skype = peopleContact.Skype;
            TrackingEmailSent = peopleContact.TrackingEmailSent;
            SecondContacted = peopleContact.SecondContacted;
            SecondMeeting = peopleContact.SecondMeeting;
            SecondTrackingEmailSent = peopleContact.SecondTrackingEmailSent;
            ThirdMeeting = peopleContact.ThirdMeeting;
            LoyaltySystemExplained = peopleContact.LoyaltySystemExplained;
            AccessGranted = peopleContact.AccessGranted;
            MoneyToPurchaseAccountSended = peopleContact.MoneyToPurchaseAccountSended;
            AbleToPurchase = peopleContact.AbleToPurchase;
            AutoCashback = peopleContact.AutoCashback;
            ShoppingPlanBackSet = peopleContact.ShoppingPlanBackSet;
            OwnUnitsContained = peopleContact.OwnUnitsContained;
            Tasks = peopleContact.Tasks;
            Note = peopleContact.Note;
            LyonessId = peopleContact.LyonessId;
            WorkflowState = peopleContact.WorkflowState;
            ConfirmTermsAndConditions = peopleContact.ConfirmTermsAndConditions;
            ConfirmPersonalData = peopleContact.ConfirmPersonalData;
        }

        public static PeopleContactEdit GetModelView(PeopleContact peopleContact)
        {
            if (peopleContact == null)
                return null;

            var peopleContactEdit = new PeopleContactEdit(peopleContact);
            return peopleContactEdit;
        }

        public PeopleContact GetModel()
        {
            var peopleContact = new PeopleContact
                                    {
                                        PeopleContactId = PeopleContactId,
                                        DistrictId = DistrictId,
                                        PhoneNumberPrefix1Id = PhoneNumberPrefix1Id,
                                        PhoneNumberPrefix2Id = PhoneNumberPrefix2Id,
                                        RegistrarId = RegistrarId,
                                        FirstName = FirstName,
                                        LastName = LastName,
                                        City = City,
                                        Title = Title,
                                        PhoneNumber2 = PhoneNumber2,
                                        Email1 = Email1,
                                        Email2 = Email2,
                                        Created = Created,
                                        FirstContacted = FirstContacted,
                                        Presented = Presented,
                                        BusinessInfoParticipated = BusinessInfoParticipated,
                                        TeamMeetingParticipated = TeamMeetingParticipated,
                                        Registrated = Registrated,
                                        PremiumMembershipGranted = PremiumMembershipGranted,
                                        Potential = Potential,
                                        MobileApplicationInstalledAndTrained = MobileApplicationInstalledAndTrained,
                                        ContactDead = ContactDead,
                                        PhoneNumber1 = PhoneNumber1,
                                        Skype = Skype,
                                        TrackingEmailSent = TrackingEmailSent,
                                        SecondContacted = SecondContacted,
                                        SecondMeeting = SecondMeeting,
                                        SecondTrackingEmailSent = SecondTrackingEmailSent,
                                        ThirdMeeting = ThirdMeeting,
                                        LoyaltySystemExplained = LoyaltySystemExplained,
                                        AccessGranted = AccessGranted,
                                        MoneyToPurchaseAccountSended = MoneyToPurchaseAccountSended,
                                        AbleToPurchase = AbleToPurchase,
                                        AutoCashback = AutoCashback,
                                        ShoppingPlanBackSet = ShoppingPlanBackSet,
                                        OwnUnitsContained = OwnUnitsContained,
                                        Tasks = Tasks,
                                        Note = Note,
                                        LyonessId = LyonessId,
                                        WorkflowState = WorkflowState,
                                        ConfirmTermsAndConditions = ConfirmTermsAndConditions,
                                        ConfirmPersonalData = ConfirmPersonalData
                                    };
            return peopleContact;
        }

        public ModelStateDictionary Validate(DefaultContext db, int userId)
        {
            var modelStateDictionary = new ModelStateDictionary();
            ValidateFields(db, userId, ref modelStateDictionary);
            ValidateConfirmPolicies(ref modelStateDictionary);

            return modelStateDictionary;
        }

        private void ValidateFields(DefaultContext db, int userId, ref ModelStateDictionary modelStateDictionary)
        {
            if (String.IsNullOrEmpty(City))
            {
                modelStateDictionary.AddModelError(BaseCache.CityField, String.Format(ValidationResource.Global_Required_ErrorMessage, FieldResource.Global_City_Name));
            }

            if (!DistrictId.HasValue)
            {
                modelStateDictionary.AddModelError(BaseCache.DistrictIdField, String.Format(ValidationResource.Global_Required_ErrorMessage, FieldResource.Global_District_Name));
            }

            if (String.IsNullOrEmpty(PhoneNumber1) && String.IsNullOrEmpty(Skype) && String.IsNullOrEmpty(Email1))
            {
                modelStateDictionary.AddModelError(BaseCache.PhoneNumber1Field, ValidationResource.PeopleContact_ChoosePhoneNumber1_ErrorMessage);
            }
            else
            {
                object data;
                PeopleContactCache.ValidationStatus validationStatus =
                    PeopleContactCache.ValidateIsPhoneNumber1OrSkypeOrEmail1UniqueByUser(db, userId, PeopleContactId,
                                                                                         PhoneNumberPrefix1Id,
                                                                                         PhoneNumber1, Skype, Email1,
                                                                                         out data);
                if (validationStatus != PeopleContactCache.ValidationStatus.Ok)
                {
                    modelStateDictionary.AddModelError(BaseCache.PhoneNumber1Field, data.ToString());
                }
            }

            bool isLyonessIdValid = String.IsNullOrEmpty(LyonessId) || !LyonessId.Equals(UserProfile.DefaultLyonessId, StringComparison.InvariantCultureIgnoreCase);
            if (!isLyonessIdValid)
            {
                string errorMessage = String.Format(ValidationResource.Global_RegexLyonessId_ErrorMessage, FieldResource.Global_LyonessId_Name);
                modelStateDictionary.AddModelError(BaseCache.LyonessIdField, errorMessage);
            }

            DateTime today = DateTime.Today;
            if ((Presented.GetValueOrDefault() > today) ||
                (Presented.HasValue && FirstContacted.HasValue && Presented.Value < FirstContacted.Value))
            {
                modelStateDictionary.AddModelError(BaseCache.PresentedField, ValidationResource.PeopleContact_PresentedAndFirstContactedComparing_ErrorMessage);
            }

            if ((SecondContacted.GetValueOrDefault() > today) ||
                (SecondContacted.HasValue && Presented.HasValue && SecondContacted.Value < Presented.Value))
            {
                modelStateDictionary.AddModelError(BaseCache.SecondContactedField, ValidationResource.PeopleContact_SecondContactedAndPresentedComparing_ErrorMessage);
            }

            if ((SecondMeeting.GetValueOrDefault() > today) ||
                (SecondMeeting.HasValue && SecondContacted.HasValue && SecondMeeting.Value < SecondContacted.Value))
            {
                modelStateDictionary.AddModelError(BaseCache.SecondMeetingField, ValidationResource.PeopleContact_SecondMeetingAndSecondContactedComparing_ErrorMessage);
            }

            if ((BusinessInfoParticipated.GetValueOrDefault() > today) ||
                (BusinessInfoParticipated.HasValue && Presented.HasValue && BusinessInfoParticipated.Value < Presented.Value))
            {
                modelStateDictionary.AddModelError(BaseCache.BusinessInfoParticipatedField, ValidationResource.PeopleContact_BusinessInfoParticipatedAndPresentedComparing_ErrorMessage);
            }

            if ((ThirdMeeting.GetValueOrDefault() > today) ||
                (ThirdMeeting.HasValue && BusinessInfoParticipated.HasValue && ThirdMeeting.Value < BusinessInfoParticipated.Value))
            {
                modelStateDictionary.AddModelError(BaseCache.ThirdMeetingField, ValidationResource.PeopleContact_ThirdMeetingAndBusinessInfoParticipatedComparing_ErrorMessage);
            }
        }

        private void ValidateConfirmPolicies(ref ModelStateDictionary modelStateDictionary)
        {
            if (!ConfirmTermsAndConditions)
            {
                modelStateDictionary.AddModelError(BaseCache.ConfirmTermsAndConditionsField, ValidationResource.PeopleContact_NotConfirmedTermsAndConditions_ErrorMessage);
            }

            if (!ConfirmPersonalData)
            {
                modelStateDictionary.AddModelError(BaseCache.ConfirmPersonalDataField, ValidationResource.PeopleContact_NotConfirmedPersonalData_ErrorMessage);
            }
        }
    }

    public class PeopleContactDelete : BaseModelView
    {
        [Display(Name = "Global_FirstName_Name", ResourceType = typeof(FieldResource))]
        public string FirstName { get; set; }

        [Display(Name = "Global_LastName_Name", ResourceType = typeof(FieldResource))]
        public string LastName { get; set; }

        [Display(Name = "Global_City_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string City { get; set; }

        public PeopleContactDelete(PeopleContact peopleContact)
        {
            FirstName = peopleContact.FirstName;
            LastName = peopleContact.LastName;
            City = peopleContact.City;
        }

        public static PeopleContactDelete GetModelView(PeopleContact peopleContact)
        {
            if (peopleContact == null)
                return null;

            var peopleContactDelete = new PeopleContactDelete(peopleContact);
            return peopleContactDelete;
        }
    }

    public class PeopleContactDetails : BaseModelView
    {
        public int PeopleContactId { get; set; }

        [Display(Name = "Global_FirstName_Name", ResourceType = typeof(FieldResource))]
        public string FirstName { get; set; }

        [Display(Name = "Global_LastName_Name", ResourceType = typeof(FieldResource))]
        public string LastName { get; set; }

        [Display(Name = "User_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Display(Name = "Global_City_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string City { get; set; }

        [Display(Name = "Global_District_Name", ResourceType = typeof(FieldResource))]
        public DistrictDetails District { get; set; }

        public PhoneNumberPrefixDetails PhoneNumberPrefix1 { get; set; }

        public PhoneNumberPrefixDetails PhoneNumberPrefix2 { get; set; }

        [Display(Name = "Global_PhoneNumber2_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string PhoneNumber2 { get; set; }

        [Display(Name = "Global_Email1_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Email1 { get; set; }

        public string Email1PrintDetailView { get { return Email1; } }

        [Display(Name = "Global_Email2_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Email2 { get; set; }

        [Display(Name = "Contact_FirstContacted_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? FirstContacted { get; set; }

        public string FirstContactedPrintDetailView { get { return Grammar.ConvertDatetimeToText(FirstContacted); } }

        [Display(Name = "Contact_Presented_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? Presented { get; set; }

        public string PresentedPrintDetailView { get { return Grammar.ConvertDatetimeToText(Presented); } }

        [Display(Name = "Contact_BusinessInfoParticipated_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? BusinessInfoParticipated { get; set; }

        public string BusinessInfoParticipatedPrintDetailView { get { return Grammar.ConvertDatetimeToText(BusinessInfoParticipated); } }

        [Display(Name = "Contact_TeamMeetingParticipated_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? TeamMeetingParticipated { get; set; }

        public string TeamMeetingParticipatedPrintDetailView { get { return Grammar.ConvertDatetimeToText(TeamMeetingParticipated); } }

        [Display(Name = "Contact_Registrated_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? Registrated { get; set; }

        [Display(Name = "Global_PremiumMembershipGranted_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? PremiumMembershipGranted { get; set; }

        [Display(Name = "Contact_Potential_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public int? Potential { get; set; }

        [Display(Name = "Contact_MobileApplicationInstalledAndTrained_Name", ResourceType = typeof(FieldResource))]
        public bool MobileApplicationInstalledAndTrained { get; set; }

        [Display(Name = "Contact_ContactDead_Name", ResourceType = typeof(FieldResource))]
        public bool ContactDead { get; set; }

        [Display(Name = "Global_PhoneNumber1_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string PhoneNumber1 { get; set; }

        [Display(Name = "Global_Skype_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Skype { get; set; }

        [Display(Name = "PeopleContact_TrackingEmailSent_Name", ResourceType = typeof(FieldResource))]
        public bool TrackingEmailSent { get; set; }

        [Display(Name = "PeopleContact_SecondContacted_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? SecondContacted { get; set; }

        public string SecondContactedPrintDetailView { get { return Grammar.ConvertDatetimeToText(SecondContacted); } }

        [Display(Name = "PeopleContact_SecondMeeting_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? SecondMeeting { get; set; }

        public string SecondMeetingPrintDetailView { get { return Grammar.ConvertDatetimeToText(SecondMeeting); } }

        [Display(Name = "PeopleContact_SecondTrackingEmailSent_Name", ResourceType = typeof(FieldResource))]
        public bool SecondTrackingEmailSent { get; set; }

        [Display(Name = "PeopleContact_ThirdMeeting_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? ThirdMeeting { get; set; }

        public string ThirdMeetingPrintDetailView { get { return Grammar.ConvertDatetimeToText(ThirdMeeting); } }

        [Display(Name = "PeopleContact_LoyaltySystemExplained_Name", ResourceType = typeof(FieldResource))]
        public bool LoyaltySystemExplained { get; set; }

        [Display(Name = "Global_AccessGranted_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public DateTime? AccessGranted { get; set; }

        [Display(Name = "PeopleContact_MoneyToPurchaseAccountSended_Name", ResourceType = typeof(FieldResource))]
        public bool MoneyToPurchaseAccountSended { get; set; }

        [Display(Name = "PeopleContact_AbleToPurchase_Name", ResourceType = typeof(FieldResource))]
        public bool AbleToPurchase { get; set; }

        [Display(Name = "PeopleContact_AutoCashback_Name", ResourceType = typeof(FieldResource))]
        public bool AutoCashback { get; set; }

        [Display(Name = "PeopleContact_ShoppingPlanBackSet_Name", ResourceType = typeof(FieldResource))]
        public bool ShoppingPlanBackSet { get; set; }

        [Display(Name = "PeopleContact_OwnUnitsContained_Name", ResourceType = typeof(FieldResource))]
        public bool OwnUnitsContained { get; set; }

        [AllowHtml]
        [Display(Name = "Global_Tasks_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Tasks { get; set; }

        [AllowHtml]
        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Note { get; set; }

        [Display(Name = "Global_LyonessId_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string LyonessId { get; set; }

        public string LyonessIdPrintDetailView { get { return LyonessId; } }

        public WorkflowState WorkflowState { get; set; }

        public bool SendVideo { get; set; }

        public PeopleContactDetails(PeopleContact peopleContact)
        {
            PeopleContactId = peopleContact.PeopleContactId;
            FirstName = peopleContact.FirstName;
            LastName = peopleContact.LastName;
            Title = peopleContact.Title;
            City = peopleContact.City;
            District = DistrictDetails.GetModelView(peopleContact.District);
            PhoneNumberPrefix1 = PhoneNumberPrefixDetails.GetModelView(peopleContact.PhoneNumberPrefix1);
            PhoneNumberPrefix2 = PhoneNumberPrefixDetails.GetModelView(peopleContact.PhoneNumberPrefix2);
            PhoneNumber2 = peopleContact.PhoneNumber2;
            Email1 = peopleContact.Email1;
            Email2 = peopleContact.Email2;
            FirstContacted = peopleContact.FirstContacted;
            Presented = peopleContact.Presented;
            BusinessInfoParticipated = peopleContact.BusinessInfoParticipated;
            TeamMeetingParticipated = peopleContact.TeamMeetingParticipated;
            Registrated = peopleContact.Registrated;
            PremiumMembershipGranted = peopleContact.PremiumMembershipGranted;
            Potential = peopleContact.Potential;
            MobileApplicationInstalledAndTrained = peopleContact.MobileApplicationInstalledAndTrained;
            ContactDead = peopleContact.ContactDead;
            PhoneNumber1 = peopleContact.PhoneNumber1;
            Skype = peopleContact.Skype;
            TrackingEmailSent = peopleContact.TrackingEmailSent;
            SecondContacted = peopleContact.SecondContacted;
            SecondMeeting = peopleContact.SecondMeeting;
            SecondTrackingEmailSent = peopleContact.SecondTrackingEmailSent;
            ThirdMeeting = peopleContact.ThirdMeeting;
            LoyaltySystemExplained = peopleContact.LoyaltySystemExplained;
            AccessGranted = peopleContact.AccessGranted;
            MoneyToPurchaseAccountSended = peopleContact.MoneyToPurchaseAccountSended;
            AbleToPurchase = peopleContact.AbleToPurchase;
            AutoCashback = peopleContact.AutoCashback;
            ShoppingPlanBackSet = peopleContact.ShoppingPlanBackSet;
            OwnUnitsContained = peopleContact.OwnUnitsContained;
            Tasks = peopleContact.Tasks;
            Note = peopleContact.Note;
            LyonessId = peopleContact.LyonessId;
            WorkflowState = peopleContact.WorkflowState;
            SendVideo = !String.IsNullOrEmpty(peopleContact.Email1);
        }

        public static PeopleContactDetails GetModelView(PeopleContact peopleContact)
        {
            if (peopleContact == null)
                return null;

            var peopleContactDetails = new PeopleContactDetails(peopleContact);
            return peopleContactDetails;
        }
    }

    public class PeopleContactIndex : BaseModelView
    {
        private const string ContactDeadClass = "ContactDeadFormatting";
        private const string PremiumMembershipGrantedClass = "PremiumMembershipGrantedFormatting";
        private const string TeamMeetingParticipatedClass = "WorkflowFinishedFormatting";
        private const string WorkflowRunningClass = "WorkflowRunningFormatting";

        public int PeopleContactId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string City
        {
            get { return _city; }
            set { _city = Grammar.CutTextWithDots(value, 18); }
        }
        private string _city;

        public PhoneNumberPrefixDetails PhoneNumberPrefix1 { get; set; }

        public string Email1
        {
            get { return _email1; }
            set { _email1 = Grammar.CutTextWithDots(value, 28); }
        }
        private string _email1;

        public DateTime? TeamMeetingParticipated { get; set; }

        public DateTime? PremiumMembershipGranted { get; set; }

        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public int? Potential { get; set; }

        public bool ContactDead { get; set; }

        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string PhoneNumber1 { get; set; }

        public string Skype
        {
            get { return _skype; }
            set { _skype = Grammar.CutTextWithDots(value, 18); }
        }
        private string _skype;

        public WorkflowState WorkflowState { get; set; }

        public string FormattingClass
        {
            get
            {
                if (ContactDead)
                    return ContactDeadClass;

                if (PremiumMembershipGranted.HasValue)
                    return PremiumMembershipGrantedClass;

                if (TeamMeetingParticipated.HasValue)
                    return TeamMeetingParticipatedClass;

                if (WorkflowState != WorkflowState.Idle && WorkflowState != WorkflowState.Stopped && WorkflowState != WorkflowState.Finished)
                    return WorkflowRunningClass;

                return String.Empty;
            }
        }

        public PeopleContactIndex(PeopleContact peopleContact)
        {
            PeopleContactId = peopleContact.PeopleContactId;
            FirstName = peopleContact.FirstName;
            LastName = peopleContact.LastName;
            City = peopleContact.City;
            PhoneNumberPrefix1 = PhoneNumberPrefixDetails.GetModelView(peopleContact.PhoneNumberPrefix1);
            Email1 = peopleContact.Email1;
            TeamMeetingParticipated = peopleContact.TeamMeetingParticipated;
            PremiumMembershipGranted = peopleContact.PremiumMembershipGranted;
            Potential = peopleContact.Potential;
            ContactDead = peopleContact.ContactDead;
            PhoneNumber1 = peopleContact.PhoneNumber1;
            Skype = peopleContact.Skype;
            WorkflowState = peopleContact.WorkflowState;
        }

        public static PeopleContactIndex[] GetModelView(PeopleContact[] peopleContacts)
        {
            if (peopleContacts == null)
                return null;

            PeopleContactIndex[] peopleContactIndex = peopleContacts.Select(pc => new PeopleContactIndex(pc)).ToArray();
            return peopleContactIndex;
        }
    }

    public class Import : StoreModelView
    {
        public HttpPostedFileBase File { get; set; }

        [Display(Name = "Global_ConfirmTermsAndConditions_Name", ResourceType = typeof(FieldResource))]
        public bool ConfirmTermsAndConditions { get; set; }

        [Display(Name = "Global_ConfirmPersonalData_Name", ResourceType = typeof(FieldResource))]
        public bool ConfirmPersonalData { get; set; }

        public void Validate(BaseController baseController)
        {
            ValidateFile(baseController, File, FileStore.None, 4);
            ValidateConfirms(baseController);
        }

        private void ValidateConfirms(BaseController baseController)
        {
            if (!ConfirmTermsAndConditions)
            {
                baseController.ModelState.AddModelError(BaseCache.ConfirmTermsAndConditionsField, ValidationResource.PeopleContact_NotConfirmedTermsAndConditions_ErrorMessage);
            }

            if (!ConfirmPersonalData)
            {
                baseController.ModelState.AddModelError(BaseCache.ConfirmPersonalDataField, ValidationResource.PeopleContact_NotConfirmedPersonalData_ErrorMessage);
            }
        }
    }

    public class MultiplePeopleContacts : BaseModelView
    {
        public string MultiplePeopleContactsJson
        {
            get { return _multiplePeopleContactsJson; }
            set
            {
                _multiplePeopleContacts = null;
                _multiplePeopleContactsJson = value;
            }
        }
        private string _multiplePeopleContactsJson;

        public List<MultiplePeopleContact> MultiplePeopleContactsList
        {
            get
            {
                if (_multiplePeopleContacts != null)
                    return _multiplePeopleContacts;

                return _multiplePeopleContacts = NewtonsoftJsonSerializer.Instance.Deserialize<List<MultiplePeopleContact>>(MultiplePeopleContactsJson);
            }
        }
        private List<MultiplePeopleContact> _multiplePeopleContacts;

        [Display(Name = "Global_ConfirmTermsAndConditions_Name", ResourceType = typeof(FieldResource))]
        public bool ConfirmTermsAndConditions { get; set; }

        [Display(Name = "Global_ConfirmPersonalData_Name", ResourceType = typeof(FieldResource))]
        public bool ConfirmPersonalData { get; set; }

        public ModelStateDictionary Validate()
        {
            var modelStateDictionary = new ModelStateDictionary();
            ValidateFields(ref modelStateDictionary);

            return modelStateDictionary;
        }

        private void ValidateFields(ref ModelStateDictionary modelStateDictionary)
        {
            var multiplePeopleContactList = NewtonsoftJsonSerializer.Instance.Deserialize<List<MultiplePeopleContact>>(MultiplePeopleContactsJson);

            // Validace na alespoň jeden platný záznam
            if (!multiplePeopleContactList.Any())
            {
                modelStateDictionary.AddModelError(BaseCache.MultiplePeopleContactsJsonField, ValidationResource.Global_NoRecord_ErrorMessage);
            }

            // Validace na povinné položky
            var existInvalidPeopleContacts =
                multiplePeopleContactList.Any(
                    mpc => String.IsNullOrEmpty(mpc.LastName) || String.IsNullOrEmpty(mpc.FirstName));
            if (existInvalidPeopleContacts)
            {
                modelStateDictionary.AddModelError(BaseCache.MultiplePeopleContactsJsonField, ValidationResource.PeopleContact_LastNameAndFirstNameRequired_ErrorMessage);
            }

            // Validace položky LastName
            existInvalidPeopleContacts = multiplePeopleContactList.Any(mpc => mpc.LastName.Length > 40);
            if (existInvalidPeopleContacts)
            {
                modelStateDictionary.AddModelError(BaseCache.MultiplePeopleContactsJsonField, String.Format(ValidationResource.Global_StringLength3_ErrorMessage, FieldResource.Global_LastName_Name, 40));
            }

            // Validace položky FirstName
            existInvalidPeopleContacts = multiplePeopleContactList.Any(mpc => mpc.FirstName.Length > 40);
            if (existInvalidPeopleContacts)
            {
                modelStateDictionary.AddModelError(BaseCache.MultiplePeopleContactsJsonField, String.Format(ValidationResource.Global_StringLength3_ErrorMessage, FieldResource.Global_FirstName_Name, 40));
            }

            // Validace položky City
            existInvalidPeopleContacts = multiplePeopleContactList.Any(mpc => !String.IsNullOrEmpty(mpc.City) && mpc.City.Length > 128);
            if (existInvalidPeopleContacts)
            {
                modelStateDictionary.AddModelError(BaseCache.MultiplePeopleContactsJsonField, String.Format(ValidationResource.Global_StringLength3_ErrorMessage, FieldResource.Global_City_Name, 128));
            }

            // Validace položky PhoneNumber1
            existInvalidPeopleContacts = multiplePeopleContactList.Any(mpc => !String.IsNullOrEmpty(mpc.PhoneNumber1) && !System.Text.RegularExpressions.Regex.IsMatch(mpc.PhoneNumber1, Regex.OnlyNumberCharacters));
            if (existInvalidPeopleContacts)
            {
                modelStateDictionary.AddModelError(BaseCache.MultiplePeopleContactsJsonField, String.Format(ValidationResource.Global_RegexOnlyNumbers_ErrorMessage, FieldResource.Global_PhoneNumber1_Name));
            }

            existInvalidPeopleContacts = multiplePeopleContactList.Any(mpc => !String.IsNullOrEmpty(mpc.PhoneNumber1) && mpc.PhoneNumber1.Length > 40);
            if (existInvalidPeopleContacts)
            {
                modelStateDictionary.AddModelError(BaseCache.MultiplePeopleContactsJsonField, String.Format(ValidationResource.Global_StringLength3_ErrorMessage, FieldResource.Global_PhoneNumber1_Name, 40));
            }

            // Validace Email1
            existInvalidPeopleContacts = multiplePeopleContactList.Any(mpc => !String.IsNullOrEmpty(mpc.Email1) && !System.Text.RegularExpressions.Regex.IsMatch(mpc.Email1, Regex.Email));
            if (existInvalidPeopleContacts)
            {
                modelStateDictionary.AddModelError(BaseCache.MultiplePeopleContactsJsonField, String.Format(ValidationResource.Global_RegexEmail_ErrorMessage, FieldResource.Global_Email1_Name));
            }

            existInvalidPeopleContacts = multiplePeopleContactList.Any(mpc => !String.IsNullOrEmpty(mpc.Email1) && mpc.Email1.Length > 100);
            if (existInvalidPeopleContacts)
            {
                modelStateDictionary.AddModelError(BaseCache.MultiplePeopleContactsJsonField, String.Format(ValidationResource.Global_StringLength3_ErrorMessage, FieldResource.Global_Email1_Name, 100));
            }

            // Validace Skype
            existInvalidPeopleContacts = multiplePeopleContactList.Any(mpc => !String.IsNullOrEmpty(mpc.Skype) && mpc.Skype.Length > 100);
            if (existInvalidPeopleContacts)
            {
                modelStateDictionary.AddModelError(BaseCache.MultiplePeopleContactsJsonField, String.Format(ValidationResource.Global_StringLength3_ErrorMessage, FieldResource.Global_Skype_Name, 100));
            }

            if (!ConfirmTermsAndConditions)
            {
                modelStateDictionary.AddModelError(BaseCache.ConfirmTermsAndConditionsField, ValidationResource.PeopleContact_NotConfirmedTermsAndConditions_ErrorMessage);
            }

            if (!ConfirmPersonalData)
            {
                modelStateDictionary.AddModelError(BaseCache.ConfirmPersonalDataField, ValidationResource.PeopleContact_NotConfirmedPersonalData_ErrorMessage);
            }
        }
    }

    public class MultiplePeopleContact : BaseModelView
    {
        /// <summary>
        /// The last name
        /// </summary>
        public string LastName;
        /// <summary>
        /// The first name
        /// </summary>
        public string FirstName;
        /// <summary>
        /// The city
        /// </summary>
        public string City;
        /// <summary>
        /// The phone number prefix1 id
        /// </summary>
        public int? PhoneNumberPrefix1Id;
        /// <summary>
        /// The phone number1
        /// </summary>
        public string PhoneNumber1;
        /// <summary>
        /// The email1
        /// </summary>
        public string Email1;
        /// <summary>
        /// The skype
        /// </summary>
        public string Skype;
    }

    public sealed class PeopleContactSendVideo : BaseModelView
    {
        public int PeopleContactId { get; set; }

        [Display(Name = "PeopleContact_FullName_Name", ResourceType = typeof(FieldResource))]
        public string FullName { get; set; }

        [Display(Name = "Global_Video_Name", ResourceType = typeof(FieldResource))]
        public int? VideoId { get; set; }

        private string _email;

        private int _peopleContactId;

        public PeopleContactSendVideo()
        {
            
        }

        private PeopleContactSendVideo(PeopleContact peopleContact)
        {
            PeopleContactId = peopleContact.PeopleContactId;
            FullName = peopleContact.FullName;
        }

        public static PeopleContactSendVideo GetModelView(PeopleContact peopleContact)
        {
            if (peopleContact == null)
                return null;

            var peopleContactSendVideo = new PeopleContactSendVideo(peopleContact);
            return peopleContactSendVideo;
        }

        public ModelStateDictionary Validate(DefaultContext db, int userId, UrlHelper urlHelper)
        {
            var modelStateDictionary = new ModelStateDictionary();

            FixData(db);
            SendEmail(db, userId, urlHelper, ref modelStateDictionary);

            return modelStateDictionary;
        }

        private void FixData(DefaultContext db)
        {
            PeopleContact peopleContact = PeopleContactCache.GetDetail(db, PeopleContactId);
            if (peopleContact == null)
                return;

            FullName = peopleContact.FullName;
            _email = peopleContact.Email1;
            _peopleContactId = peopleContact.PeopleContactId;
        }

        private void SendEmail(DefaultContext db, int userId, UrlHelper urlHelper, ref ModelStateDictionary modelStateDictionary)
        {
            if (String.IsNullOrEmpty(_email) || _peopleContactId == 0)
            {
                modelStateDictionary.AddModelError(BaseCache.FullNameField, ValidationResource.PeopleContact_CannotSendEmail_ErrorMessage);
                return;
            }

            if (VideoId.GetValueOrDefault() == 0)
            {
                modelStateDictionary.AddModelError(BaseCache.VideoIdField, String.Format(ValidationResource.Global_Required_ErrorMessage, FieldResource.Global_Video_Name));
                return;
            }

            Video video = VideoCache.GetDetail(db, VideoId.GetValueOrDefault());
            if (video == null)
            {
                modelStateDictionary.AddModelError(BaseCache.VideoIdField, ValidationResource.PeopleContact_CannotReadVideo_ErrorMessage);
                return;
            }

            UserProfile userProfile = UserProfileCache.GetDetail(db, userId);
            if (userProfile == null)
            {
                throw new Exception("Cannot read UserProfile.");
            }

            VideoToken videoToken = VideoTokenCache.Insert(db, video.VideoId, userProfile.UserId, _peopleContactId);
            string token = Cryptography.Encrypt(videoToken.VideoTokenId.ToString(CultureInfo.InvariantCulture));
            string videoUrl = Url.GetActionAbsoluteUrl(urlHelper, "Player", "Video", new {token});
            string body = String.Format(video.EmailBody, userProfile.FullNameWithoutLyonessId, video.Title, videoUrl, Properties.Settings.Default.VideoTokenExpirationHours);
            bool success = Mail.SendEmail(_email, video.EmailSubject, body, true, true);
            if (!success)
            {
                modelStateDictionary.AddModelError(BaseCache.VideoIdField, ValidationResource.PeopleContact_CannotSendEmail_ErrorMessage);
            }
        }
    }
}