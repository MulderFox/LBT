using System;
using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using System.Linq;

namespace LBT.Cache
{
    public class DistrictCache : BaseCache
    {
        public static District[] GetIndex(DefaultContext db)
        {
            IQueryable<District> districts = db.Districts.OrderBy(d => d.Title);
            return districts.ToArray();
        }

        public static void Insert(DefaultContext db, District district)
        {
            db.Districts.Add(district);
            db.SaveChanges();
        }

        public static District GetDetail(DefaultContext db, int id)
        {
            District district = db.Districts.Find(id);
            return district;
        }

        public static bool Update(DefaultContext db, ref District district)
        {
            District dbDistrict = GetDetail(db, district.DistrictId);
            if (dbDistrict == null)
                return false;

            dbDistrict.CopyFrom(district);
            db.SaveChanges();

            district = dbDistrict;
            return true;
        }

        public static DeleteResult Delete(DefaultContext db, int id, out District district)
        {
            district = GetDetail(db, id);
            if (district == null)
                return DeleteResult.AuthorizationFailed;

            try
            {
                db.Districts.Remove(district);
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