using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using LBT.ModelViews;
using LBT.Resources;
using System;
using System.Linq;

namespace LBT.Cache
{
    public class MeetingAttendeeCache : BaseCache
    {
        public static MeetingAttendee GetDetail(DefaultContext db, int id)
        {
            MeetingAttendee meetingAttendee = db.MeetingAttendees.Find(id);
            return meetingAttendee;
        }

        public static void SignOrReserveAttendee(DefaultContext db, int meetingId, int attendeeId, int registrarId)
        {
            SignOrReserve(db, meetingId, attendeeId, 0, registrarId);
        }

        public static void SignOrReserveUserAttendee(DefaultContext db, int meetingId, int userAttendeeId, int registrarId)
        {
            SignOrReserve(db, meetingId, 0, userAttendeeId, registrarId);
        }

        public static void UnsignMeetingAttendee(DefaultContext db, int meetingAttendeeId, int userId, MeetingType meetingType, bool isAdmin, ref int meetingId)
        {
            Unsign(db, meetingAttendeeId, 0, 0, userId, meetingType, isAdmin, ref meetingId);
        }

        public static void UnsignAttendee(DefaultContext db, int meetingId, int attendeeId, int userId, MeetingType meetingType, bool isAdmin)
        {
            int dbMeetingId = meetingId;
            Unsign(db, 0, attendeeId, 0, userId, meetingType, isAdmin, ref dbMeetingId);
        }

        public static void UnsignUserAttendee(DefaultContext db, int meetingId, int userAttendeeId, int userId, MeetingType meetingType, bool isAdmin)
        {
            int dbMeetingId = meetingId;
            Unsign(db, 0, 0, userAttendeeId, userId, meetingType, isAdmin, ref dbMeetingId);
        }

        public static string GetMaxReservationDate(DateTime currentDateTime)
        {
            string maxReservationDate = currentDateTime.AddDays(Properties.Settings.Default.DaysForLockReservation).ToString("dd. MM. yyyy");
            return maxReservationDate;
        }

        public static void ProcessReservationExpiration(DefaultContext db)
        {
            DateTime maxReserved = DateTime.Now.AddDays(-Properties.Settings.Default.DaysForLockReservation);
            MeetingAttendee[] meetingAttendees =
                db.MeetingAttendees.Where(
                    ma =>
                    ma.Reserved != null && ma.Reserved < maxReserved &&
                    !ma.Registered.HasValue).ToArray();

            foreach (MeetingAttendee meetingAttendee in meetingAttendees)
            {
                // TODO: Dodělat logování prošlých rezervací

                db.MeetingAttendees.Remove(meetingAttendee);
            }

            db.SaveChanges();
        }

