using LBT.Controllers;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LBT.Helpers;

namespace LBT.App_Start
{
    public static class ErrorConfig
    {
        public static void ShowCustomErrorPage(HttpServerUtility server, HttpContext context, HttpResponse response)
        {
            var exception = server.GetLastError();
            Logger.SetLog(exception);

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("fromAppErrorEvent", true);

            var httpException = exception as HttpException ?? new HttpException(500, "Internal Server Error", exception);
            int httpCode = httpException.GetHttpCode();
            switch (httpCode)
            {
                case 403:
                    routeData.Values.Add("action", "AccessDenied");
                    break;

                case 404:
                    routeData.Values.Add("action", "NotFound");
                    break;

                case 500:
                    routeData.Values.Add("action", "ServerError");
                    break;

                default:
                    routeData.Values.Add("action", "OtherHttpStatusCode");
                    routeData.Values.Add("httpStatusCode", httpCode);
                    break;
            }

            server.ClearError();

            response.Clear();
            
            IController controller = new ErrorController();
            controller.Execute(new RequestContext(new HttpContextWrapper(context), routeData));
        }
    }
}