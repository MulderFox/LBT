//// ***********************************************************************
//// Assembly         : LBT
//// Author           : zmikeska
//// Created          : 12-20-2013
////
//// Last Modified By : zmikeska
//// Last Modified On : 01-14-2014
//// ***********************************************************************
//// <copyright file="MSPContactController.cs" company="Zdeněk Mikeska">
////     Copyright (c) Zdeněk Mikeska. All rights reserved.
//// </copyright>
//// <summary></summary>
//// ***********************************************************************

//using LBT.Filters;
//using LBT.Models;
//using LBT.Properties;
//using PagedList;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Web.Mvc;

//namespace LBT.Controllers
//{
//    /// <summary>
//    /// Class MSPContactController
//    /// </summary>
//    [InitializeSimpleMembership]
//    [Authorize(Roles = ViewResource.AdminRoles)]
//    // ReSharper disable InconsistentNaming
//    public class MSPContactController : BaseController
//    // ReSharper restore InconsistentNaming
//    {
//        //
//        // GET: /MSPContact/

//        /// <summary>
//        /// Indexes the specified sort order.
//        /// </summary>
//        /// <param name="sortOrder">The sort order.</param>
//        /// <param name="currentFilter">The current filter.</param>
//        /// <param name="searchString">The search string.</param>
//        /// <param name="page">The page.</param>
//        /// <param name="filteredUserId">The filtered user id.</param>
//        /// <returns>ActionResult.</returns>
//        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int filteredUserId = 0)
//        {
//            if (!CheckFilteredUserId(ref filteredUserId))
//            {
//                return HttpNotFound(Resources.Global_AccessDeniedText_ErrorMessage);
//            }

//            IQueryable<MSPContact> mspContacts =
//                Db.MSPContacts.Include(p => p.Registrar).Include(p => p.District).Include(p => p.PhoneNumberPrefix1).
//                    Include(p => p.PhoneNumberPrefix2).Where(
//                        p => p.RegistrarId == filteredUserId);

//            if (!String.IsNullOrEmpty(searchString))
//            {
//                page = 1;
//            }
//            else
//            {
//                searchString = currentFilter;
//            }

//            if (!String.IsNullOrEmpty(searchString))
//            {
//                mspContacts =
//                    mspContacts.Where(
//                        up =>
//                        up.CompanyName.ToUpper().Contains(searchString.ToUpper()));
//            }

//            switch (sortOrder)
//            {
//                case "CompanyName_desc":
//                    mspContacts = mspContacts.OrderByDescending(up => up.CompanyName);
//                    break;

//                default:
//                    mspContacts = mspContacts.OrderBy(up => up.CompanyName);
//                    break;
//            }

//            ViewBag.SelectedFilteredUserId = filteredUserId;
//            ViewBag.CurrentSort = sortOrder;
//            ViewBag.CurrentFilter = searchString;
//            ViewBag.CompanyNameSortParm = String.IsNullOrEmpty(sortOrder) ? "CompanyName_desc" : "";
//            ViewBag.IsRegistrar = filteredUserId == UserId;

//            PopulateFilteredUserId(filteredUserId);

//            int pageNumber = (page ?? 1);
//            return View(mspContacts.ToPagedList(pageNumber, PageSize));
//        }

//        //
//        // GET: /MSPContact/Details/5

//        /// <summary>
//        /// Detailses the specified id.
//        /// </summary>
//        /// <param name="id">The id.</param>
//        /// <returns>ActionResult.</returns>
//        public ActionResult Details(int id = 0)
//        {
//            MSPContact mspcontact = Db.MSPContacts.Find(id);
//            if (mspcontact == null || !GetFilteredUserIds().Select(s => s.UserId).Contains(mspcontact.RegistrarId))
//            {
//                return HttpNotFound(Resources.Global_AccessDeniedText_ErrorMessage);
//            }

//            ViewBag.IsRegistrar = mspcontact.RegistrarId == UserId;

//            return View(mspcontact);
//        }

//        //
//        // GET: /MSPContact/Create

//        /// <summary>
//        /// Creates this instance.
//        /// </summary>
//        /// <returns>ActionResult.</returns>
//        public ActionResult Create()
//        {
//            ViewBag.RegistrarId = new SelectList(Db.UserProfiles, "UserId", "UserName");
//            ViewBag.DistrictId = new SelectList(Db.Districts, "DistrictId", "Title");
//            ViewBag.PhoneNumberPrefix1Id = new SelectList(Db.PhoneNumberPrefixes, "PhoneNumberPrefixId", "Title");
//            ViewBag.PhoneNumberPrefix2Id = new SelectList(Db.PhoneNumberPrefixes, "PhoneNumberPrefixId", "Title");