        private static void SignOrReserve(DefaultContext db, int meetingId, int attendeeId, int userAttendeeId, int registrarId)
        {
            Meeting meeting = MeetingCache.GetDetail(db, meetingId);
            if (meeting == null)
                throw new Exception(ValidationResource.Meeting_CannotFindMeeting_ErrorMessage);

            if (meeting.MeetingAttendees.Count >= meeting.Capacity)
                throw new Exception(ValidationResource.Meeting_CapacityOverloaded_ErrorMessage);

            DateTime currentDateTime = DateTime.Now;
            string maxReservationDate = GetMaxReservationDate(currentDateTime);

            var meetingAttendee = new MeetingAttendee
            {
                MeetingId = meetingId,
                RegistrarId = registrarId,
                MeetingAttendeeType = meeting.Price.GetValueOrDefault() == 0 ? MeetingAttendeeType.Registred : MeetingAttendeeType.Reserved,
                Reserved = currentDateTime,
                Registered = meeting.Price.GetValueOrDefault() == 0 ? currentDateTime : new DateTime?(),
            };

            string meetingDetail = MeetingCommon.GetMeetingDetail(meeting);
            string textBody;

            if (attendeeId != 0)
            {
                PeopleContact peopleContact = PeopleContactCache.GetDetail(db, attendeeId);
                if (peopleContact == null)
                    throw new Exception(ValidationResource.Meeting_CannotFindDbAttendee_ErrorMessage);

                if (!String.IsNullOrEmpty(peopleContact.LyonessId) && meeting.MeetingAttendees.Any(ma =>
                    ma.AttendeeId > 0 && !String.IsNullOrEmpty(ma.Attendee.LyonessId) && ma.Attendee.LyonessId.Equals(peopleContact.LyonessId) ||
                    ma.UserAttendeeId > 0 && ma.UserAttendee.LyonessId.Equals(peopleContact.LyonessId)))
                    throw new Exception(ValidationResource.Meeting_CannotAddDuplicateLyonessId_ErrorMessage);

                meetingAttendee.AttendeeId = attendeeId;
                meetingAttendee.BonusLyonessId = peopleContact.LyonessId;

                db.MeetingAttendees.Add(meetingAttendee);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Logger.SetLog(e);
                    throw new Exception(ValidationResource.Global_DbCommunicationFailed_ErrorMessage);
                }

                if (meeting.Price.GetValueOrDefault() == 0)
                {
                    if (!String.IsNullOrEmpty(peopleContact.Email1))
                    {
                        textBody = String.Format(MailResource.MeetingController_Registered_TextBody, meeting.Title, meetingDetail);
                        Mail.SendEmail(peopleContact.Email1, MailResource.MeetingController_Registered_Subject, textBody, true, true);
                    }

                    textBody = String.Format(MailResource.MeetingController_OwnerRegisteredContact_TextBody, peopleContact.FullName, meeting.Title, meetingDetail);
                    Mail.SendEmail(peopleContact.Registrar.Email1, MailResource.MeetingController_Registered_Subject, textBody, true, true);
                }
                else
                {
                    string paymentInfo = MeetingCommon.GetPaymentInfo(db, meeting, peopleContact.LyonessId);
                    if (!String.IsNullOrEmpty(peopleContact.Email1))
                    {
                        textBody = String.Format(MailResource.MeetingController_Signed_TextBody, meeting.Title, maxReservationDate, meetingDetail, paymentInfo);
                        Mail.SendEmail(peopleContact.Email1, MailResource.MeetingController_Signed_Subject, textBody, true, true);
                    }

                    textBody = String.Format(MailResource.MeetingController_OwnerSignedContact_TextBody, peopleContact.FullName, meeting.Title, maxReservationDate, meetingDetail, paymentInfo);
                    Mail.SendEmail(peopleContact.Registrar.Email1, MailResource.MeetingController_Signed_Subject, textBody, true, true);
                }
            }
            else
            {
                UserProfile userProfile = UserProfileCache.GetDetail(db, userAttendeeId);
                if (userProfile == null)
                    throw new Exception(ValidationResource.Meeting_CannotFindDbAttendee_ErrorMessage);

                if (meeting.MeetingAttendees.Any(ma =>
                    ma.AttendeeId > 0 && !String.IsNullOrEmpty(ma.Attendee.LyonessId) && ma.Attendee.LyonessId.Equals(userProfile.LyonessId) ||
                    ma.UserAttendeeId > 0 && ma.UserAttendee.LyonessId.Equals(userProfile.LyonessId)))
                    throw new Exception(ValidationResource.Meeting_CannotAddDuplicateLyonessId_ErrorMessage);

                meetingAttendee.UserAttendeeId = userAttendeeId;
                meetingAttendee.BonusLyonessId = userProfile.LyonessId;

                db.MeetingAttendees.Add(meetingAttendee);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Logger.SetLog(e);
                    throw new Exception(ValidationResource.Global_DbCommunicationFailed_ErrorMessage);
                }

