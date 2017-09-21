using System.Web;
using System.Web.Routing;

namespace LBT.Handlers
{
    public class StreamVideoRouteHandler : IRouteHandler
    {
        /// <summary>
        /// Provides the object that processes the request.
        /// </summary>
        /// <returns>
        /// An object that processes the request.
        /// </returns>
        /// <param name="requestContext">An object that encapsulates information about the request.</param>
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new StreamVideoHandler();
        }
    }
}