//            return View();
//        }

//        //
//        // POST: /MSPContact/Create

//        /// <summary>
//        /// Creates the specified mspcontact.
//        /// </summary>
//        /// <param name="mspcontact">The mspcontact.</param>
//        /// <returns>ActionResult.</returns>
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(MSPContact mspcontact)
//        {
//            if (ModelState.IsValid)
//            {
//                mspcontact.Created = DateTime.Now;
//                mspcontact.RegistrarId = UserId;
//                Db.MSPContacts.Add(mspcontact);
//                Db.SaveChanges();

//                return RedirectToAction("Index");
//            }

//            ViewBag.RegistrarId = new SelectList(Db.UserProfiles, "UserId", "UserName", mspcontact.RegistrarId);
//            ViewBag.DistrictId = new SelectList(Db.Districts, "DistrictId", "Title", mspcontact.DistrictId);
//            ViewBag.PhoneNumberPrefix1Id = new SelectList(Db.PhoneNumberPrefixes, "PhoneNumberPrefixId", "Title", mspcontact.PhoneNumberPrefix1Id);
//            ViewBag.PhoneNumberPrefix2Id = new SelectList(Db.PhoneNumberPrefixes, "PhoneNumberPrefixId", "Title", mspcontact.PhoneNumberPrefix2Id);

//            return View(mspcontact);
//        }

//        //
//        // GET: /MSPContact/Edit/5

//        /// <summary>
//        /// Edits the specified id.
//        /// </summary>
//        /// <param name="id">The id.</param>
//        /// <returns>ActionResult.</returns>
//        public ActionResult Edit(int id = 0)
//        {
//            MSPContact mspcontact = Db.MSPContacts.Find(id);
//            if (mspcontact == null || mspcontact.RegistrarId != UserId)
//            {
//                return HttpNotFound(Resources.Global_AccessDeniedText_ErrorMessage);
//            }

//            ViewBag.RegistrarId = new SelectList(Db.UserProfiles, "UserId", "UserName", mspcontact.RegistrarId);
//            ViewBag.DistrictId = new SelectList(Db.Districts, "DistrictId", "Title", mspcontact.DistrictId);
//            ViewBag.PhoneNumberPrefix1Id = new SelectList(Db.PhoneNumberPrefixes, "PhoneNumberPrefixId", "Title", mspcontact.PhoneNumberPrefix1Id);
//            ViewBag.PhoneNumberPrefix2Id = new SelectList(Db.PhoneNumberPrefixes, "PhoneNumberPrefixId", "Title", mspcontact.PhoneNumberPrefix2Id);

//            return View(mspcontact);
//        }

//        //
//        // POST: /MSPContact/Edit/5

//        /// <summary>
//        /// Edits the specified mspcontact.
//        /// </summary>
//        /// <param name="mspcontact">The mspcontact.</param>
//        /// <returns>ActionResult.</returns>
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(MSPContact mspcontact)
//        {
//            if (ModelState.IsValid)
//            {
//                if (mspcontact.RegistrarId != UserId)
//                {
//                    return HttpNotFound(Resources.Global_AccessDeniedText_ErrorMessage);
//                }

//                Db.Entry(mspcontact).State = EntityState.Modified;
//                Db.SaveChanges();

//                return RedirectToAction("Index");
//            }
//            ViewBag.RegistrarId = new SelectList(Db.UserProfiles, "UserId", "UserName", mspcontact.RegistrarId);
//            ViewBag.DistrictId = new SelectList(Db.Districts, "DistrictId", "Title", mspcontact.DistrictId);
//            ViewBag.PhoneNumberPrefix1Id = new SelectList(Db.PhoneNumberPrefixes, "PhoneNumberPrefixId", "Title", mspcontact.PhoneNumberPrefix1Id);
//            ViewBag.PhoneNumberPrefix2Id = new SelectList(Db.PhoneNumberPrefixes, "PhoneNumberPrefixId", "Title", mspcontact.PhoneNumberPrefix2Id);

//            return View(mspcontact);
//        }

//        //
//        // GET: /MSPContact/Delete/5

//        /// <summary>
//        /// Deletes the specified id.
//        /// </summary>
//        /// <param name="id">The id.</param>
//        /// <returns>ActionResult.</returns>
//        public ActionResult Delete(int id = 0)
//        {
//            MSPContact mspcontact = Db.MSPContacts.Find(id);
//            if (mspcontact == null || mspcontact.RegistrarId != UserId)
//            {
//                return HttpNotFound(Resources.Global_AccessDeniedText_ErrorMessage);
//            }

//            return View(mspcontact);
//        }

//        //
//        // POST: /MSPContact/Delete/5

