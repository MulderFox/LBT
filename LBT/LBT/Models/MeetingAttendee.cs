using LBT.ModelViews;
using LBT.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace LBT.Models
{
    public enum MeetingAttendeeType
    {
        Reserved = 0,
        ReserverWithAccommodation = 1,
        Registred = 2,
        RegistredWithAccommodation = 3
    }

    public class MeetingAttendee
    {
        public int MeetingAttendeeId { get; set; }

        [Column("MeetingId")]
        public virtual Meeting Meeting { get; set; }

        [ForeignKey("Meeting")]
        public int MeetingId { get; set; }

        [Column("AttendeeId")]
        public virtual PeopleContact Attendee { get; set; }

        [ForeignKey("Attendee")]
        public int? AttendeeId { get; set; }

        [Column("UserAttendeeId")]
        public virtual UserProfile UserAttendee { get; set; }

        [ForeignKey("UserAttendee")]
        public int? UserAttendeeId { get; set; }

        [Column("RegistrarId")]
        public virtual UserProfile Registrar { get; set; }

        [ForeignKey("Registrar")]
        public int RegistrarId { get; set; }

        public MeetingAttendeeType MeetingAttendeeType { get; set; }

        public int PaidAmount { get; set; }

        public int SecondPaidAmount { get; set; }

        public DateTime? Reserved { get; set; }

        public DateTime? Registered { get; set; }

        public string BonusLyonessId { get; set; }

        public string AccommodationNote { get; set; }

        [Display(Name = "MeetingAttendee_BankAccountHistoryNote_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = BaseModelView.NullDisplayText)]
        public string BankAccountHistoryNote { get; set; }

        [AllowHtml]
        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = BaseModelView.NullDisplayText)]
        public string Note { get; set; }
    }
}