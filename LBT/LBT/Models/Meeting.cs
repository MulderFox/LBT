using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using LBT.Resources;

namespace LBT.Models
{
    public enum MeetingKind
    {
        Public = 0,
        LifeLine = 1,
        Invited = 2,
        MSP = 3
    }

    public enum MeetingType
    {
        BusinessInfo = 0,
        SetkaniTymu = 1,
        WorkshopyBi = 2,
        Leaders = 3,
        SkoleniDavidaKotaska = 4,
        Ostatni = 5,
        MspEvening = 6,
        Webinar = 7
    }

    public class Meeting
    {
        public virtual ICollection<MeetingAttendee> MeetingAttendees { get; set; }

        [NotMapped]
        public int FillCapacity
        {
            get { return MeetingAttendees != null ? MeetingAttendees.Count : 0; }
        }

        public int MeetingId { get; set; }

        [NotMapped]
        public string SpecificSymbol
        {
            get
            {
                string strMeetingId = MeetingId.ToString(CultureInfo.InvariantCulture);
                return strMeetingId.Substring(Math.Max(strMeetingId.Length - 10, 0), Math.Min(strMeetingId.Length, 10));
            }
        }

        [Column("OrganizerId")]
        public virtual UserProfile Organizer { get; set; }

        [ForeignKey("Organizer")]
        public int OrganizerId { get; set; }

        [Column("SecondaryOrganizerId")]
        public virtual UserProfile SecondaryOrganizer { get; set; }

        [ForeignKey("SecondaryOrganizer")]
        public int? SecondaryOrganizerId { get; set; }

        [NotMapped]
        public string PrimaryAndSecondaryOrganizerFullName
        {
            get
            {
                string organizer = Organizer.FullNameWithoutLyonessId;
                if (SecondaryOrganizerId.HasValue)
                {
                    organizer = String.Format("{0}, {1}", organizer, SecondaryOrganizer.FullNameWithoutLyonessId);
                }

                return organizer;
            }
        }

        [Column("MainLeaderId")]
        public virtual UserProfile MainLeader { get; set; }

        [ForeignKey("MainLeader")]
        public int MainLeaderId { get; set; }

        [Column("SecondaryLeaderId")]
        public virtual UserProfile SecondaryLeader { get; set; }

        [ForeignKey("SecondaryLeader")]
        public int? SecondaryLeaderId { get; set; }

        [Column("MeetingTitleTypeId")]
        public virtual MeetingTitleType MeetingTitleType { get; set; }

        [ForeignKey("MeetingTitleType")]
        public int? MeetingTitleTypeId { get; set; }

        [Column("BankAccountId")]
        public virtual BankAccount BankAccount { get; set; }

        [ForeignKey("BankAccount")]
        public int? BankAccountId { get; set; }

        [Column("SecondBankAccountId")]
        public virtual BankAccount SecondBankAccount { get; set; }

        [ForeignKey("SecondBankAccount")]
        public int? SecondBankAccountId { get; set; }

        [Required]
        public string Title { get; set; }

        public MeetingKind MeetingKind { get; set; }

        public MeetingType MeetingType { get; set; }

        public string City { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public bool Chargeable { get; set; }

        public int? Price { get; set; }

        public int? SecondPrice { get; set; }

        public bool WithAccommodation { get; set; }

        public int? PriceWithAccommodation { get; set; }

        public DateTime Started { get; set; }

        public DateTime Finished { get; set; }

        public string BringYourOwn { get; set; }

        public int Capacity { get; set; }

        public int AccomodationCapacity { get; set; }

        public DateTime? RegisterDeadline { get; set; }

        public string Lecturer { get; set; }

        public string InvitationCardUrl { get; set; }

        public string Note { get; set; }

        public int? Total { get; set; }

        public int? TotalWithAccommodation { get; set; }

        public bool Private { get; set; }

        public string WebinarUrl { get; set; }

        public void CopyFrom(Meeting meeting)
        {
            OrganizerId = meeting.OrganizerId;
            SecondaryOrganizerId = meeting.SecondaryOrganizerId;
            MainLeaderId = meeting.MainLeaderId;
            SecondaryLeaderId = meeting.SecondaryLeaderId;
            MeetingTitleTypeId = meeting.MeetingTitleTypeId;
            Title = meeting.Title;
            MeetingKind = meeting.MeetingKind;
            MeetingType = meeting.MeetingType;
            City = meeting.City;
            AddressLine1 = meeting.AddressLine1;
            AddressLine2 = meeting.AddressLine2;
            Chargeable = meeting.Chargeable;
            Price = meeting.Price;
            SecondPrice = meeting.SecondPrice;
            BankAccountId = meeting.BankAccountId;
            SecondBankAccountId = meeting.SecondBankAccountId;
            WithAccommodation = meeting.WithAccommodation;
            PriceWithAccommodation = meeting.PriceWithAccommodation;
            Started = meeting.Started;
            Finished = meeting.Finished;
            BringYourOwn = meeting.BringYourOwn;
            Capacity = meeting.Capacity;
            AccomodationCapacity = meeting.AccomodationCapacity;
            RegisterDeadline = meeting.RegisterDeadline;
            Lecturer = meeting.Lecturer;
            InvitationCardUrl = meeting.InvitationCardUrl;
            Note = meeting.Note;
            Private = meeting.Private;
            WebinarUrl = meeting.WebinarUrl;
        }

        public static string GetMeetingTypeLocalizedText(MeetingType meetingType)
        {
            switch (meetingType)
            {
                case MeetingType.BusinessInfo:
                    return ListItemsResource.MeetingType_Bi;

                case MeetingType.SetkaniTymu:
                    return ListItemsResource.MeetingType_SetkaniTymu;

                case MeetingType.WorkshopyBi:
                    return ListItemsResource.MeetingType_WorkshopyBi;

                case MeetingType.Leaders:
                    return ListItemsResource.MeetingType_Leaders;

                case MeetingType.SkoleniDavidaKotaska:
                    return ListItemsResource.MeetingType_SkoleniDavidaKotaska;

                case MeetingType.Ostatni:
                    return ListItemsResource.MeetingType_Ostatni;

                case MeetingType.MspEvening:
                    return ListItemsResource.MeetingType_MspEvening;

                case MeetingType.Webinar:
                    return ListItemsResource.MeetingType_Webinar;

                default:
                    throw new ArgumentOutOfRangeException("meetingType");
            }
        }

        public static Dictionary<MeetingType, string> GetTranslationDictionaryForBankAccountType()
        {
            var translationDictionary = new Dictionary<MeetingType, string>();
            foreach (MeetingType meetingType in Enum.GetValues(typeof(MeetingType)))
            {
                translationDictionary[meetingType] = GetMeetingTypeLocalizedText(meetingType);
            }
            return translationDictionary;
        }
    }
}