//        /// <summary>
//        /// Deletes the confirmed.
//        /// </summary>
//        /// <param name="id">The id.</param>
//        /// <returns>ActionResult.</returns>
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            MSPContact mspcontact = Db.MSPContacts.Find(id);
//            if (mspcontact == null || mspcontact.RegistrarId != UserId)
//            {
//                return HttpNotFound(Resources.Global_AccessDeniedText_ErrorMessage);
//            }

//            Db.MSPContacts.Remove(mspcontact);
//            Db.SaveChanges();

//            return RedirectToAction("Index");
//        }

//        /// <summary>
//        /// Determines whether is IČO unique.
//        /// </summary>
//        /// <param name="ico">The IČO.</param>
//        /// <param name="mspContactId">The MSP contact id.</param>
//        public JsonResult IsIcoUnique(string ico, int? mspContactId)
//        {
//            if (String.IsNullOrEmpty(ico))
//            {
//                return Json(true, JsonRequestBehavior.AllowGet);
//            }

//            // ReSharper disable InconsistentNaming
//            int fixMSPContactId = mspContactId.GetValueOrDefault();
//            // ReSharper restore InconsistentNaming
//            MSPContact[] mspContacts = Db.MSPContacts.Include(p => p.Registrar)
//                .Where(m => m.MSPContactId != fixMSPContactId && m.ICO == ico).ToArray();

//            return !mspContacts.Any()
//                       ? Json(true, JsonRequestBehavior.AllowGet)
//                       : Json(
//                           String.Format("Toto IČO je již evidováno uživatelem {0}.", mspContacts[0].Registrar.UserName),
//                           JsonRequestBehavior.AllowGet);
//        }

//        /// <summary>
//        /// Determines whether [is phone number1 unique by user] [the specified phone number1].
//        /// </summary>
//        /// <param name="phoneNumber1">The phone number1.</param>
//        /// <param name="mspContactId">The MSP contact id.</param>
//        /// <param name="phoneNumberPrefix1Id">The phone number prefix1 id.</param>
//        public JsonResult IsPhoneNumber1UniqueByUser(string phoneNumber1, int? mspContactId, int? phoneNumberPrefix1Id)
//        {
//            if (String.IsNullOrEmpty(phoneNumber1))
//            {
//                return Json(true, JsonRequestBehavior.AllowGet);
//            }

//            int fixMspContactId = mspContactId.GetValueOrDefault();
//            int fixPhoneNumberPrefix1Id = phoneNumberPrefix1Id.GetValueOrDefault(1);
//            MSPContact[] mspContacts =
//                Db.MSPContacts.Include(p => p.Registrar).Include(p => p.PhoneNumberPrefix1)
//                    .Where(
//                        p =>
//                        p.MSPContactId != fixMspContactId &&
//                        p.PhoneNumberPrefix1.PhoneNumberPrefixId == fixPhoneNumberPrefix1Id &&
//                        p.PhoneNumber1 == phoneNumber1 &&
//                        p.Registrar.UserId == UserId).ToArray();

//            return Json(!mspContacts.Any(), JsonRequestBehavior.AllowGet);
//        }

//        /// <summary>
//        /// Gets the filtered user ids.
//        /// </summary>
//        /// <returns>IEnumerable{UserProfile}.</returns>
//        private IEnumerable<UserProfile> GetFilteredUserIds()
//        {
//            IEnumerable<UserProfile> filteredUserIds = new[] { new UserProfile { UserId = UserId, UserName = "@já" } }
//                .Union(from s in Db.SharedMSPContacts
//                       join u in Db.UserProfiles on s.FromUserId equals u.UserId
//                       where s.ToUserId == UserId
//                       select u);
//            return filteredUserIds;
//        }

//        /// <summary>
//        /// Checks the filtered user id.
//        /// </summary>
//        /// <param name="selectedFilteredUserId">The selected filtered user id.</param>
//        private new bool CheckFilteredUserId(ref int selectedFilteredUserId)
//        {
//            if (selectedFilteredUserId == 0)
//            {
//                selectedFilteredUserId = UserId;
//                return true;
//            }

//            int userId = selectedFilteredUserId;
//            return GetFilteredUserIds().Any(gfui => gfui.UserId == userId);
//        }

//        /// <summary>
//        /// Populates the filtered user id.
//        /// </summary>
//        /// <param name="selectedFilteredUserId">The selected filtered user id.</param>
//        private new void PopulateFilteredUserId(object selectedFilteredUserId = null)
//        {
//            var filteredUserIds = GetFilteredUserIds().Distinct();
//            ViewBag.FilteredUserId = new SelectList(filteredUserIds, "UserId", "UserName", selectedFilteredUserId);
//        }
//    }
//}