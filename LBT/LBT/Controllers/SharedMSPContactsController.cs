//// ***********************************************************************
//// Assembly         : LBT
//// Author           : zmikeska
//// Created          : 01-18-2014
////
//// Last Modified By : zmikeska
//// Last Modified On : 01-18-2014
//// ***********************************************************************
//// <copyright file="SharedMSPContactsController.cs" company="Zdeněk Mikeska">
////     Copyright (c) Zdeněk Mikeska. All rights reserved.
//// </copyright>
//// <summary></summary>
//// ***********************************************************************

//using LBT.Filters;
//using LBT.Models;
//using LBT.Properties;
//using PagedList;
//using System.Data.Entity;
//using System.Linq;
//using System.Web.Mvc;

//namespace LBT.Controllers
//{
//    // ReSharper disable InconsistentNaming
//    /// <summary>
//    /// Class SharedMSPContactsController
//    /// </summary>
//    [InitializeSimpleMembership]
//    [Authorize]
//    public class SharedMSPContactsController : BaseController
//    // ReSharper restore InconsistentNaming
//    {
//        //
//        // GET: /SharedMSPContacts/

//        /// <summary>
//        /// Indexes this instance.
//        /// </summary>
//        /// <returns>ActionResult.</returns>
//        public ActionResult Index(int? page)
//        {
//            IQueryable<SharedMSPContact> sharedMspContacts = Db.SharedMSPContacts.Include(s => s.FromUser).Include(s => s.ToUser).Where(s => s.FromUserId == UserId).OrderBy(s => s.ToUser.UserName);

//            int pageNumber = page ?? 1;
//            return View(sharedMspContacts.ToPagedList(pageNumber, PageSize));
//        }

//        //
//        // GET: /SharedMSPContacts/Create

//        /// <summary>
//        /// Creates this instance.
//        /// </summary>
//        /// <returns>ActionResult.</returns>
//        public ActionResult Create()
//        {
//            IQueryable<int> excludedUserIds =
//                Db.SharedMSPContacts.Where(sc => sc.FromUserId == UserId).Select(sc => sc.ToUserId).Union(new[] { UserId });
//            ViewBag.ToUserId = new SelectList(Db.UserProfiles.Where(up => !excludedUserIds.Contains(up.UserId)), "UserId", "UserName");

//            return View();
//        }

//        //
//        // POST: /SharedMSPContacts/Create

//        /// <summary>
//        /// Creates the specified sharedmspcontact.
//        /// </summary>
//        /// <param name="sharedmspcontact">The sharedmspcontact.</param>
//        /// <returns>ActionResult.</returns>
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(SharedMSPContact sharedmspcontact)
//        {
//            if (ModelState.IsValid)
//            {
//                sharedmspcontact.FromUserId = UserId;
//                Db.SharedMSPContacts.Add(sharedmspcontact);
//                Db.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            ViewBag.ToUserId = new SelectList(Db.UserProfiles, "UserId", "UserName", sharedmspcontact.ToUserId);
//            return View(sharedmspcontact);
//        }

//        //
//        // GET: /SharedMSPContacts/Delete/5

//        /// <summary>
//        /// Deletes the specified id.
//        /// </summary>
//        /// <param name="id">The id.</param>
//        /// <returns>ActionResult.</returns>
//        public ActionResult Delete(int id = 0)
//        {
//            SharedMSPContact sharedmspcontact = Db.SharedMSPContacts.Find(id);
//            if (sharedmspcontact == null || sharedmspcontact.FromUserId != UserId)
//            {
//                return HttpNotFound(Resources.Global_AccessDeniedText_ErrorMessage);
//            }

//            return View(sharedmspcontact);
//        }

//        //
//        // POST: /SharedMSPContacts/Delete/5

//        /// <summary>
//        /// Deletes the confirmed.
//        /// </summary>
//        /// <param name="id">The id.</param>
//        /// <returns>ActionResult.</returns>
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            SharedMSPContact sharedmspcontact = Db.SharedMSPContacts.Find(id);
//            if (sharedmspcontact == null || sharedmspcontact.FromUserId != UserId)
//            {
//                return HttpNotFound(Resources.Global_AccessDeniedText_ErrorMessage);
//            }

//            Db.SharedMSPContacts.Remove(sharedmspcontact);
//            Db.SaveChanges();

//            return RedirectToAction("Index");
//        }
//    }
//}