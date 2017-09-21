using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using System;
using System.Linq;

namespace LBT.Cache
{
    public sealed class ManualTypeCache : BaseCache
    {
        public static ManualType[] GetIndex(DefaultContext db)
        {
            IQueryable<ManualType> manualTypes = db.ManualTypes.OrderBy(mt => mt.Order);
            return manualTypes.ToArray();
        }

        public static void Insert(DefaultContext db, ManualType manualType)
        {
            db.ManualTypes.Add(manualType);
            db.SaveChanges();
        }

        public static ManualType GetDetail(DefaultContext db, int id)
        {
            ManualType manualType = db.ManualTypes.Find(id);
            return manualType;
        }

        public static bool Update(DefaultContext db, ManualType manualType)
        {
            int manualTypeId = manualType.ManualTypeId;
            ManualType dbManualType = GetDetail(db, manualTypeId);
            if (dbManualType == null)
                return false;

            dbManualType.CopyFrom(manualType);

            db.SaveChanges();

            return true;
        }

        public static DeleteResult Delete(DefaultContext db, ManualType manualType)
        {
            if (manualType == null)
                return DeleteResult.AuthorizationFailed;

            try
            {
                db.ManualTypes.Remove(manualType);
                db.SaveChanges();

                return DeleteResult.Ok;
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
                return DeleteResult.DbFailed;
            }
        }

        public static bool IsTitleUnique(DefaultContext db, string title, int? modelTypeId)
        {
            int dbModelTypeId = modelTypeId.GetValueOrDefault();
            bool isUnique = !db.ManualTypes.Any(mt => mt.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase) && mt.ManualTypeId != dbModelTypeId);
            return isUnique;
        }
    }
}