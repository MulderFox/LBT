// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-06-2014
//
// Last Modified By : zmikeska
// Last Modified On : 01-10-2014
// ***********************************************************************
// <copyright file="SharedContactsController.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.Cache;
using LBT.Filters;
using LBT.ModelViews;
using LBT.Models;
using PagedList;
using LBT.Resources;
using System;
using System.Web.Mvc;

namespace LBT.Controllers
{
    /// <summary>
    /// Class SharedContactsController
    /// </summary>
    [InitializeSimpleMembership]
    [Authorize]
    public class SharedContactsController : BaseController
    {
        //
        // GET: /SharedContacts/

        /// <summary>
        /// Indexes the specified return page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Index(int? page, int pageSize = PageSize)
        {
            int pageNumber;
            ProcessPaging(page, out pageNumber);

            PopulatePageSize(pageSize);

            SharedContact[] sharedContacts = SharedContactsCache.GetIndex(Db, UserId);
            SharedContactsIndex[] sharedContactsIndices = SharedContactsIndex.GetModelView(sharedContacts);

            return View(sharedContactsIndices.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /SharedContacts/Create

        /// <summary>
        /// Creates the specified return page.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Create()
        {
            PopulateToUplineUserId();
            PopulateToDownlineUserId();
            PopulateToAnyUserId();

            return View();
        }

        //
        // POST: /SharedContacts/Create

        /// <summary>
        /// Creates the specified sharedContactsCreate.
        /// </summary>
        /// <param name="sharedContactsCreate">The sharedContactsCreate.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SharedContactsCreate sharedContactsCreate)
        {
            if (ModelState.IsValid)
            {
                SharedContact sharedContact = sharedContactsCreate.GetModel(UserId);
                SharedContactsCache.Insert(Db, sharedContact);
                
                return RedirectToAction("Index");
            }

            PopulateToUplineUserId();
            PopulateToDownlineUserId();
            PopulateToAnyUserId();

            return View(sharedContactsCreate);
        }

        //
        // GET: /SharedContacts/Delete/5

        /// <summary>
        /// Deletes the specified return page.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Delete(int id = 0)
        {
            SharedContact sharedContact = SharedContactsCache.GetDetail(Db, id);
            if (!IsAccess(sharedContact))
            {
                return RedirectToAccessDenied();
            }

            SharedContactsDelete sharedContactDelete = SharedContactsDelete.GetModelView(sharedContact);

            return View(sharedContactDelete);
        }

        //
        // POST: /SharedContacts/Delete/5

        /// <summary>
        /// Deletes the confirmed.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SharedContact sharedContact;
            DeleteResult deleteResult = SharedContactsCache.Delete(Db, id, out sharedContact);
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

            SharedContactsDelete sharedContactDelete = SharedContactsDelete.GetModelView(sharedContact);

            return View(sharedContactDelete);
        }

        private bool IsAccess(SharedContact sharedContact)
        {
            return sharedContact != null && sharedContact.FromUserId == UserId;
        }
    }
}