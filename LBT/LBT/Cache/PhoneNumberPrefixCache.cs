using LBT.DAL;
using LBT.Models;
using System.Linq;

namespace LBT.Cache
{
    public class PhoneNumberPrefixCache : BaseCache
    {
        public static IQueryable<PhoneNumberPrefix> GetIndex(DefaultContext db)
        {
            IQueryable<PhoneNumberPrefix> phoneNumberPrefixes = db.PhoneNumberPrefixes.OrderBy(pnp => pnp.Title);
            return phoneNumberPrefixes;
        }
    }
}