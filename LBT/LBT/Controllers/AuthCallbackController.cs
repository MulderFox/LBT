// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-27-2014
//
// Last Modified By : zmikeska
// Last Modified On : 01-29-2014
// ***********************************************************************
// <copyright file="AuthCallbackController.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.Helpers;
using LBT.Models;
using LBT.Services.GoogleApis;
using System;
using System.Web.Mvc;

namespace LBT.Controllers
{
    /// <summary>
    /// Class AuthCallbackController
    /// </summary>
    public class AuthCallbackController : Google.Apis.Auth.OAuth2.Mvc.Controllers.AuthCallbackController
    {
        /// <summary>
        /// The _user id
        /// </summary>
        private string _googleCredentialsJson;

        /// <summary>
        /// Gets the flow data which contains
        /// </summary>
        /// <value>The flow data.</value>
        protected override Google.Apis.Auth.OAuth2.Mvc.FlowMetadata FlowData
        {
            get { return new AppFlowMetadata(Request.PhysicalApplicationPath, _googleCredentialsJson); }
        }

        /// <summary>
        /// Called before the action method is invoked.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string cookie = Cookie.GetCookie(filterContext.HttpContext.Request, Cookie.GoogleCredentialsJsonCookieKey);
            if (String.IsNullOrEmpty(cookie))
                return;

            _googleCredentialsJson = cookie;
        }

        /// <summary>
        /// Called after the action method is invoked.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewBag.IsAdmin = User.IsInRole(UserProfile.GetRoleTypeDbName(RoleType.Admin)) || User.IsInRole(UserProfile.GetRoleTypeDbName(RoleType.AdminLeader));
        }

        /// <summary>
        /// A callback which gets the error when this controller didn't receive an authorization code. The default
        /// implementation throws a <seealso cref="T:Google.Apis.Auth.OAuth2.Responses.TokenResponseException" />.
        /// </summary>
        /// <param name="errorResponse">The error response.</param>
        /// <returns>System.Web.Mvc.ActionResult.</returns>
        protected override ActionResult OnTokenError(Google.Apis.Auth.OAuth2.Responses.TokenErrorResponse errorResponse)
        {
            ViewBag.Error = errorResponse.Error;
            return View("IndexAsync", "_PopupLayout");
        }
    }
}
