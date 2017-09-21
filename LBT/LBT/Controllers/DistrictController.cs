// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 12-20-2013
//
// Last Modified By : zmikeska
// Last Modified On : 01-12-2014
// ***********************************************************************
// <copyright file="DistrictController.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.Cache;
using LBT.Filters;
using LBT.Models;
using LBT.ModelViews;
using PagedList;
using LBT.Resources;
using System;
using System.Linq;
using System.Web.Mvc;

namespace LBT.Controllers
{
    /// <summary>
    /// Class DistrictController
    /// </summary>
    [InitializeSimpleMembership]
    [Authorize(Roles = AdminRoles)]
    public class DistrictController : BaseController
    {
        //
        // GET: /District/

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Index(int? page, int pageSize = PageSize)
        {
            int pageNumber;
            ProcessPaging(page, out pageNumber);

            PopulatePageSize(pageSize);

            District[] districts = DistrictCache.GetIndex(Db);

            DistrictIndex[] districtIndices = DistrictIndex.GetModelView(districts);

            return View(districtIndices.ToPagedList(pageNumber, PageSize));
        }

        //
        // GET: /District/Create

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /District/Create

        /// <summary>
        /// Creates the specified districtEdit.
        /// </summary>
        /// <param name="districtEdit">The districtEdit.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DistrictEdit districtEdit)
        {
            if (ModelState.IsValid)
            {
                District district = districtEdit.GetModel();
                DistrictCache.Insert(Db, district);
                return RedirectToAction("Index");
            }

            return View(districtEdit);
        }

        //
        // GET: /District/Edit/5

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Edit(int id = 0)
        {
            District district = DistrictCache.GetDetail(Db, id);
            if (!IsAccess(district))
            {
                return RedirectToAccessDenied();
            }

            DistrictEdit districtEdit = DistrictEdit.GetModelView(district);

            return View(districtEdit);
        }

        //
        // POST: /District/Edit/5

        /// <summary>
        /// Edits the specified districtEdit.
        /// </summary>
        /// <param name="districtEdit"> </param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DistrictEdit districtEdit)
        {
            if (ModelState.IsValid)
            {
                District district = districtEdit.GetModel();
                bool success = DistrictCache.Update(Db, ref district);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("Index");
            }

            return View(districtEdit);
        }

        //
        // GET: /District/Delete/5

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Delete(int id = 0)
        {
            District district = DistrictCache.GetDetail(Db, id);
            if (!IsAccess(district))
            {
                return RedirectToAccessDenied();
            }

            DistrictDelete districtDelete = DistrictDelete.GetModelView(district);

            return View(districtDelete);
        }

        //
        // POST: /District/Delete/5

        /// <summary>
        /// Deletes the confirmed.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            District district;
            DeleteResult deleteResult = DistrictCache.Delete(Db, id, out district);

            switch (deleteResult)
            {
                case DeleteResult.Ok:
                    return RedirectToAction("Index");

                case DeleteResult.AuthorizationFailed:
                    return RedirectToAccessDenied();

                case DeleteResult.DbFailed:
                    ModelState.AddModelError(BaseCache.TitleField, ValidationResource.Global_DeleteRecord_ErrorMessage);
                    DistrictDelete districtDelete = DistrictDelete.GetModelView(district);
                    return View(districtDelete);
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Determines whether is title unique.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="districtId">The districtEdit id.</param>
        public JsonResult IsTitleUnique(string title, int? districtId)
        {
            if (String.IsNullOrEmpty(title))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            int fixDistrictId = districtId.GetValueOrDefault();
            District[] districts = Db.Districts.Where(d => d.DistrictId != fixDistrictId && d.Title == title).ToArray();
            return Json(!districts.Any(), JsonRequestBehavior.AllowGet);
        }

        private bool IsAccess(District district)
        {
            return district != null;
        }
    }
}