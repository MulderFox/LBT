// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 12-20-2013
//
// Last Modified By : zmikeska
// Last Modified On : 01-10-2014
// ***********************************************************************
// <copyright file="HomeController.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.Filters;
using LBT.ModelViews;
using System.Web.Mvc;

namespace LBT.Controllers
{
    /// <summary>
    /// Class HomeController
    /// </summary>
    [InitializeSimpleMembership]
    [Authorize]
    public class HomeController : BaseController
    {
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Index()
        {
            ManualDashboard manualDashboard = ManualDashboard.GetViewModel(this);
            if (manualDashboard == null)
                return RedirectToAccessDenied();

            return View(manualDashboard);
        }

        /// <summary>
        /// Abouts this instance.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [OutputCache(Duration = 3600)]
        public ActionResult About()
        {
            return View();
        }
    }
}
