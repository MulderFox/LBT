// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-25-2014
//
// Last Modified By : zmikeska
// Last Modified On : 01-25-2014
// ***********************************************************************
// <copyright file="RequireLBTHttpsAttribute.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Web.Mvc;

namespace LBT.Filters
{
    /// <summary>
    /// Class RequireHttpsAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    // ReSharper disable InconsistentNaming
    public class RequireLBTHttpsAttribute : FilterAttribute, IAuthorizationFilter
    // ReSharper restore InconsistentNaming
    {
        // Methods
        /// <summary>
        /// Handles the non HTTPS request.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        /// <exception cref="System.InvalidOperationException">The requested resource can only be accessed via SSL.</exception>
        protected virtual void HandleNonHttpsRequest(AuthorizationContext filterContext)
        {
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("The requested resource can only be accessed via SSL.");
            }

            if (filterContext.HttpContext.Request.Url == null)
                return;

            string url = String.Format("https://{0}:{1}{2}", filterContext.HttpContext.Request.Url.Host, Properties.Settings.Default.SSLPort, filterContext.HttpContext.Request.RawUrl);
            filterContext.Result = new RedirectResult(url);
        }

        /// <summary>
        /// Called when authorization is required.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        /// <exception cref="System.ArgumentNullException">filterContext</exception>
        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.PermanentRedirectUrl) && filterContext.HttpContext.Request.Url != null && !filterContext.HttpContext.Request.Url.AbsoluteUri.Contains(Properties.Settings.Default.PermanentRedirectUrl))
            {
                string url = String.Format("{0}{1}", Properties.Settings.Default.PermanentRedirectUrl, filterContext.HttpContext.Request.RawUrl);
                filterContext.Result = new RedirectResult(url);
                return;
            }

            bool useSsl = Properties.Settings.Default.UseSSL;
            if (!filterContext.HttpContext.Request.IsSecureConnection && useSsl)
            {
                HandleNonHttpsRequest(filterContext);
            }
        }
    }
}