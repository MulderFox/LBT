// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 12-20-2013
//
// Last Modified By : zmikeska
// Last Modified On : 01-10-2014
// ***********************************************************************
// <copyright file="PhoneNumberPrefixController.cs" company="Zdeněk Mikeska">
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
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace LBT.Controllers
{
    /// <summary>
    /// Class PhoneNumberPrefixController
    /// </summary>
    [InitializeSimpleMembership]
    [Authorize(Roles = AdminRoles)]
    public class PhoneNumberPrefixController : BaseController
    {
        //
        // GET: /PhoneNumberPrefix/

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

            IQueryable<PhoneNumberPrefix> phoneNumberPrefixes = PhoneNumberPrefixCache.GetIndex(Db);
            PhoneNumberPrefixIndex[] phoneNumberPrefixIndices = PhoneNumberPrefixIndex.GetModelView(phoneNumberPrefixes.ToArray());

            return View(phoneNumberPrefixIndices.ToPagedList(pageNumber, PageSize));
        }

        //
        // GET: /PhoneNumberPrefix/Create

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /PhoneNumberPrefix/Create

        /// <summary>
        /// Creates the specified phoneNumberPrefixEdit.
        /// </summary>
        /// <param name="phoneNumberPrefixEdit">The phoneNumberPrefixEdit.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PhoneNumberPrefixEdit phoneNumberPrefixEdit)
        {
            if (ModelState.IsValid)
            {
                PhoneNumberPrefix phoneNumberPrefix = phoneNumberPrefixEdit.GetModel();
                Db.PhoneNumberPrefixes.Add(phoneNumberPrefix);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(phoneNumberPrefixEdit);
        }

        //
        // GET: /PhoneNumberPrefix/Edit/5

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Edit(int id = 0)
        {
            PhoneNumberPrefix phonenumberprefix = Db.PhoneNumberPrefixes.Find(id);
            if (phonenumberprefix == null)
            {
                return RedirectToAccessDenied();
            }

            PhoneNumberPrefixEdit phoneNumberPrefixEdit = PhoneNumberPrefixEdit.GetModelView(phonenumberprefix);

            return View(phoneNumberPrefixEdit);
        }

        //
        // POST: /PhoneNumberPrefix/Edit/5

        /// <summary>
        /// Edits the specified phoneNumberPrefixEdit.
        /// </summary>
        /// <param name="phoneNumberPrefixEdit">The phoneNumberPrefixEdit.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PhoneNumberPrefixEdit phoneNumberPrefixEdit)
        {
            if (ModelState.IsValid)
            {
                PhoneNumberPrefix phoneNumberPrefix = phoneNumberPrefixEdit.GetModel();
                Db.Entry(phoneNumberPrefix).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(phoneNumberPrefixEdit);
        }

        //
        // GET: /PhoneNumberPrefix/Delete/5

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Delete(int id = 0)
        {
            PhoneNumberPrefix phonenumberprefix = Db.PhoneNumberPrefixes.Find(id);
            if (phonenumberprefix == null)
            {
                return RedirectToAccessDenied();
            }

            PhoneNumberPrefixDetails phoneNumberPrefixDetails = PhoneNumberPrefixDetails.GetModelView(phonenumberprefix);

            return View(phoneNumberPrefixDetails);
        }

        //
        // POST: /PhoneNumberPrefix/Delete/5

        /// <summary>
        /// Deletes the confirmed.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PhoneNumberPrefix phonenumberprefix = Db.PhoneNumberPrefixes.Find(id);
            try
            {
                Db.PhoneNumberPrefixes.Remove(phonenumberprefix);
                Db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError(BaseCache.TitleField, ValidationResource.Global_DeleteRecord_ErrorMessage);

                PhoneNumberPrefixDetails phoneNumberPrefixDetails = PhoneNumberPrefixDetails.GetModelView(phonenumberprefix);

                return View(phoneNumberPrefixDetails);
            }
        }

        /// <summary>
        /// Determines whether is title unique.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="phoneNumberPrefixId">The phone number prefix id.</param>
        public JsonResult IsTitleUnique(string title, int? phoneNumberPrefixId)
        {
            if (String.IsNullOrEmpty(title))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            int fixPhoneNumberPrefixId = phoneNumberPrefixId.GetValueOrDefault();
            PhoneNumberPrefix[] phoneNumberPrefixes = Db.PhoneNumberPrefixes.Where(d => d.PhoneNumberPrefixId != fixPhoneNumberPrefixId && d.Title == title).ToArray();
            return Json(!phoneNumberPrefixes.Any(), JsonRequestBehavior.AllowGet);
        }
    }
}