using LBT.DAL;
using LBT.Models;
using System.Linq;

namespace LBT.Cache
{
    public class StatisticsCache : BaseCache
    {
        public static Statistics[] GetIndex(DefaultContext db, int userId)
        {
            Statistics[] statistics = db.Statistics.Where(s => s.UserId == null || s.UserId == userId).ToArray();
            return statistics;
        }

        public static void RefreshStatistics(DefaultContext db)
        {
            db.Database.ExecuteSqlCommand(RefreshStatisticsProcedureTemplate);
        }
    }
}