                if (meeting.Price.GetValueOrDefault() == 0)
                {
                    textBody = String.Format(MailResource.MeetingController_Registered_TextBody, meeting.Title, meetingDetail);
                    Mail.SendEmail(userProfile.Email1, MailResource.MeetingController_Registered_Subject, textBody, true, true);
                }
                else
                {
                    string paymentInfo = MeetingCommon.GetPaymentInfo(db, meeting, userProfile.LyonessId);
                    textBody = String.Format(MailResource.MeetingController_Signed_TextBody, meeting.Title, maxReservationDate, meetingDetail, paymentInfo);
                    Mail.SendEmail(userProfile.Email1, MailResource.MeetingController_Signed_Subject, textBody, true, true);
                }
            }
        }

        private static void Unsign(DefaultContext db, int meetingAttendeeId, int attendeeId, int userAttendeeId, int userId, MeetingType meetingType, bool isAdmin, ref int meetingId)
        {
            MeetingAttendee[] meetingAttendees;
            if (meetingAttendeeId != 0)
            {
                meetingAttendees = new[] { db.MeetingAttendees.Find(meetingAttendeeId) };
            }
            else if (attendeeId != 0)
            {
                int dbMeetingId = meetingId;
                meetingAttendees = db.MeetingAttendees.Where(ma => ma.MeetingId == dbMeetingId && ma.AttendeeId == attendeeId).ToArray();
            }
            else
            {
                int dbMeetingId = meetingId;
                meetingAttendees = db.MeetingAttendees.Where(ma => ma.MeetingId == dbMeetingId && ma.UserAttendeeId == userAttendeeId).ToArray();
            }

            if (meetingAttendees.Length == 0)
                return;

            Meeting meeting = meetingAttendees[0].Meeting;
            if (meeting == null)
                throw new Exception(ValidationResource.Meeting_CannotFindMeeting_ErrorMessage);

            meetingId = meeting.MeetingId;

            if (meeting.Finished < DateTime.Now)
                throw new Exception(ValidationResource.Meeting_CannotUnsignAttendeeFromPastMeeting_ErrorMessage);

            string meetingDetail = MeetingCommon.GetMeetingDetail(meeting);
            foreach (MeetingAttendee meetingAttendee in meetingAttendees)
            {
                switch (meetingType)
                {
                    case MeetingType.Lgs:
                    case MeetingType.Webinar:
                    case MeetingType.MspEvening:
                    case MeetingType.Ostatni:
                        break;

                    case MeetingType.SetkaniTymu:
                        if (meetingAttendee.Registered.HasValue && !isAdmin && meetingAttendee.Meeting.OrganizerId != userId && meetingAttendee.Meeting.SecondaryOrganizerId != userId)
                            throw new Exception(ValidationResource.Meeting_CannotRemoveSignedAttendee_ErrorMessage);

                        if (meetingAttendee.Reserved.GetValueOrDefault().Date == DateTime.MaxValue.Date && !isAdmin && meetingAttendee.Meeting.OrganizerId != userId && meetingAttendee.Meeting.SecondaryOrganizerId != userId)
                            throw new Exception(ValidationResource.Meeting_NotAllowedRemoveAttendee_ErrorMessage);

                        break;

                    case MeetingType.SkoleniDavidaKotaska:
                        if (meetingAttendee.Registered.HasValue && !isAdmin && meetingAttendee.Meeting.OrganizerId != userId && meetingAttendee.Meeting.SecondaryOrganizerId != userId)
                            throw new Exception(ValidationResource.Meeting_CannotRemoveSignedAttendee_ErrorMessage);

                        if (meetingAttendee.Reserved.GetValueOrDefault().Date == DateTime.MaxValue.Date && !isAdmin && meetingAttendee.Meeting.OrganizerId != userId && meetingAttendee.Meeting.SecondaryOrganizerId != userId)
                            throw new Exception(ValidationResource.Meeting_NotAllowedRemoveAttendee_ErrorMessage);

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                string textBody;
                if (meetingAttendee.AttendeeId.HasValue)
                {
                    if (meetingAttendee.PaidAmount == 0)
                    {
                        if (!String.IsNullOrEmpty(meetingAttendee.Attendee.Email1))
                        {
                            textBody = String.Format(MailResource.MeetingController_Unsigned_TextBody, meeting.Title, meetingDetail);
                            Mail.SendEmail(meetingAttendee.Attendee.Email1, MailResource.MeetingController_Unsigned_Subject, textBody, true, true);
                        }

                        textBody = String.Format(MailResource.MeetingController_OwnerUnsignedContact_TextBody, meetingAttendee.Attendee.FullName, meeting.Title, meetingDetail);
                        Mail.SendEmail(meetingAttendee.Attendee.Registrar.Email1, MailResource.MeetingController_Unsigned_Subject, textBody, true, true);
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(meetingAttendee.Attendee.Email1))
                        {
                            textBody = String.Format(MailResource.MeetingController_UnsignedWithPaidAmmount_TextBody, meeting.Title, meetingDetail, meetingAttendee.PaidAmount);
                            Mail.SendEmail(meetingAttendee.Attendee.Email1, MailResource.MeetingController_Unsigned_Subject, textBody, true, true);
                        }

                        textBody = String.Format(MailResource.MeetingController_OwnerUnsignedContactWithPaidAmmount_TextBody, meetingAttendee.Attendee.FullName, meeting.Title, meetingDetail, meetingAttendee.PaidAmount);
                        Mail.SendEmail(meetingAttendee.Attendee.Registrar.Email1, MailResource.MeetingController_Unsigned_Subject, textBody, true, true);

                        textBody = String.Format(MailResource.MeetingController_OrganizerUnsignedContactWithPaidAmmount_TextBody, meetingAttendee.Attendee.FullName, meeting.Title, meetingDetail, meetingAttendee.PaidAmount);
                        Mail.SendEmail(meeting.Organizer.Email1, MailResource.MeetingController_Unsigned_Subject, textBody, true, true);

                        if (meeting.SecondaryOrganizerId.HasValue)
                        {
                            Mail.SendEmail(meeting.SecondaryOrganizer.Email1, MailResource.MeetingController_Unsigned_Subject, textBody, true, true);
                        }
                    }
                }
                else
                {
                    if (meetingAttendee.PaidAmount == 0)
                    {
                        textBody = String.Format(MailResource.MeetingController_Unsigned_TextBody, meeting.Title, meetingDetail);
                        Mail.SendEmail(meetingAttendee.UserAttendee.Email1, MailResource.MeetingController_Unsigned_Subject, textBody, true, true);
                    }
                    else
                    {
                        textBody = String.Format(MailResource.MeetingController_UnsignedWithPaidAmmount_TextBody, meeting.Title, meetingDetail, meetingAttendee.PaidAmount);
                        Mail.SendEmail(meetingAttendee.UserAttendee.Email1, MailResource.MeetingController_Unsigned_Subject, textBody, true, true);

                        textBody = String.Format(MailResource.MeetingController_OrganizerUnsignedWithPaidAmmount_TextBody, meetingAttendee.UserAttendee.FullName, meeting.Title, meetingDetail, meetingAttendee.PaidAmount);
                        Mail.SendEmail(meeting.Organizer.Email1, MailResource.MeetingController_Unsigned_Subject, textBody, true, true);

                        if (meeting.SecondaryOrganizerId.HasValue)
                        {
                            Mail.SendEmail(meeting.SecondaryOrganizer.Email1, MailResource.MeetingController_Unsigned_Subject, textBody, true, true);
                        }
                    }
                }

                db.MeetingAttendees.Remove(meetingAttendee);
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
                throw new Exception(ValidationResource.Global_DbCommunicationFailed_ErrorMessage);
            }
        }
    }
}