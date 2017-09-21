// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 06-04-2014
//
// Last Modified By : zmikeska
// Last Modified On : 06-05-2014
// ***********************************************************************
// <copyright file="TopTenController.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.Cache;
using LBT.Filters;
using LBT.Helpers;
using LBT.Models;
using LBT.ModelViews;
using LBT.Services.LyconetService;
using PagedList;
using LBT.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace LBT.Controllers
{
    /// <summary>
    /// Class TopTenController
    /// </summary>
    [InitializeSimpleMembership]
    [Authorize(Roles = LeadingRoles)]
    public class TopTenController : BaseController
    {
        private const string ExportFileDownloadName = "Top10.csv";

        //
        // GET: /TopTen/

        /// <summary>
        /// Indexes the specified sort order.
        /// </summary>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="currentFilter">The current filter.</param>
        /// <param name="currentFilterAccording">The current filter according.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="searchStringAccording">The search string according.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Index(string sortOrder, string currentFilter, string currentFilterAccording,
                                  string searchString, string searchStringAccording, int? page, int pageSize = PageSize)
        {
            int pageNumber;
            ProcessSearchFilterAndPaging(currentFilter, currentFilterAccording, ref searchString, ref searchStringAccording, ref page, out pageNumber);

            var sortingNames = new[]
                                   {
                                       BaseCache.LastNameField, BaseCache.FirstNameField, BaseCache.StatusField,
                                       BaseCache.LastContactField, BaseCache.ActualCareerStageField, BaseCache.RoleField,
                                       BaseCache.LyonessIdField
                                   };
            ProcessSorting(sortOrder, sortingNames);

            PopulateSearchStringAccording(searchString, searchStringAccording);
            PopulatePageSize(pageSize);

            IEnumerable<TopTenIndex> topTenIndexRows = TopTenIndex.GetIndex(Db, UserId, searchString, searchStringAccording, sortOrder);
            return View(topTenIndexRows.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /TopTen/Details/5

        /// <summary>
        /// Detailses the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Details(int id = 0)
        {
            TopTen topTen = TopTenCache.GetDetail(Db, id, UserId);
            if (!IsAccess(topTen))
            {
                return RedirectToAccessDenied();
            }

            TopTenDetails topTenDetails = TopTenDetails.GetModelView(topTen);
            return View(topTenDetails);
        }

        //
        // GET: /TopTen/Create

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Create()
        {
            PopulateToUserId();
            PopulateStatus();

            return View();
        }

        //
        // POST: /TopTen/Create

        /// <summary>
        /// Creates the specified top ten.
        /// </summary>
        /// <param name="topTenEdit">The top ten.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TopTenEdit topTenEdit)
        {
            if (ModelState.IsValid)
            {
                TopTen topTen = topTenEdit.GetModel();
                TopTenCache.Insert(Db, UserId, ref topTen);
                return RedirectToAction("Index");
            }

            PopulateToUserId(topTenEdit.ToUserId);
            PopulateStatus(topTenEdit.Status);

            return View(topTenEdit);
        }

        //
        // GET: /TopTen/Edit/5

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Edit(int id = 0)
        {
            TopTen topten = TopTenCache.GetDetail(Db, id, UserId);
            if (!IsAccess(topten))
            {
                return RedirectToAccessDenied();
            }

            TopTenEdit topTenEdit = TopTenEdit.GetModelView(topten);

            PopulateStatus(topten.Status);

            return View(topTenEdit);
        }

        //
        // POST: /TopTen/Edit/5

        /// <summary>
        /// Edits the specified top ten.
        /// </summary>
        /// <param name="topTenEdit">The top ten.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TopTenEdit topTenEdit)
        {
            if (ModelState.IsValid)
            {
                TopTen topTen = topTenEdit.GetModel();
                bool success = TopTenCache.Update(Db, UserId, ref topTen);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("Index");
            }

            return View(topTenEdit);
        }

        //
        // GET: /TopTen/Delete/5

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Delete(int id = 0)
        {
            TopTen topTen = TopTenCache.GetDetail(Db, id, UserId);
            if (!IsAccess(topTen))
            {
                return RedirectToAccessDenied();
            }

            TopTenDelete topTenDelete = TopTenDelete.GetModelView(topTen);

            return View(topTenDelete);
        }

        //
        // POST: /TopTen/Delete/5

        /// <summary>
        /// Deletes the confirmed.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TopTen topTen;
            DeleteResult deleteResult = TopTenCache.Delete(Db, id, UserId, out topTen);
            switch (deleteResult)
            {
                case DeleteResult.Ok:
                    return RedirectToAction("Index");

                case DeleteResult.AuthorizationFailed:
                    return RedirectToAccessDenied();

                case DeleteResult.DbFailed:
                    ModelState.AddModelError(BaseCache.TitleField, ValidationResource.Global_DeleteRecord_ErrorMessage);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            TopTenDelete topTenDelete = TopTenDelete.GetModelView(topTen);

            return View(topTenDelete);
        }

        /// <summary>
        /// Prints the index.
        /// </summary>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="currentFilter">The current filter.</param>
        /// <param name="currentFilterAccording">The current filter according.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult PrintIndex(string sortOrder, string currentFilter, string currentFilterAccording)
        {
            TopTenIndex[] topTenIndex = TopTenIndex.GetIndex(Db, UserId, currentFilter, currentFilterAccording, sortOrder).ToArray();
            if (topTenIndex.Count() > 1000)
            {
                topTenIndex = topTenIndex.Take(1000).ToArray();
                ViewBag.IsListCuttedMessage = ViewResource.Global_IsListCutted_Text;
            }

            return View("PrintIndex", "_PrintLayout", topTenIndex);
        }

        public ActionResult PrintDetails(int id)
        {
            TopTen topTen = TopTenCache.GetDetail(Db, id, UserId);
            if (!IsAccess(topTen))
            {
                return RedirectToAccessDenied();
            }

            TopTenDetails topTenDetails = TopTenDetails.GetModelView(topTen);
            return View("PrintDetails", "_PrintLayout", topTenDetails);
        }

        public FileContentResult Export()
        {
            var csvBytes = new byte[0];
            try
            {
                if (ModelState.IsValid)
                {
                    using (var lyconetService = new LyconetService(Db, UserId))
                    {
                        csvBytes = lyconetService.GetTopTen();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
            }

            return File(csvBytes, ExportCsvContentType, ExportFileDownloadName);
        }

        private bool IsAccess(TopTen topTen)
        {
            return topTen != null;
        }
    }
}