using System.Collections.Generic;
using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace LBT.Cache
{
    public class SharedContactsCache : BaseCache
    {
        public static SharedContact[] GetIndex(DefaultContext db, int userId)
        {
            IQueryable<SharedContact> sharedContacts = db.SharedContacts.Include(s => s.FromUser).Include(s => s.ToUser).Where(s => s.FromUserId == userId).OrderBy(s => s.ToUser.UserName);
            return sharedContacts.ToArray();
        }

        public static SharedContact GetDetail(DefaultContext db, int sharedContactId)
        {
            SharedContact sharedContact = db.SharedContacts.Find(sharedContactId);
            return sharedContact;
        }

        public static void Insert(DefaultContext db, SharedContact sharedContact)
        {
            db.SharedContacts.Add(sharedContact);
            db.SaveChanges();
        }

        public static DeleteResult Delete(DefaultContext db, int sharedContactId, out SharedContact sharedContact)
        {
            sharedContact = GetDetail(db, sharedContactId);
            if (sharedContact == null)
                return DeleteResult.AuthorizationFailed;

            try
            {
                db.SharedContacts.Remove(sharedContact);
                db.SaveChanges();
                return DeleteResult.Ok;
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
                return DeleteResult.DbFailed;
            }
        }

        public class PopulateDownlineUser
        {
            public static IEnumerable<PopulateDownlineUser> GetPopulateUplineUsers(DefaultContext db, int userId)
            {
                IEnumerable<PopulateDownlineUser> populateDownlineUsers = db.Database.SqlQuery<PopulateDownlineUser>(String.Format(GetPopulateDownlineUsersProcedureTemplate, userId));
                return populateDownlineUsers;
            }

            private PopulateDownlineUser()
            {
            }

            public int UserId { get; set; }

            public string Title { get; set; }
        }
    }
}