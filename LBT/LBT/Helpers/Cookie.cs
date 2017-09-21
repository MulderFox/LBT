// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-30-2014
//
// Last Modified By : zmikeska
// Last Modified On : 01-30-2014
// ***********************************************************************
// <copyright file="Cookie.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Linq;
using System.Web;

namespace LBT.Helpers
{
    /// <summary>
    /// Class Cookie
    /// </summary>
    public static class Cookie
    {
        #region Const

        public const string TestGoogleCredentialsJsonCookieKey = "LBT.TestGoogleCredentialsJson";
        public const string TestGoogleCalendarIdCookieKey = "LBT.TestGoogleCalendarId";
        public const string GoogleCredentialsJsonCookieKey = "LBT.GoogleCredentialsJson";
        public const string SwitchToUserProfileIndexTreeCookieKey = "LBT.SwitchToUserProfileIndexTree";
        public const string ShowFormattingCookieKey = "LBT.ShowFormatting";
        public const string HideDeathContactsCookieKey = "LBT.HideDeathContacts";
        public const string ScrollTopCookieKey = "LBT.ScrollTop";
        public const string IsAllowedPrivacyCookieKey = "LBT.IsAllowedPrivacy";
        public const string IsAllowedPrivacyForUserCookieKey = "LBT.IsAllowedPrivacyForUser";
        public const string IsUsingCookiesClosed = "LBT.IsUsingCookiesClosed";

        public const string CookieTrue = "true";
        public const string CookieFalse = "false";

        #endregion

        /// <summary>
        /// Sets the new cookie.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="response">The response.</param>
        public static void SetNewCookie(string key, string value, HttpResponse response)
        {
            // Nepoužívat ref reponse. WebHttpResponse nemá setter!
            SetNewCookie(key, value, DateTime.Now.AddDays(1), response);
        }

        /// <summary>
        /// Sets the new cookie.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expires">The expires.</param>
        /// <param name="response">The response.</param>
        public static void SetNewCookie(string key, string value, DateTime expires, HttpResponse response)
        {
            // Nepoužívat ref reponse. WebHttpResponse nemá setter!
            RemoveCookie(key, ref response);

            var httpCookie = new HttpCookie(key, value) { Expires = expires };
            response.Cookies.Add(httpCookie);
        }

        /// <summary>
        /// Sets the new cookie.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="response">The response.</param>
        public static void SetNewCookie(string key, string value, HttpResponseBase response)
        {
            // Nepoužívat ref reponse. WebHttpResponse nemá setter!
            SetNewCookie(key, value, DateTime.Now.AddDays(1), response);
        }

        /// <summary>
        /// Sets the new cookie.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expires">The expires.</param>
        /// <param name="response">The response.</param>
        public static void SetNewCookie(string key, string value, DateTime expires, HttpResponseBase response)
        {
            // Nepoužívat ref reponse. WebHttpResponse nemá setter!
            RemoveCookie(key, ref response);

            var httpCookie = new HttpCookie(key, value) { Expires = expires };
            response.Cookies.Add(httpCookie);
        }

        /// <summary>
        /// Removes the cookie.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="response">The response.</param>
        public static void RemoveCookie(string key, ref HttpResponse response)
        {
            if (response.Cookies.AllKeys.Contains(key))
                response.Cookies.Remove(key);
        }

        /// <summary>
        /// Removes the cookie.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="response">The response.</param>
        public static void RemoveCookie(string key, ref HttpResponseBase response)
        {
            if (response.Cookies.AllKeys.Contains(key))
                response.Cookies.Remove(key);
        }

        /// <summary>
        /// Gets the cookie.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public static string GetCookie(HttpRequest request, string key)
        {
            if (!request.Cookies.AllKeys.Contains(key))
                return null;

            HttpCookie httpCookie = request.Cookies.Get(key);
            return httpCookie != null ? httpCookie.Value : null;
        }

        /// <summary>
        /// Gets the cookie.
        /// </summary>
        /// <param name="requestBase">The request base.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public static string GetCookie(HttpRequestBase requestBase, string key)
        {
            if (!requestBase.Cookies.AllKeys.Contains(key))
                return null;

            HttpCookie httpCookie = requestBase.Cookies.Get(key);
            return httpCookie != null ? httpCookie.Value : null;
        }
    }
}