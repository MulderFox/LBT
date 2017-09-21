using LBT.Cache;
using LBT.DAL;
using LBT.Models;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;

namespace LBT.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class SetAutomaticLogoutAttribute : FilterAttribute, IAuthorizationFilter
    {
        public static KeyValuePair<int, string>[] AvailableLCIDList;

        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (filterContext.HttpContext == null)
                throw new Exception("filterContext.HttpContext");

            AutomaticLogoutIntervalType automaticLogoutIntervalType;
            bool isUserAutomaticLogoutIntervalSet;
            IPrincipal user = filterContext.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                var db = new DefaultContext();
                UserProfile userProfile = UserProfileCache.GetDetail(db, user.Identity.Name);
                automaticLogoutIntervalType = userProfile.AutomaticLogoutInterval;
                isUserAutomaticLogoutIntervalSet = true;
            }
            else
            {
                automaticLogoutIntervalType = AutomaticLogoutIntervalType.SixtyMinutes;
                isUserAutomaticLogoutIntervalSet = false;
            }

            filterContext.Controller.ViewBag.IsUserAutomaticLogoutIntervalSet = isUserAutomaticLogoutIntervalSet;
            filterContext.Controller.ViewBag.UserAutomaticLogoutInterval = (int)automaticLogoutIntervalType;
        }
    }
}