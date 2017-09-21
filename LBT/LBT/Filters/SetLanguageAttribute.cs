using LBT.Cache;
using LBT.DAL;
using LBT.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace LBT.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class SetLanguageAttribute : FilterAttribute, IAuthorizationFilter
    {
        public static KeyValuePair<int, string>[] AvailableLCIDList;

        static SetLanguageAttribute()
        {
            AvailableLCIDList = new[]
                                    {
                                        new KeyValuePair<int, string>(1029, "Čeština"),
                                        //new KeyValuePair<int, string>(1033, "English")
                                    };
        }

        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (filterContext.HttpContext == null)
                throw new Exception("filterContext.HttpContext");

            int currentLcid = Thread.CurrentThread.CurrentCulture.LCID;
            int lcid = 1029;
            string requestLcid = filterContext.HttpContext.Request.QueryString["LCID"];
            IPrincipal user = filterContext.HttpContext.User;
            if (!String.IsNullOrEmpty(requestLcid))
            {
                filterContext.Controller.ViewBag.LCID = requestLcid;
                Int32.TryParse(requestLcid, out lcid);
            }
            else if (user.Identity.IsAuthenticated)
            {
                var db = new DefaultContext();
                UserProfile userProfile = UserProfileCache.GetDetail(db, user.Identity.Name);
                lcid = userProfile.LCID;
            }

            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(lcid);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }
    }
}