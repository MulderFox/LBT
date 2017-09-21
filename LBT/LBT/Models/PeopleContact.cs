// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 12-20-2013
//
// Last Modified By : zmikeska
// Last Modified On : 01-18-2014
// ***********************************************************************
// <copyright file="PeopleContact.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.Cache;
using LBT.DAL;
using LBT.Helpers;
using LBT.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace LBT.Models
{
    public enum WorkflowState
    {
        /// <summary>
        /// The idle
        /// </summary>
        Idle = 0,
        /// <summary>
        /// The presented
        /// </summary>
        Presented,
        /// <summary>
        /// The second contacted
        /// </summary>
        SecondContacted,
        /// <summary>
        /// The second contacted and business info participated
        /// </summary>
        SecondContactedAndBusinessInfoParticipated,
        /// <summary>
        /// The business info participated
        /// </summary>
        BusinessInfoParticipated,
        /// <summary>
        /// The finished
        /// </summary>
        Finished,
        /// <summary>
        /// The stopped
        /// </summary>
        Stopped
    }

    public class PeopleContact
    {
        public int PeopleContactId { get; set; }

        [Required]
        [StringLength(40)]
        public string FirstName { get; set; }

        [NotMapped]
        public string FirstNameAcronym { get { return String.IsNullOrEmpty(FirstName) ? FirstName : String.Format("{0}.", FirstName[0]); } }

        [Required]
        [StringLength(40)]
        public string LastName { get; set; }

        [NotMapped]
        public string LastNameAcronym { get { return String.IsNullOrEmpty(LastName) ? LastName : String.Format("{0}.", LastName[0]); } }

        [NotMapped]
        public string FullName { get { return String.Format("{0} {1} ({2})", LastName, FirstName, LyonessId); } }

        [StringLength(20)]
        public string Title { get; set; }

        [StringLength(128)]
        public string City { get; set; }

        [NotMapped]
        public string CityIndexView { get { return Grammar.CutTextWithDots(City, 18); } }

        [ForeignKey("District")]
        public int? DistrictId { get; set; }

        [Column("DistrictId")]
        public virtual District District { get; set; }

        [Column("PhoneNumberPrefix1Id")]
        public virtual PhoneNumberPrefix PhoneNumberPrefix1 { get; set; }

        [ForeignKey("PhoneNumberPrefix2")]
        public int? PhoneNumberPrefix2Id { get; set; }

        [Column("PhoneNumberPrefix2Id")]
        public virtual PhoneNumberPrefix PhoneNumberPrefix2 { get; set; }

        [ForeignKey("Registrar")]
        public int RegistrarId { get; set; }

        [Column("RegistrarId")]
        public virtual UserProfile Registrar { get; set; }

        [StringLength(40)]
        public string PhoneNumber2 { get; set; }

        [StringLength(100)]
        public string Email1 { get; set; }

        [StringLength(100)]
        public string Email2 { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime? FirstContacted { get; set; }

        public DateTime? Presented { get; set; }

        public DateTime? BusinessInfoParticipated { get; set; }

        public DateTime? TeamMeetingParticipated { get; set; }

        public DateTime? Registrated { get; set; }

        public DateTime? PremiumMembershipGranted { get; set; }

        public int? Potential { get; set; }

        public bool MobileApplicationInstalledAndTrained { get; set; }

        public bool ContactDead { get; set; }

        [ForeignKey("PhoneNumberPrefix1")]
        public int? PhoneNumberPrefix1Id { get; set; }

        [StringLength(40)]
        public string PhoneNumber1 { get; set; }

        [StringLength(100)]
        public string Skype { get; set; }

        public bool TrackingEmailSent { get; set; }

        public DateTime? SecondContacted { get; set; }

        public DateTime? SecondMeeting { get; set; }

        public bool SecondTrackingEmailSent { get; set; }

        public DateTime? ThirdMeeting { get; set; }

        public bool LoyaltySystemExplained { get; set; }

        public DateTime? AccessGranted { get; set; }

        public bool MoneyToPurchaseAccountSended { get; set; }

        public bool AbleToPurchase { get; set; }

        public bool AutoCashback { get; set; }

        public bool ShoppingPlanBackSet { get; set; }

        public bool OwnUnitsContained { get; set; }

        public string Tasks { get; set; }

        public string Note { get; set; }

        [StringLength(15)]
        public string LyonessId { get; set; }

        [Required]
        public WorkflowState WorkflowState { get; set; }

        [Required]
        public WorkflowState WorkflowStatePrevious { get; set; }

        [NotMapped]
        public bool ConfirmTermsAndConditions { get; set; }

        [NotMapped]
        public bool ConfirmPersonalData { get; set; }

        public void CopyFrom(PeopleContact peopleContact)
        {
            DistrictId = peopleContact.DistrictId;
            PhoneNumberPrefix2Id = peopleContact.PhoneNumberPrefix2Id;
            RegistrarId = peopleContact.RegistrarId;
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
            FirstName = peopleContact.FirstName;
            LastName = peopleContact.LastName;
            Title = peopleContact.Title;
            City = peopleContact.City;
            PhoneNumberPrefix1Id = peopleContact.PhoneNumberPrefix1Id;
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
            WorkflowStatePrevious = peopleContact.WorkflowStatePrevious;
            ConfirmTermsAndConditions = peopleContact.ConfirmTermsAndConditions;
            ConfirmPersonalData = peopleContact.ConfirmPersonalData;
        }

        public ModelStateDictionary Validate(DefaultContext db, int userId)
        {
            var modelStateDictionary = new ModelStateDictionary();
            ValidateFields(db, userId, ref modelStateDictionary);
            ValidateConfirmPolicies(ref modelStateDictionary);

            return modelStateDictionary;
        }

        public void CheckDuplicity(DefaultContext db)
        {
            if (!PhoneNumberPrefix1Id.HasValue && !String.IsNullOrEmpty(PhoneNumber1))
                return;

            PeopleContact[] peopleContacts;
            if (PhoneNumberPrefix1Id.HasValue && !String.IsNullOrEmpty(PhoneNumber1))
            {
                peopleContacts = db.PeopleContacts.Include(p => p.Registrar).Include(p => p.PhoneNumberPrefix1)
                    .Where(
                        p =>
                            p.PhoneNumberPrefix1.PhoneNumberPrefixId == PhoneNumberPrefix1Id.Value &&
                            p.PhoneNumber1 == PhoneNumber1).ToArray();

                if (peopleContacts.Length > 1)
                    ProcessDuplicity(peopleContacts);

                return;
            }

            if (!String.IsNullOrEmpty(Skype))
            {
                peopleContacts = db.PeopleContacts.Include(p => p.Registrar).Include(p => p.PhoneNumberPrefix1)
                    .Where(
                        p =>
                            p.Skype == Skype).ToArray();

                if (peopleContacts.Length > 1)
                    ProcessDuplicity(peopleContacts);

                return;
            }

            if (!String.IsNullOrEmpty(Email1))
            {
                peopleContacts = db.PeopleContacts.Include(p => p.Registrar).Include(p => p.PhoneNumberPrefix1)
                    .Where(
                        p =>
                            p.Email1 == Email1).ToArray();

                if (peopleContacts.Length > 1)
                    ProcessDuplicity(peopleContacts);
            }
        }

        public PeopleContactTask[] GetPeopleContactTasks()
        {
            var peopleContactTasks = new List<PeopleContactTask>();
            if (!TrackingEmailSent)
            {
                peopleContactTasks.Add(GetTrackingEmailSentTask());
            }

            switch (WorkflowState)
            {
                case WorkflowState.Presented:
                    peopleContactTasks.Add(GetSecondContactedTask());
                    peopleContactTasks.Add(GetBusinessInfoParticipatedTask());
                    break;

                case WorkflowState.SecondContacted:
                    if (!SecondTrackingEmailSent)
                    {
                        peopleContactTasks.Add(GetSecondTrackingEmailSentTask());
                    }

                    if (!SecondMeeting.HasValue)
                    {
                        peopleContactTasks.Add(GetSecondMeetingTask());
                    }

                    peopleContactTasks.Add(GetBusinessInfoParticipatedTask());
                    break;

                case WorkflowState.SecondContactedAndBusinessInfoParticipated:
                    if (!SecondTrackingEmailSent)
                    {
                        peopleContactTasks.Add(GetSecondTrackingEmailSentTask());
                    }

                    if (!SecondMeeting.HasValue)
                    {
                        peopleContactTasks.Add(GetSecondMeetingTask());
                    }

                    if (!ThirdMeeting.HasValue)
                    {
                        peopleContactTasks.Add(GetThirdMeetingTask());
                    }
                    break;

                case WorkflowState.BusinessInfoParticipated:
                    peopleContactTasks.Add(GetSecondContactedTask());

                    if (!ThirdMeeting.HasValue)
                    {
                        peopleContactTasks.Add(GetThirdMeetingTask());
                    }
                    break;
            }

            return peopleContactTasks.ToArray();
        }

        public PeopleContactTask GetPeopleContactTasks(string fieldName)
        {
            PeopleContactTask[] peopleContactTasks = GetPeopleContactTasks();
            if (peopleContactTasks.All(pct => pct.FieldName != fieldName))
                return null;

            PeopleContactTask peopleContactTask = peopleContactTasks.First(pct => pct.FieldName == fieldName);
            return peopleContactTask;
        }

        private PeopleContactTask GetTrackingEmailSentTask()
        {
            DateTime maxDate = Presented.GetValueOrDefault(DateTime.Today);
            var peopleContactTask = new PeopleContactTask
            {
                FieldName = BaseCache.TrackingEmailSentField,
                PeopleContactId = PeopleContactId,
                PeopleContactTaskType = DateTime.Today <= maxDate ? PeopleContactTaskType.Actual : PeopleContactTaskType.Delayed,
                RegistrarId = RegistrarId,
                Text = String.Format(ViewResource.PeopleContact_GetTrackingEmailSentTask_Text, LastName, FirstName),
                UseMail = Registrar.UseMail,
                Email1 = Registrar.Email1,
                UseSms = Registrar.UseSms,
                SmsEmail = Registrar.SmsEmail
            };
            return peopleContactTask;
        }

        private PeopleContactTask GetSecondContactedTask()
        {
            DateTime minDate = Presented.GetValueOrDefault(DateTime.Today).AddDays(1);
            DateTime maxDate = minDate;
            var peopleContactTask = new PeopleContactTask
            {
                FieldName = BaseCache.SecondContactedField,
                PeopleContactId = PeopleContactId,
                PeopleContactTaskType = DateTime.Today <= maxDate ? PeopleContactTaskType.Actual : PeopleContactTaskType.Delayed,
                RegistrarId = RegistrarId,
                Text = String.Format(ViewResource.PeopleContact_GetSecondContactedTask_Text, LastName, FirstName, minDate.ToString("dd.MM.yyyy")),
                UseMail = Registrar.UseMail,
                Email1 = Registrar.Email1,
                UseSms = Registrar.UseSms,
                SmsEmail = Registrar.SmsEmail
            };
            return peopleContactTask;
        }

        private PeopleContactTask GetBusinessInfoParticipatedTask()
        {
            DateTime minDate = Presented.GetValueOrDefault(DateTime.Today).AddDays(1);
            DateTime maxDate = minDate.AddDays(13);
            var peopleContactTask = new PeopleContactTask
            {
                FieldName = BaseCache.BusinessInfoParticipatedField,
                PeopleContactId = PeopleContactId,
                PeopleContactTaskType = DateTime.Today <= maxDate ? PeopleContactTaskType.Actual : PeopleContactTaskType.Delayed,
                RegistrarId = RegistrarId,
                Text = String.Format(ViewResource.PeopleContact_GetBusinessInfoParticipatedTask_Text, LastName, FirstName, minDate.ToString("dd.MM.yyyy")),
                UseMail = Registrar.UseMail,
                Email1 = Registrar.Email1,
                UseSms = Registrar.UseSms,
                SmsEmail = Registrar.SmsEmail
            };
            return peopleContactTask;
        }
        private PeopleContactTask GetSecondTrackingEmailSentTask()
        {
            DateTime maxDate = SecondContacted.GetValueOrDefault(DateTime.Today);
            var peopleContactTask = new PeopleContactTask
            {
                FieldName = BaseCache.SecondTrackingEmailSentField,
                PeopleContactId = PeopleContactId,
                PeopleContactTaskType = DateTime.Today <= maxDate ? PeopleContactTaskType.Actual : PeopleContactTaskType.Delayed,
                RegistrarId = RegistrarId,
                Text = String.Format(ViewResource.PeopleContact_GetSecondTrackingEmailSentTask_Text, LastName, FirstName),
                UseMail = Registrar.UseMail,
                Email1 = Registrar.Email1,
                UseSms = Registrar.UseSms,
                SmsEmail = Registrar.SmsEmail
            };
            return peopleContactTask;
        }

        private PeopleContactTask GetSecondMeetingTask()
        {
            DateTime minDate = SecondContacted.GetValueOrDefault(DateTime.Today).AddDays(1);
            DateTime maxDate = minDate.AddDays(1);
            var peopleContactTask = new PeopleContactTask
            {
                FieldName = BaseCache.SecondMeetingField,
                PeopleContactId = PeopleContactId,
                PeopleContactTaskType = DateTime.Today <= maxDate ? PeopleContactTaskType.Actual : PeopleContactTaskType.Delayed,
                RegistrarId = RegistrarId,
                Text = String.Format(ViewResource.PeopleContact_GetSecondMeetingTask_Text, LastName, FirstName, minDate.ToString("dd.MM.yyyy")),
                UseMail = Registrar.UseMail,
                Email1 = Registrar.Email1,
                UseSms = Registrar.UseSms,
                SmsEmail = Registrar.SmsEmail
            };
            return peopleContactTask;
        }

        private PeopleContactTask GetThirdMeetingTask()
        {
            DateTime minDate = BusinessInfoParticipated.GetValueOrDefault(DateTime.Today).AddDays(2);
            DateTime maxDate = minDate.AddDays(2);
            var peopleContactTask = new PeopleContactTask
            {
                FieldName = BaseCache.ThirdMeetingField,
                PeopleContactId = PeopleContactId,
                PeopleContactTaskType = DateTime.Today <= maxDate ? PeopleContactTaskType.Actual : PeopleContactTaskType.Delayed,
                RegistrarId = RegistrarId,
                Text = String.Format(ViewResource.PeopleContact_GetThirdMeetingTask_Text, LastName, FirstName, minDate.ToString("dd.MM.yyyy")),
                UseMail = Registrar.UseMail,
                Email1 = Registrar.Email1,
                UseSms = Registrar.UseSms,
                SmsEmail = Registrar.SmsEmail
            };
            return peopleContactTask;
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

        /// <summary>
        /// Processes the duplicity.
        /// </summary>
        /// <param name="peopleContacts">The people contacts.</param>
        private void ProcessDuplicity(PeopleContact[] peopleContacts)
        {
            if (!peopleContacts.Any())
                return;

            foreach (PeopleContact peopleContact in peopleContacts)
            {
                string email = String.IsNullOrEmpty(peopleContact.Registrar.Email1)
                                   ? peopleContact.Registrar.Email2
                                   : peopleContact.Registrar.Email1;
                if (String.IsNullOrEmpty(email))
                    continue;

                string textBody = String.Format(peopleContact.Registrated.HasValue
                                                    ? MailResource.PeopleContactController_ProcessDuplicityAndGetStatusMessage_IsRegistrated_TextBody
                                                    : MailResource.PeopleContactController_ProcessDuplicityAndGetStatusMessage_IsNotRegistrated_TextBody,
                                                peopleContact.FirstName, peopleContact.LastName);
                Mail.SendEmail(email, MailResource.PeopleContactController_ProcessDuplicityAndGetStatusMessage_Subject, textBody, peopleContact.Registrar.UseMail, true);
            }
        }
    }

    /// <summary>
    /// Class PeopleContactTask
    /// </summary>
    public class PeopleContactTask
    {
        /// <summary>
        /// Gets or sets the people contact id.
        /// </summary>
        /// <value>The people contact id.</value>
        public int PeopleContactId { get; set; }

        /// <summary>
        /// Gets or sets the registrar id.
        /// </summary>
        /// <value>The registrar id.</value>
        public int RegistrarId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use mail].
        /// </summary>
        /// <value><c>true</c> if [use mail]; otherwise, <c>false</c>.</value>
        public bool UseMail { get; set; }

        /// <summary>
        /// Gets or sets the email1.
        /// </summary>
        /// <value>The email1.</value>
        public string Email1 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use SMS].
        /// </summary>
        /// <value><c>true</c> if [use SMS]; otherwise, <c>false</c>.</value>
        public bool UseSms { get; set; }

        /// <summary>
        /// Gets or sets the SMS email.
        /// </summary>
        /// <value>The SMS email.</value>
        public string SmsEmail { get; set; }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>The name of the field.</value>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the field value.
        /// </summary>
        /// <value>The field value.</value>
        public DateTime? FieldValue { get; set; }

        /// <summary>
        /// Gets the text field value.
        /// </summary>
        /// <value>The text field value.</value>
        public string TextFieldValue
        {
            get { return FieldValue.GetValueOrDefault().ToString("dd.MM.yyyy"); }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the type of the people contact task.
        /// </summary>
        /// <value>The type of the people contact task.</value>
        public PeopleContactTaskType PeopleContactTaskType { get; set; }
    }

    /// <summary>
    /// Enum PeopleContactTaskType
    /// </summary>
    public enum PeopleContactTaskType
    {
        /// <summary>
        /// The actual
        /// </summary>
        Actual,
        /// <summary>
        /// The delayed
        /// </summary>
        Delayed
    }

    /// <summary>
    /// Class PeopleContactStopWorkflow
    /// </summary>
    public class PeopleContactStopWorkflow
    {
        /// <summary>
        /// The note
        /// </summary>
        /// <value>The note.</value>
        [AllowHtml]
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        public string Note { get; set; }
    }
}