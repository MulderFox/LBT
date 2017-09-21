// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 05-08-2014
//
// Last Modified By : zmikeska
// Last Modified On : 05-08-2014
// ***********************************************************************
// <copyright file="Statistics.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.ComponentModel.DataAnnotations.Schema;

namespace LBT.Models
{
    /// <summary>
    /// Enum StatisticsGroup
    /// </summary>
    public enum StatisticsGroup
    {
        /// <summary>
        /// All
        /// </summary>
        All = 0,
        /// <summary>
        /// The owner
        /// </summary>
        Owner = 1,
        /// <summary>
        /// The downline
        /// </summary>
        Downline = 2,
        /// <summary>
        /// The leader downline
        /// </summary>
        LeaderDownline = 3
    }

    public class Statistics
    {
        public int StatisticsId { get; set; }

        [ForeignKey("UserProfile")]
        public int? UserId { get; set; }

        [Column("UserId")]
        public virtual UserProfile UserProfile { get; set; }

        public StatisticsGroup StatisticsGroup { get; set; }

        public decimal? RegistredPeopleQuota { get; set; }

        public decimal? RegistredPeopleQuotaLastMonth { get; set; }

        public decimal? PremiumPartnersQuota { get; set; }

        public decimal? PremiumPartnersQuotaLastMonth { get; set; }

        public decimal? BuyersQuota { get; set; }

        public int? ContactedPeopleCount { get; set; }

        public int? ContactedPeopleCountLastMonth { get; set; }
    }
}