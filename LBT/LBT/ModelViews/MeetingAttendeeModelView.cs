using LBT.Models;
using LBT.Resources;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace LBT.ModelViews
{
    public class MeetingAttendeeDetails : BaseModelView
    {
        public PeopleContactDetails Attendee { get; set; }

        public int? AttendeeId { get; set; }

        public UserProfileDetails UserAttendee { get; set; }

        [Display(Name = "MeetingAttendee_BankAccountHistoryNote_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string BankAccountHistoryNote { get; set; }

        [AllowHtml]
        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Note { get; set; }

        public MeetingAttendeeDetails(MeetingAttendee meetingAttendee)
        {
            Attendee = PeopleContactDetails.GetModelView(meetingAttendee.Attendee);
            AttendeeId = meetingAttendee.AttendeeId;
            UserAttendee = UserProfileDetails.GetModelView(meetingAttendee.UserAttendee);
            BankAccountHistoryNote = meetingAttendee.BankAccountHistoryNote;
            Note = meetingAttendee.Note;
        }

        public static MeetingAttendeeDetails GetModelView(MeetingAttendee meetingAttendee)
        {
            if (meetingAttendee == null)
                return null;

            var meetingAttendeeDetails = new MeetingAttendeeDetails(meetingAttendee);
            return meetingAttendeeDetails;
        }
    }

    public class MeetingAttendeeEdit : BaseModelView
    {
        public int MeetingAttendeeId { get; set; }

        public PeopleContactDetails Attendee { get; set; }

        public int? AttendeeId { get; set; }

        public UserProfileDetails UserAttendee { get; set; }

        [Display(Name = "MeetingAttendee_BankAccountHistoryNote_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string BankAccountHistoryNote { get; set; }

        [AllowHtml]
        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Note { get; set; }

        public MeetingAttendeeEdit()
        {
            
        }

        public MeetingAttendeeEdit(MeetingAttendee meetingAttendee)
        {
            MeetingAttendeeId = meetingAttendee.MeetingAttendeeId;
            Attendee = PeopleContactDetails.GetModelView(meetingAttendee.Attendee);
            AttendeeId = meetingAttendee.AttendeeId;
            UserAttendee = UserProfileDetails.GetModelView(meetingAttendee.UserAttendee);
            BankAccountHistoryNote = meetingAttendee.BankAccountHistoryNote;
            Note = meetingAttendee.Note;
        }

        public static MeetingAttendeeEdit GetModelView(MeetingAttendee meetingAttendee)
        {
            if (meetingAttendee == null)
                return null;

            var meetingAttendeeEdit = new MeetingAttendeeEdit(meetingAttendee);
            return meetingAttendeeEdit;
        }
    }
}