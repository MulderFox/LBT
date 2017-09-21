// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 05-08-2014
//
// Last Modified By : zmikeska
// Last Modified On : 05-09-2014
// ***********************************************************************
// <copyright file="StatisticsController.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.Cache;
using LBT.Filters;
using LBT.Models;
using LBT.ModelViews;
using System.Web.Mvc;

namespace LBT.Controllers
{
    /// <summary>
    /// Class StatisticsController
    /// </summary>
    [InitializeSimpleMembership]
    [Authorize]
    public class StatisticsController : BaseController
    {
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Index()
        {
            Statistics[] statistics = StatisticsCache.GetIndex(Db, UserId);
            var statisticsIndex = StatisticsIndex.GetModelView(statistics);
            return View(statisticsIndex);
        }
    }
}