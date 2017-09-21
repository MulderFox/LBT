using LBT.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LBT.Resources;

namespace LBT.ModelViews
{
    public class StatisticsIndex : BaseModelView
    {
        public StatisticsDetails Owner { get; set; }

        public StatisticsDetails Downline { get; set; }

        public StatisticsDetails LeaderDownline { get; set; }

        public StatisticsDetails All { get; set; }

        private StatisticsIndex(Statistics[] statistics)
        {
            All = StatisticsDetails.GetModelView(statistics.FirstOrDefault(s => s.StatisticsGroup == StatisticsGroup.All));
            Downline = StatisticsDetails.GetModelView(statistics.FirstOrDefault(s => s.StatisticsGroup == StatisticsGroup.Downline));
            LeaderDownline = StatisticsDetails.GetModelView(statistics.FirstOrDefault(s => s.StatisticsGroup == StatisticsGroup.LeaderDownline));
            Owner = StatisticsDetails.GetModelView(statistics.FirstOrDefault(s => s.StatisticsGroup == StatisticsGroup.Owner));
        }

        public static StatisticsIndex GetModelView(Statistics[] statistics)
        {
            if (statistics == null || statistics.Length == 0)
                return null;

            var statisticsIndex = new StatisticsIndex(statistics);
            return statisticsIndex;
        }
    }

    public class StatisticsDetails : BaseModelView
    {
        public int? UserId { get; set; }

        [Display(Name = "Global_RegistredPeopleQuota_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(HtmlEncode = false, NullDisplayText = NullDisplayTextForInteger)]
        public decimal? RegistredPeopleQuota { get; set; }

        [Display(Name = "Global_RegistredPeopleQuotaLastMonth_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(HtmlEncode = false, NullDisplayText = NullDisplayTextForInteger)]
        public decimal? RegistredPeopleQuotaLastMonth { get; set; }

        [Display(Name = "Global_PremiumPartnersQuota_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(HtmlEncode = false, NullDisplayText = NullDisplayTextForInteger)]
        public decimal? PremiumPartnersQuota { get; set; }

        [Display(Name = "Global_PremiumPartnersQuotaLastMonth_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(HtmlEncode = false, NullDisplayText = NullDisplayTextForInteger)]
        public decimal? PremiumPartnersQuotaLastMonth { get; set; }

        [Display(Name = "Global_BuyersQuota_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(HtmlEncode = false, NullDisplayText = NullDisplayTextForInteger)]
        public decimal? BuyersQuota { get; set; }

        [Display(Name = "Global_ContactedPeopleCount_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(HtmlEncode = false, NullDisplayText = NullDisplayTextForInteger)]
        public int? ContactedPeopleCount { get; set; }

        [Display(Name = "Global_ContactedPeopleCountLastMonth_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(HtmlEncode = false, NullDisplayText = NullDisplayTextForInteger)]
        public int? ContactedPeopleCountLastMonth { get; set; }

        private StatisticsDetails(Statistics statistics)
        {
            UserId = statistics.UserId;
            RegistredPeopleQuota = statistics.RegistredPeopleQuota;
            RegistredPeopleQuotaLastMonth = statistics.RegistredPeopleQuotaLastMonth;
            PremiumPartnersQuota = statistics.PremiumPartnersQuota;
            PremiumPartnersQuotaLastMonth = statistics.PremiumPartnersQuotaLastMonth;
            BuyersQuota = statistics.BuyersQuota;
            ContactedPeopleCount = statistics.ContactedPeopleCount;
            ContactedPeopleCountLastMonth = statistics.ContactedPeopleCountLastMonth;
        }

        public static StatisticsDetails GetModelView(Statistics statistics)
        {
            if (statistics == null)
                return null;

            var statisticsDetails = new StatisticsDetails(statistics);
            return statisticsDetails;
        }
    }
}