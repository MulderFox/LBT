using System;
using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using System.Linq;

namespace LBT.Cache
{
    public class CurrencyCache : BaseCache
    {
        public static Currency[] GetIndex(DefaultContext db, int exceptCurrencyId)
        {
            IQueryable<Currency> currencies = db.Currencies.Where(ba => ba.CurrencyId != exceptCurrencyId);
            return currencies.ToArray();
        }

        public static Currency[] GetIndex(DefaultContext db)
        {
            IQueryable<Currency> currencies = db.Currencies.AsQueryable();
            return currencies.ToArray();
        }

        public static void Insert(DefaultContext db, Currency currency)
        {
            db.Currencies.Add(currency);
            db.SaveChanges();
        }

        public static Currency GetDetail(DefaultContext db, int id)
        {
            Currency currency = db.Currencies.Find(id);
            return currency;
        }

        public static bool Update(DefaultContext db, ref Currency currency)
        {
            Currency dbCurrency = GetDetail(db, currency.CurrencyId);
            if (dbCurrency == null)
                return false;

            dbCurrency.CopyFrom(currency);
            db.SaveChanges();

            currency = dbCurrency;
            return true;
        }

        public static DeleteResult Delete(DefaultContext db, int id, out Currency currency)
        {
            currency = GetDetail(db, id);
            if (currency == null)
                return DeleteResult.AuthorizationFailed;

            try
            {
                db.Currencies.Remove(currency);
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