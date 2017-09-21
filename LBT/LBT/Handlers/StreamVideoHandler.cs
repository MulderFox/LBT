using LBT.Models;
using LBT.ModelViews;
using LBT.Services;
using System;
using System.Web;

namespace LBT.Handlers
{
    public class StreamVideoHandler : IHttpHandler
    {
        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests. </param>
        public void ProcessRequest(HttpContext context)
        {
            RoleType? roleType = context.User.Identity.IsAuthenticated ? UserProfile.GetRoleForUser(context.User.Identity.Name) : new RoleType?();
            bool isAdmin = roleType.HasValue && (roleType.Value == RoleType.Admin || roleType.Value == RoleType.AdminLeader);
            
            string idRequest = context.Request.QueryString["id"];
            int id;
            Int32.TryParse(idRequest ?? String.Empty, out id);

            string token = context.Request.QueryString["token"];

            var server = new HttpServerUtilityWrapper(context.Server);
            var streamVideoStream = new StreamVideoStream(isAdmin, id, token, server);
            if (!streamVideoStream.IsValid)
                throw new Exception("Cannot process video stream.");

            context.Response.Clear();
            context.Response.ContentType = HttpResponseBaseService.GetContentType(streamVideoStream.VideoRelativeFilePath);
            context.Response.TransmitFile(streamVideoStream.VideoRelativeFilePath);
        }

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.
        /// </returns>
        public bool IsReusable { get { return false; } }
    }
}