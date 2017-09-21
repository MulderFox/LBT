using System.Web.Mvc;

namespace LBT.Filters
{
    public class PreventDirectAccessAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            object value = filterContext.RouteData.Values["fromAppErrorEvent"];
            if (!(value is bool && (bool)value))
            {
                filterContext.Result = new ViewResult { ViewName = "Error404" };                
            }
        }
    }
}