using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using System;
using System.Linq;

namespace LBT.Cache
{
    public sealed class ManualCache : BaseCache
    {
        public static Manual[] GetIndex(DefaultContext db)
        {
            IOrderedQueryable<Manual> manuals = db.Manuals.OrderBy(m => m.ManualType.Order).ThenBy(m => m.Order);
            return manuals.ToArray();
        }

        public static void Insert(DefaultContext db, Manual manual)
        {
            db.Manuals.Add(manual);
            db.SaveChanges();
        }

        public static Manual GetDetail(DefaultContext db, int id)
        {
            Manual manual = db.Manuals.Find(id);
            return manual;
        }

        public static bool Update(DefaultContext db, Manual manual)
        {
            int manualId = manual.ManualId;
            Manual dbManual = GetDetail(db, manualId);
            if (dbManual == null)
                return false;

            dbManual.CopyFrom(manual);

            db.SaveChanges();

            return true;
        }

        public static DeleteResult Delete(DefaultContext db, Manual manual)
        {
            if (manual == null)
                return DeleteResult.AuthorizationFailed;

            try
            {
                db.Manuals.Remove(manual);
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