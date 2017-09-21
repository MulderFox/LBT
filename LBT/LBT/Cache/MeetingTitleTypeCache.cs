using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using System;
using System.Linq;

namespace LBT.Cache
{
    public class MeetingTitleTypeCache : BaseCache
    {
        public static MeetingTitleType[] GetIndex(DefaultContext db, string sortOrder)
        {
            IQueryable<MeetingTitleType> meetingTitleTypes = db.MeetingTitleTypes.AsQueryable();

            switch (sortOrder)
            {
                default:
                    meetingTitleTypes = meetingTitleTypes.OrderBy(ba => ba.Title);
                    break;

                case TitleDescSortOrder:
                    meetingTitleTypes = meetingTitleTypes.OrderByDescending(ba => ba.Title);
                    break;

                case MeetingTypeAscSortOrder:
                    meetingTitleTypes = meetingTitleTypes.OrderBy(ba => ba.MeetingType);
                    break;

                case MeetingTypeDescSortOrder:
                    meetingTitleTypes = meetingTitleTypes.OrderByDescending(ba => ba.MeetingType);
                    break;
            }

            return meetingTitleTypes.ToArray();
        }

        public static void Insert(DefaultContext db, MeetingTitleType meetingTitleType)
        {
            db.MeetingTitleTypes.Add(meetingTitleType);
            db.SaveChanges();
        }

        public static MeetingTitleType GetDetail(DefaultContext db, int id)
        {
            MeetingTitleType meetingTitleType = db.MeetingTitleTypes.Find(id);
            return meetingTitleType;
        }

        public static bool Update(DefaultContext db, ref MeetingTitleType meetingTitleType)
        {
            MeetingTitleType dbMeetingTitleType = GetDetail(db, meetingTitleType.MeetingTitleTypeId);
            if (dbMeetingTitleType == null)
                return false;

            dbMeetingTitleType.CopyFrom(meetingTitleType);
            db.SaveChanges();

            meetingTitleType = dbMeetingTitleType;
            return true;
        }

        public static DeleteResult Delete(DefaultContext db, int id, out MeetingTitleType meetingTitleType)
        {
            meetingTitleType = GetDetail(db, id);
            if (meetingTitleType == null)
                return DeleteResult.AuthorizationFailed;

            // Pokud je název akcí součástí akcí, nesmí se smazat
            if (db.Meetings.Any(m => m.MeetingTitleTypeId == id))
                return DeleteResult.UnlinkFailed;

            try
            {
                db.MeetingTitleTypes.Remove(meetingTitleType);
                db.SaveChanges();
                return DeleteResult.Ok;
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
                return DeleteResult.DbFailed;
            }
        }
    }
}