using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using LBT.ModelViews;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace LBT.Cache
{
    public class MeetingCache : BaseCache
    {
        public static string GetMeetingAction(Meeting meeting)
        {
            switch (meeting.MeetingType)
            {
                case MeetingType.Lgs:
                    return "BusinessInfoDetails";

                case MeetingType.SetkaniTymu:
                    return "SetkaniTymuDetails";

                case MeetingType.SkoleniDavidaKotaska:
                    return "SkoleniDavidaKotaskaDetails";

                case MeetingType.Ostatni:
                    return "OstatniDetails";

                case MeetingType.MspEvening:
                    return "MspEveningDetails";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public abstract class MeetingCacheBase
        {
            public int MeetingId { get; set; }
            public DateTime Started { get; set; }
            public string OrganizerLastName { get; set; }
            public string OrganizerFirstName { get; set; }
            public int OrganizerId { get; set; }
            public string SecondaryOrganizerLastName { get; set; }
            public string SecondaryOrganizerFirstName { get; set; }
            public int? SecondaryOrganizerId { get; set; }
            public int Capacity { get; set; }
            public int FillCapacity { get; set; }
            public string Lecturer { get; set; }
            public bool Private { get; set; }

            public string OrganizerFullName
            {
                get { return String.Format("{0} {1}", OrganizerLastName, OrganizerFirstName); }
            }

            public string SecondaryOrganizerFullName
            {
                get
                {
                    return SecondaryOrganizerId.HasValue
                               ? String.Format("{0} {1}", SecondaryOrganizerLastName, SecondaryOrganizerFirstName)
                               : String.Empty;
                }
            }

            public string PrimaryAndSecondaryOrganizerFullName
            {
                get
                {
                    return !SecondaryOrganizerId.HasValue
                               ? OrganizerFullName
                               : String.Format("{0}, {1}", OrganizerFullName, SecondaryOrganizerFullName);
                }
            }
        }

        public interface IMeetingCache
        {
        }

        public class MeetingCacheBusinessInfo : MeetingCacheBase, IMeetingCache
        {
            public string City { get; set; }
            public string AddressLine1 { get; set; }
        }

        public class MeetingCacheWebinar : MeetingCacheBase, IMeetingCache
        {
        }

        public class MeetingCacheMspEvening : MeetingCacheBase, IMeetingCache
        {
            public string City { get; set; }
            public string AddressLine1 { get; set; }
        }

        public class MeetingCacheSetkaniTymu : MeetingCacheBase, IMeetingCache
        {
            public string City { get; set; }
            public string AddressLine1 { get; set; }
            public string MeetingTitleType { get; set; }
            public int? Price { get; set; }
            public CurrencyType? CurrencyType { get; set; }
        }

        public class MeetingCacheSkoleniDavidaKotaska : MeetingCacheBase, IMeetingCache
        {
            public string City { get; set; }
            public string AddressLine1 { get; set; }
            public string Title { get; set; }
            public int? Price { get; set; }
            public CurrencyType? CurrencyType { get; set; }
        }

        public class MeetingCacheOstatni : MeetingCacheBase, IMeetingCache
        {
            public string City { get; set; }
            public string AddressLine1 { get; set; }
            public string MeetingTitleType { get; set; }
            public int? Price { get; set; }
            public CurrencyType? CurrencyType { get; set; }
        }

        public static Meeting[] GetIndex(DefaultContext db, int bankAccountId)
        {
            IQueryable<Meeting> meetings = db.Meetings.Where(m => m.BankAccountId == bankAccountId);
            return meetings.ToArray();
        }

        public static IEnumerable<IMeetingCache> GetIndexForAdmin(DefaultContext db, MeetingType meetingType)
        {
            switch (meetingType)
            {
                case MeetingType.Lgs:
                    MeetingCacheBusinessInfo[] meetingCacheBusinessInfos = db.Database.SqlQuery<MeetingCacheBusinessInfo>(String.Format(GetMeetingIndexProcedureTemplate, (int)MeetingType.Lgs)).ToArray();
                    return meetingCacheBusinessInfos;

                case MeetingType.Webinar:
                    MeetingCacheWebinar[] meetingCacheWebinars = db.Database.SqlQuery<MeetingCacheWebinar>(String.Format(GetMeetingIndexProcedureTemplate, (int)MeetingType.Webinar)).ToArray();
                    return meetingCacheWebinars;

                case MeetingType.MspEvening:
                    MeetingCacheMspEvening[] meetingCacheMspEvenings = db.Database.SqlQuery<MeetingCacheMspEvening>(String.Format(GetMeetingIndexProcedureTemplate, (int)MeetingType.MspEvening)).ToArray();
                    return meetingCacheMspEvenings;

                case MeetingType.SetkaniTymu:
                    MeetingCacheSetkaniTymu[] meetingCacheSetkaniTymu = db.Database.SqlQuery<MeetingCacheSetkaniTymu>(String.Format(GetMeetingIndexProcedureTemplate, (int)MeetingType.SetkaniTymu)).ToArray();
                    return meetingCacheSetkaniTymu;

                case MeetingType.SkoleniDavidaKotaska:
                    MeetingCacheSkoleniDavidaKotaska[] meetingCacheSkoleniDavidaKotaska = db.Database.SqlQuery<MeetingCacheSkoleniDavidaKotaska>(String.Format(GetMeetingIndexProcedureTemplate, (int)MeetingType.SkoleniDavidaKotaska)).ToArray();
                    return meetingCacheSkoleniDavidaKotaska;

                case MeetingType.Ostatni:
                    MeetingCacheOstatni[] meetingCacheOstatnis = db.Database.SqlQuery<MeetingCacheOstatni>(String.Format(GetMeetingIndexProcedureTemplate, (int)MeetingType.Ostatni)).ToArray();
                    return meetingCacheOstatnis;

                default:
                    throw new ArgumentOutOfRangeException("meetingType");
            }
        }

        public static IEnumerable<IMeetingCache> GetIndexForUser(DefaultContext db, MeetingType meetingType, int userId, bool showAll)
        {
            string procedureTemplate = showAll ? GetMeetingIndexForUserProcedureTemplate : GetMeetingFilteredIndexProcedureTemplate;
            switch (meetingType)
            {
                case MeetingType.Lgs:
                    MeetingCacheBusinessInfo[] meetingCacheBusinessInfos = db.Database.SqlQuery<MeetingCacheBusinessInfo>(String.Format(procedureTemplate, (int)MeetingType.Lgs, userId)).ToArray();
                    return meetingCacheBusinessInfos;

                case MeetingType.Webinar:
                    MeetingCacheWebinar[] meetingCacheWebinars = db.Database.SqlQuery<MeetingCacheWebinar>(String.Format(procedureTemplate, (int)MeetingType.Webinar, userId)).ToArray();
                    return meetingCacheWebinars;

                case MeetingType.MspEvening:
                    MeetingCacheMspEvening[] meetingCacheMspEvenings = db.Database.SqlQuery<MeetingCacheMspEvening>(String.Format(procedureTemplate, (int)MeetingType.MspEvening, userId)).ToArray();
                    return meetingCacheMspEvenings;

                case MeetingType.SetkaniTymu:
                    MeetingCacheSetkaniTymu[] meetingCacheSetkaniTymu = db.Database.SqlQuery<MeetingCacheSetkaniTymu>(String.Format(procedureTemplate, (int)MeetingType.SetkaniTymu, userId)).ToArray();
                    return meetingCacheSetkaniTymu;

                case MeetingType.SkoleniDavidaKotaska:
                    MeetingCacheSkoleniDavidaKotaska[] meetingCacheSkoleniDavidaKotaska = db.Database.SqlQuery<MeetingCacheSkoleniDavidaKotaska>(String.Format(procedureTemplate, (int)MeetingType.SkoleniDavidaKotaska, userId)).ToArray();
                    return meetingCacheSkoleniDavidaKotaska;

                case MeetingType.Ostatni:
                    MeetingCacheOstatni[] meetingCacheOstatnis = db.Database.SqlQuery<MeetingCacheOstatni>(String.Format(procedureTemplate, (int)MeetingType.Ostatni, userId)).ToArray();
                    return meetingCacheOstatnis;

                default:
                    throw new ArgumentOutOfRangeException("meetingType");
            }
        }

        public static Meeting GetDetail(DefaultContext db, int id)
        {
            Meeting meeting = db.Meetings.Find(id);
            return meeting;
        }

        public static IEnumerable<IMeetingCache> GetArchiveIndex(DefaultContext db, MeetingType meetingType, bool isAdmin, int userId)
        {
            switch (meetingType)
            {
                case MeetingType.Lgs:
                    MeetingCacheBusinessInfo[] meetingCacheBusinessInfos = isAdmin
                        ? db.Database.SqlQuery<MeetingCacheBusinessInfo>(String.Format(GetMeetingArchiveIndexProcedureTemplate, (int)MeetingType.Lgs)).ToArray()
                        : db.Database.SqlQuery<MeetingCacheBusinessInfo>(String.Format(GetMeetingFilteredArchiveIndexProcedureTemplate, (int)MeetingType.Lgs, userId)).ToArray();
                    return meetingCacheBusinessInfos;

                case MeetingType.Webinar:
                    MeetingCacheWebinar[] meetingCacheWebinars = isAdmin
                        ? db.Database.SqlQuery<MeetingCacheWebinar>(String.Format(GetMeetingArchiveIndexProcedureTemplate, (int)MeetingType.Webinar)).ToArray()
                        : db.Database.SqlQuery<MeetingCacheWebinar>(String.Format(GetMeetingFilteredArchiveIndexProcedureTemplate, (int)MeetingType.Webinar, userId)).ToArray();
                    return meetingCacheWebinars;

                case MeetingType.MspEvening:
                    MeetingCacheMspEvening[] meetingCacheMspEvenings = isAdmin
                        ? db.Database.SqlQuery<MeetingCacheMspEvening>(String.Format(GetMeetingArchiveIndexProcedureTemplate, (int)MeetingType.MspEvening)).ToArray()
                        : db.Database.SqlQuery<MeetingCacheMspEvening>(String.Format(GetMeetingFilteredArchiveIndexProcedureTemplate, (int)MeetingType.MspEvening, userId)).ToArray();
                    return meetingCacheMspEvenings;

                case MeetingType.SetkaniTymu:
                    MeetingCacheSetkaniTymu[] meetingCacheSetkaniTymu = isAdmin
                        ? db.Database.SqlQuery<MeetingCacheSetkaniTymu>(String.Format(GetMeetingArchiveIndexProcedureTemplate, (int)MeetingType.SetkaniTymu)).ToArray()
                        : db.Database.SqlQuery<MeetingCacheSetkaniTymu>(String.Format(GetMeetingFilteredArchiveIndexProcedureTemplate, (int)MeetingType.SetkaniTymu, userId)).ToArray();
                    return meetingCacheSetkaniTymu;

                case MeetingType.SkoleniDavidaKotaska:
                    MeetingCacheSkoleniDavidaKotaska[] meetingCacheSkoleniDavidaKotaska = isAdmin
                        ? db.Database.SqlQuery<MeetingCacheSkoleniDavidaKotaska>(String.Format(GetMeetingArchiveIndexProcedureTemplate, (int)MeetingType.SkoleniDavidaKotaska)).ToArray()
                        : db.Database.SqlQuery<MeetingCacheSkoleniDavidaKotaska>(String.Format(GetMeetingFilteredArchiveIndexProcedureTemplate, (int)MeetingType.SkoleniDavidaKotaska, userId)).ToArray();
                    return meetingCacheSkoleniDavidaKotaska;

                case MeetingType.Ostatni:
                    MeetingCacheOstatni[] meetingCacheOstatnis = isAdmin
                        ? db.Database.SqlQuery<MeetingCacheOstatni>(String.Format(GetMeetingArchiveIndexProcedureTemplate, (int)MeetingType.Ostatni)).ToArray()
                        : db.Database.SqlQuery<MeetingCacheOstatni>(String.Format(GetMeetingFilteredArchiveIndexProcedureTemplate, (int)MeetingType.Ostatni, userId)).ToArray();
                    return meetingCacheOstatnis;

                default:
                    throw new ArgumentOutOfRangeException("meetingType");
            }
        }

        public static void Insert(DefaultContext db, MeetingType meetingType, int userId, IMeetingEdit iMeetingEdit)
        {
            Meeting meeting;
            switch (meetingType)
            {
                case MeetingType.Lgs:
                    meeting = ((MeetingBusinessInfoEdit)iMeetingEdit).GetModel(userId);
                    break;

                case MeetingType.Webinar:
                    meeting = ((MeetingWebinarEdit)iMeetingEdit).GetModel(userId);
                    break;

                case MeetingType.MspEvening:
                    meeting = ((MeetingMspEveningEdit)iMeetingEdit).GetModel(userId);
                    break;

                case MeetingType.SetkaniTymu:
                    meeting = ((MeetingSetkaniTymuEdit)iMeetingEdit).GetModel(userId);
                    break;

                case MeetingType.SkoleniDavidaKotaska:
                    meeting = ((MeetingSkoleniDavidaKotaskaEdit)iMeetingEdit).GetModel(userId);
                    break;

                case MeetingType.Ostatni:
                    meeting = ((MeetingOstatniEdit)iMeetingEdit).GetModel(db, userId);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("meetingType");
            }

            db.Meetings.Add(meeting);
            TrySaveChanges(db);
        }

        public static bool Update(DefaultContext db, ref Meeting meeting)
        {
            Meeting dbMeeting = GetDetail(db, meeting.MeetingId);
            if (dbMeeting == null)
                return false;

            dbMeeting.CopyFrom(meeting);
            db.SaveChanges();

            meeting = dbMeeting;
            return true;
        }

        public static DeleteResult Delete(DefaultContext db, int id, out Meeting meeting)
        {
            meeting = GetDetail(db, id);
            if (meeting == null)
                return DeleteResult.AuthorizationFailed;

            try
            {
                var parameter = new SqlParameter(MeetingIdSqlParameter, meeting.MeetingId);
                db.Database.ExecuteSqlCommand(CascadeRemoveMeetingProcedureTemplate, parameter);
                return DeleteResult.Ok;
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
                return DeleteResult.DbFailed;
            }
        }

        public static void ProcessSearchingAndSorting(ref IEnumerable<MeetingCacheBusinessInfo> meetings, string searchString, string searchStringAccording, string sortOrder)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchStringAccording)
                {
                    case CityField:
                        meetings = meetings.Where(m => m.City.ToUpper().Contains(searchString.ToUpper()));
                        break;

                    case OrganizerField:
                        meetings = meetings.Where(m => m.PrimaryAndSecondaryOrganizerFullName.ToUpper().Contains(searchString.ToUpper()));
                        break;
                }
            }

            switch (sortOrder)
            {
                case CityDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.City);
                    break;

                case CityAscSortOrder:
                    meetings = meetings.OrderBy(m => m.City);
                    break;

                case DateDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.Started);
                    break;

                case AddressLine1DescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.AddressLine1);
                    break;

                case AddressLine1AscSortOrder:
                    meetings = meetings.OrderBy(m => m.AddressLine1);
                    break;

                case OrganizerDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.PrimaryAndSecondaryOrganizerFullName);
                    break;

                case OrganizerAscSortOrder:
                    meetings = meetings.OrderBy(m => m.PrimaryAndSecondaryOrganizerFullName);
                    break;

                default:
                    meetings = meetings.OrderBy(m => m.Started);
                    break;
            }
        }

        public static void ProcessSearchingAndSorting(ref IEnumerable<MeetingCacheWebinar> meetings, string searchString, string searchStringAccording, string sortOrder)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchStringAccording)
                {
                    case OrganizerField:
                        meetings = meetings.Where(m => m.PrimaryAndSecondaryOrganizerFullName.ToUpper().Contains(searchString.ToUpper()));
                        break;
                }
            }

            switch (sortOrder)
            {
                case DateDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.Started);
                    break;

                case OrganizerDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.PrimaryAndSecondaryOrganizerFullName);
                    break;

                case OrganizerAscSortOrder:
                    meetings = meetings.OrderBy(m => m.PrimaryAndSecondaryOrganizerFullName);
                    break;

                default:
                    meetings = meetings.OrderBy(m => m.Started);
                    break;
            }
        }

        public static void ProcessSearchingAndSorting(ref IEnumerable<MeetingCacheMspEvening> meetings, string searchString, string searchStringAccording, string sortOrder)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchStringAccording)
                {
                    case CityField:
                        meetings = meetings.Where(m => m.City.ToUpper().Contains(searchString.ToUpper()));
                        break;

                    case OrganizerField:
                        meetings =
                            meetings.Where(
                                m => m.PrimaryAndSecondaryOrganizerFullName.ToUpper().Contains(searchString.ToUpper()));
                        break;
                }
            }

            switch (sortOrder)
            {
                case CityDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.City);
                    break;

                case CityAscSortOrder:
                    meetings = meetings.OrderBy(m => m.City);
                    break;

                case DateDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.Started);
                    break;

                case AddressLine1DescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.AddressLine1);
                    break;

                case AddressLine1AscSortOrder:
                    meetings = meetings.OrderBy(m => m.AddressLine1);
                    break;

                case OrganizerDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.PrimaryAndSecondaryOrganizerFullName);
                    break;

                case OrganizerAscSortOrder:
                    meetings = meetings.OrderBy(m => m.PrimaryAndSecondaryOrganizerFullName);
                    break;

                default:
                    meetings = meetings.OrderBy(m => m.Started);
                    break;
            }
        }

        public static void ProcessSearchingAndSorting(ref IEnumerable<MeetingCacheSetkaniTymu> meetings, string searchString, string searchStringAccording, string sortOrder)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchStringAccording)
                {
                    case CityField:
                        meetings = meetings.Where(m => m.City.ToUpper().Contains(searchString.ToUpper()));
                        break;

                    case OrganizerField:
                        meetings =
                            meetings.Where(
                                m => m.PrimaryAndSecondaryOrganizerFullName.ToUpper().Contains(searchString.ToUpper()));
                        break;
                }
            }

            switch (sortOrder)
            {
                case MeetingTitleTypeDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.MeetingTitleType);
                    break;

                case MeetingTitleTypeAscSortOrder:
                    meetings = meetings.OrderBy(m => m.MeetingTitleType);
                    break;

                case CityDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.City);
                    break;

                case CityAscSortOrder:
                    meetings = meetings.OrderBy(m => m.City);
                    break;

                case DateDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.Started);
                    break;

                case AddressLine1DescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.AddressLine1);
                    break;

                case AddressLine1AscSortOrder:
                    meetings = meetings.OrderBy(m => m.AddressLine1);
                    break;

                case OrganizerDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.PrimaryAndSecondaryOrganizerFullName);
                    break;

                case OrganizerAscSortOrder:
                    meetings = meetings.OrderBy(m => m.PrimaryAndSecondaryOrganizerFullName);
                    break;

                default:
                    meetings = meetings.OrderBy(m => m.Started);
                    break;
            }
        }

        public static void ProcessSearchingAndSorting(ref IEnumerable<MeetingCacheSkoleniDavidaKotaska> meetings, string searchString, string searchStringAccording, string sortOrder)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchStringAccording)
                {
                    case CityField:
                        meetings = meetings.Where(m => m.City.ToUpper().Contains(searchString.ToUpper()));
                        break;

                    case OrganizerField:
                        meetings =
                            meetings.Where(
                                m => m.PrimaryAndSecondaryOrganizerFullName.ToUpper().Contains(searchString.ToUpper()));
                        break;
                }
            }

            switch (sortOrder)
            {
                case CityDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.City);
                    break;

                case CityAscSortOrder:
                    meetings = meetings.OrderBy(m => m.City);
                    break;

                case DateDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.Started);
                    break;

                case AddressLine1DescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.AddressLine1);
                    break;

                case AddressLine1AscSortOrder:
                    meetings = meetings.OrderBy(m => m.AddressLine1);
                    break;

                case OrganizerDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.PrimaryAndSecondaryOrganizerFullName);
                    break;

                case OrganizerAscSortOrder:
                    meetings = meetings.OrderBy(m => m.PrimaryAndSecondaryOrganizerFullName);
                    break;

                default:
                    meetings = meetings.OrderBy(m => m.Started);
                    break;
            }
        }

        public static void ProcessSearchingAndSorting(ref IEnumerable<MeetingCacheOstatni> meetings, string searchString, string searchStringAccording, string sortOrder)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchStringAccording)
                {
                    case CityField:
                        meetings = meetings.Where(m => m.City.ToUpper().Contains(searchString.ToUpper()));
                        break;

                    case OrganizerField:
                        meetings =
                            meetings.Where(
                                m => m.PrimaryAndSecondaryOrganizerFullName.ToUpper().Contains(searchString.ToUpper()));
                        break;
                }
            }

            switch (sortOrder)
            {
                case MeetingTitleTypeDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.MeetingTitleType);
                    break;

                case MeetingTitleTypeAscSortOrder:
                    meetings = meetings.OrderBy(m => m.MeetingTitleType);
                    break;

                case CityDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.City);
                    break;

                case CityAscSortOrder:
                    meetings = meetings.OrderBy(m => m.City);
                    break;

                case DateDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.Started);
                    break;

                case AddressLine1DescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.AddressLine1);
                    break;

                case AddressLine1AscSortOrder:
                    meetings = meetings.OrderBy(m => m.AddressLine1);
                    break;

                case OrganizerDescSortOrder:
                    meetings = meetings.OrderByDescending(m => m.PrimaryAndSecondaryOrganizerFullName);
                    break;

                case OrganizerAscSortOrder:
                    meetings = meetings.OrderBy(m => m.PrimaryAndSecondaryOrganizerFullName);
                    break;

                default:
                    meetings = meetings.OrderBy(m => m.Started);
                    break;
            }
        }

        public static void ProcessSearchingAndSorting(ref List<MeetingSignUnsignAttendee> attendees, ref List<MeetingSignUnsignAttendee> signedAttendees, string searchString, string searchStringAccording, string sortOrder, string signedSortOrder)
        {
            IEnumerable<MeetingSignUnsignAttendee> newAttendees = attendees;
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchStringAccording)
                {
                    case LastNameField:
                        newAttendees = newAttendees.Where(a => a.LastName.ToUpper().Contains(searchString.ToUpper()));
                        break;

                    case CityField:
                        newAttendees = newAttendees.Where(m => m.City.ToUpper().Contains(searchString.ToUpper()));
                        break;
                }
            }

            switch (sortOrder)
            {
                case LastNameDescSortOrder:
                    newAttendees = newAttendees.OrderByDescending(a => a.LastName);
                    break;

                case FirstNameAscSortOrder:
                    newAttendees = newAttendees.OrderBy(a => a.FirstName);
                    break;

                case FirstNameDescSortOrder:
                    newAttendees = newAttendees.OrderByDescending(a => a.FirstName);
                    break;

                case CityAscSortOrder:
                    newAttendees = newAttendees.OrderBy(a => a.City);
                    break;

                case CityDescSortOrder:
                    newAttendees = newAttendees.OrderByDescending(a => a.City);
                    break;

                default:
                    newAttendees = newAttendees.OrderBy(a => a.LastName);
                    break;
            }

            attendees = newAttendees.ToList();

            IEnumerable<MeetingSignUnsignAttendee> newSignedAttendees = signedAttendees;
            switch (signedSortOrder)
            {
                case LastNameDescSortOrder:
                    newSignedAttendees = newSignedAttendees.OrderByDescending(a => a.LastName);
                    break;

                case FirstNameAscSortOrder:
                    newSignedAttendees = newSignedAttendees.OrderBy(a => a.FirstName);
                    break;

                case FirstNameDescSortOrder:
                    newSignedAttendees = newSignedAttendees.OrderByDescending(a => a.FirstName);
                    break;

                case CityAscSortOrder:
                    newSignedAttendees = newSignedAttendees.OrderBy(a => a.City);
                    break;

                case CityDescSortOrder:
                    newSignedAttendees = newSignedAttendees.OrderByDescending(a => a.City);
                    break;

                default:
                    newSignedAttendees = newSignedAttendees.OrderBy(a => a.LastName);
                    break;
            }

            signedAttendees = newSignedAttendees.ToList();
        }

        public static MeetingDetailAttendee[] ProcessSorting(MeetingDetailAttendee[] meetingDetailAttendees, string sortOrder)
        {
            MeetingDetailAttendee[] sortedMeetingDetailAttendees;
            switch (sortOrder)
            {
                case LastNameDescSortOrder:
                    sortedMeetingDetailAttendees = meetingDetailAttendees.OrderByDescending(m => m.LastName).ToArray();
                    break;

                case LyonessIdDescSortOrder:
                    sortedMeetingDetailAttendees = meetingDetailAttendees.OrderByDescending(m => m.LyonessId).ToArray();
                    break;

                case LyonessIdAscSortOrder:
                    sortedMeetingDetailAttendees = meetingDetailAttendees.OrderBy(m => m.LyonessId).ToArray();
                    break;

                default:
                    sortedMeetingDetailAttendees = meetingDetailAttendees.OrderBy(tt => tt.LastName).ToArray();
                    break;
            }

            return sortedMeetingDetailAttendees;
        }

        public static void CascadeRemoveArchivedMeeting(DefaultContext db)
        {
            db.Database.ExecuteSqlCommand(CascadeRemoveArchivedMeetingTemplate);
        }
    }
}