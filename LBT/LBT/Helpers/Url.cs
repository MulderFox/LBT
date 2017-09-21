// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 02-27-2014
//
// Last Modified By : zmikeska
// Last Modified On : 02-27-2014
// ***********************************************************************
// <copyright file="Url.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Web.Mvc;

namespace LBT.Helpers
{
    /// <summary>
    /// Class Url
    /// </summary>
    public class Url
    {
        /// <summary>
        /// Gets the web root URL.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GetWebRootUrl()
        {
            bool useSsl = Properties.Settings.Default.UseSSL;
            string webUrl = Properties.Settings.Default.WebUrl;
            string sslPort = Properties.Settings.Default.SSLPort;
            return String.Format(useSsl ? "https://{0}:{1}" : "http://{0}", webUrl, sslPort);
        }

        public static string GetActionAbsoluteUrl(UrlHelper urlHelper, string actionName, string controllerName, object routeValues)
        {
            var baseUri = new Uri(GetWebRootUrl());
            string relativeUri = urlHelper.Action(actionName, controllerName, routeValues);
            var uri = new Uri(baseUri, relativeUri);
            return uri.AbsoluteUri;
        }
    }
}