using System;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;
using LBT.Cache;
using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using System.Linq;

namespace LBT.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    // ReSharper disable InconsistentNaming
    public class CheckIsPoliciesAcceptedAttribute : FilterAttribute, IAuthorizationFilter
    // ReSharper restore InconsistentNaming
    {
        private const string CheckIsPoliciesAcceptedAction = "CheckIsPoliciesAccepted";
        private const string CheckBillingInformation = "CheckBillingInformation";
        private const string GetGoogleTokenAction = "GetGoogleToken";
        private const string LogOffAction = "LogOff";
        private const string AccountController = "Account";
        private const string CannotContinueAction = "CannotContinue";
        private const string HomeController = "Home";

        private readonly string[] _allowedAccountActionNames = new[] { CheckIsPoliciesAcceptedAction, CheckBillingInformation, LogOffAction, GetGoogleTokenAction };
        private readonly string[] _allowedHomeActionNames = new[] { CannotContinueAction };


        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (filterContext.HttpContext == null)
                throw new Exception("filterContext.HttpContext");

            IPrincipal user = filterContext.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
                return;

            string isAllowedPrivacy = Cookie.GetCookie(filterContext.HttpContext.Request, Cookie.IsAllowedPrivacyCookieKey);
            string isAllowedPrivacyForUser = Cookie.GetCookie(filterContext.HttpContext.Request, Cookie.IsAllowedPrivacyForUserCookieKey);
            if (!String.IsNullOrEmpty(isAllowedPrivacy) && isAllowedPrivacy == Cookie.CookieTrue
                && !String.IsNullOrEmpty(isAllowedPrivacyForUser) && isAllowedPrivacyForUser == user.Identity.Name)
                return;

            var actionDescriptor = filterContext.ActionDescriptor;
            if (actionDescriptor.ControllerDescriptor.ControllerName == AccountController && _allowedAccountActionNames.Contains(actionDescriptor.ActionName)
                || actionDescriptor.ControllerDescriptor.ControllerName == HomeController && _allowedHomeActionNames.Contains(actionDescriptor.ActionName))
                return;

            var db = new DefaultContext();
            UserProfile userProfile = UserProfileCache.GetDetail(db, user.Identity.Name);
            if (userProfile.IsPoliciesAccepted)
            {
                Cookie.SetNewCookie(Cookie.IsAllowedPrivacyCookieKey, Cookie.CookieTrue, filterContext.HttpContext.Response);
                Cookie.SetNewCookie(Cookie.IsAllowedPrivacyForUserCookieKey, user.Identity.Name, filterContext.HttpContext.Response);
                return;
            }

            string role = UserProfile.GetRoleTypeDbName(RoleType.AuthGuest);
            string action = user.IsInRole(role) ? CheckIsPoliciesAcceptedAction : CheckBillingInformation;

            filterContext.Result =
                new RedirectToRouteResult(
                    new RouteValueDictionary(new {action, controller = AccountController }));
        }
    }
}