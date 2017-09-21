// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-09-2014
//
// Last Modified By : zmikeska
// Last Modified On : 01-14-2014
// ***********************************************************************
// <copyright file="BaseController.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.DAL;
using LBT.Filters;
using LBT.Helpers;
using LBT.Models;
using LBT.ModelViews;
using System;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace LBT.Controllers
{
    /// <summary>
    /// Class BaseController
    /// </summary>
    [RequireLBTHttps]
    [SetLanguage]
    [SetAutomaticLogout]
    [CheckIsPoliciesAccepted]
    public partial class BaseController : Controller
    {
        #region Const

        public const string StatusMessageTempKey = "StatusMessage";
        public const string SuccessMessageTempKey = "SuccessMessage";

        protected const int PageSize = 15;
        protected const string ExportCsvContentType = "text/csv";

        protected const string LeadingRoles = "Admin,AdminLídr,Lídr";
        protected const string AdminRoles = "Admin,AdminLídr";

        private const string AscendingSufix = "asc";
        private const string DescendingSufix = "desc";

        #endregion

        #region Enums

        /// <summary>
        /// Enum DeleteRecordMessageId
        /// </summary>
        public enum DeleteRecordMessageId
        {
            /// <summary>
            /// The delete record success
            /// </summary>
            DeleteRecordSuccess,
            /// <summary>
            /// The delete record failed
            /// </summary>
            DeleteRecordFailed
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the user id.
        /// </summary>
        /// <value>The user id.</value>
        public int UserId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is admin.
        /// </summary>
        /// <value><c>true</c> if this instance is admin; otherwise, <c>false</c>.</value>
        public bool IsAdmin { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is leader.
        /// </summary>
        /// <value><c>true</c> if this instance is leader; otherwise, <c>false</c>.</value>
        public bool IsLeader { get; private set; }

        public bool IsAuthenticatedGuest { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is authenticated.
        /// </summary>
        /// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
        public bool IsAuthenticated { get; private set; }

        #endregion

        #region Fields

        /// <summary>
        /// The db
        /// </summary>
        public readonly DefaultContext Db = new DefaultContext();

        private bool _isAuthenticatedButNotGuest;

        #endregion

        #region Constructors and desctructors

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            Db.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        /// <summary>
        /// Initializes data that might not be available when the constructor is called.
        /// </summary>
        /// <param name="requestContext">The HTTP context and route data.</param>
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            UserId = WebSecurity.GetUserId(User.Identity.Name);
            IsAdmin = User.IsInRole(UserProfile.GetRoleTypeDbName(RoleType.Admin)) || User.IsInRole(UserProfile.GetRoleTypeDbName(RoleType.AdminLeader));
            IsLeader = User.IsInRole(UserProfile.GetRoleTypeDbName(RoleType.AdminLeader)) || User.IsInRole(UserProfile.GetRoleTypeDbName(RoleType.Leader));
            IsAuthenticatedGuest = User.IsInRole(UserProfile.GetRoleTypeDbName(RoleType.AuthGuest));
            IsAuthenticated = WebSecurity.IsAuthenticated;
            _isAuthenticatedButNotGuest = WebSecurity.IsAuthenticated && !IsAuthenticatedGuest;
        }

        /// <summary>
        /// Called after the action method is invoked.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            ViewBag.IsAdmin = IsAdmin;
            ViewBag.IsLeader = IsLeader;
            ViewBag.IsAuthenticated = IsAuthenticated;
            ViewBag.IsAuthenticatedButNotGuest = _isAuthenticatedButNotGuest;
            ViewBag.UserId = UserId;
            ViewBag.CurrentCultureLCID = Thread.CurrentThread.CurrentCulture.LCID;

            object message;
            if (TempData.TryGetValue(StatusMessageTempKey, out message))
            {
                ViewBag.StatusMessage = message.ToString();                
            }

            if (TempData.TryGetValue(SuccessMessageTempKey, out message))
            {
                ViewBag.SuccessMessage = message.ToString();
            }
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        protected ActionResult RedirectToAccessDenied()
        {
            return View("Error403", "_ErrorLayout");
        }

        protected void ProcessSearchFilterAndPaging(string currentFilter, string currentFilterAccording, ref string searchString, ref string searchStringAccording, ref int? page, out int pageNumber)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
                searchStringAccording = currentFilterAccording;
            }
            pageNumber = (page ?? 1);
        }

        protected void ProcessPaging(int? page, out int pageNumber)
        {
            pageNumber = page ?? 1;
        }

        /// <summary>
        /// Nastaví řazení (první sortingName je default)
        /// </summary>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="sortingNames">The sorting names.</param>
        /// <param name="prefix">The prefix.</param>
        /// <example>
        /// sortingName = Title
        /// class for column = ViewBag.TitleSortSymbol
        /// sortOrder param for column = ViewBag.TitleSortParam
        /// sortOrder param for another element = ViewBag.CurrentSort
        ///   </example>
        protected void ProcessSorting(string sortOrder, string[] sortingNames, string prefix = null)
        {
            string currentSortParamName = String.Format("{0}CurrentSort", prefix);
            ViewData[currentSortParamName] = sortOrder;

            for (int i = 0; i < sortingNames.Length; i++)
            {
                string sortParamName = String.Format("{0}{1}SortParam", prefix, sortingNames[i]);
                string sortParamValue = GetSortParam(sortOrder, sortingNames[i], i == 0);
                ViewData[sortParamName] = sortParamValue;

                string sortSymbolName = String.Format("{0}{1}SortSymbol", prefix, sortingNames[i]);
                string sortSymbolValue = GetSortSymbol(sortOrder, sortingNames[i], i == 0);
                ViewData[sortSymbolName] = sortSymbolValue;
            }
        }

        protected bool CheckFilteredUserId(ref int selectedFilteredUserId)
        {
            if (selectedFilteredUserId == 0)
            {
                selectedFilteredUserId = UserId;
                return true;
            }

            int userId = selectedFilteredUserId;
            return FilteredUser.GetFilteredUsers(Db, UserId).Any(gfu => gfu.UserId == userId);
        }

        private string GetSortSymbol(string sortOrder, string name, bool defaultSortOrder)
        {
            string ascendingName = GetAscendingName(name);
            if (defaultSortOrder && String.IsNullOrEmpty(sortOrder) || sortOrder == ascendingName)
            {
                return AscendingSufix;
            }

            string descendingName = GetDescendingName(name);
            return sortOrder == descendingName ? DescendingSufix : String.Empty;
        }

        private string GetSortParam(string sortOrder, string name, bool defaultSortOrder)
        {
            string ascendingName = GetAscendingName(name);
            if (defaultSortOrder && String.IsNullOrEmpty(sortOrder) || sortOrder == ascendingName)
            {
                string descendingName = GetDescendingName(name);
                return descendingName;
            }

            return !defaultSortOrder ? ascendingName : String.Empty;
        }

        private string GetAscendingName(string name)
        {
            string ascendingName = String.Format("{0}_{1}", name, AscendingSufix);
            return ascendingName;
        }

        private string GetDescendingName(string name)
        {
            string descendingName = String.Format("{0}_{1}", name, DescendingSufix);
            return descendingName;
        }
    }
}
