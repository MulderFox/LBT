using System;
using System.IO;
using LBT.Cache;
using LBT.Filters;
using LBT.ModelViews;
using LBT.Models;
using LBT.Resources;
using LBT.Services;
using PagedList;
using System.Web.Mvc;

namespace LBT.Controllers
{
    [InitializeSimpleMembership]
    [Authorize(Roles = AdminRoles)]
    public class VideoController : BaseController
    {
        public ActionResult Index(string sortOrder, int? page, int pageSize = PageSize)
        {
            int pageNumber;
            ProcessPaging(page, out pageNumber);

            var sortingNames = new[] {"Title"};
            ProcessSorting(sortOrder, sortingNames);

            PopulatePageSize(pageSize);

            VideoIndex[] videos = VideoIndex.GetIndex(Db, sortOrder);
            return View(videos.ToPagedList(pageNumber, PageSize));
        }

        public ActionResult Create()
        {
            var videoCreate = new VideoCreate(Server);

            PopulateUserIds();

            return View(videoCreate);
        }

        [HttpPost]
        public ActionResult Create(VideoCreate videoCreate)
        {
            videoCreate.Validate(this);

            if (ModelState.IsValid)
            {
                int[] userIds;
                Video video = videoCreate.GetModel(out userIds);
                VideoCache.Insert(Db, video, userIds);
                return RedirectToAction("Index");
            }

            PopulateUserIds(videoCreate.UserIds);

            return View(videoCreate);
        }

        public ActionResult Edit(int id = 0)
        {
            Video video = VideoCache.GetDetail(Db, id);
            if (!IsAccess(video))
            {
                return RedirectToAccessDenied();
            }

            VideoEdit videoEdit = VideoEdit.GetModelView(video, Server);

            PopulateUserIds(videoEdit.UserIds);

            return View(videoEdit);
        }

        [HttpPost]
        public ActionResult Edit(VideoEdit videoEdit)
        {
            videoEdit.Validate(this);

            if (ModelState.IsValid)
            {
                int[] userIds;
                Video video = videoEdit.GetModel(out userIds);
                bool success = VideoCache.Update(Db, video, userIds);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("Index");
            }

            PopulateUserIds(videoEdit.UserIds);

            return View(videoEdit);
        }

        public ActionResult Details(int id = 0)
        {
            Video video = VideoCache.GetDetail(Db, id);
            if (!IsAccess(video))
            {
                return RedirectToAccessDenied();
            }

            VideoDetails videoDetails = VideoDetails.GetModelView(video);

            return View(videoDetails);
        }

        public ActionResult Delete(int id = 0)
        {
            Video video = VideoCache.GetDetail(Db, id);
            if (!IsAccess(video))
            {
                return RedirectToAccessDenied();
            }

            VideoDelete videoDelete = VideoDelete.GetModelView(video);

            return View(videoDelete);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Video video = VideoCache.GetDetail(Db, id);
            if (!IsAccess(video))
            {
                return RedirectToAccessDenied();
            }

            VideoDelete videoDelete = VideoDelete.GetModelView(video);

            ModelState.Merge(videoDelete.Validate(Db, Server));

            if (ModelState.IsValid)
            {
                DeleteResult deleteResult = VideoCache.Delete(Db, video);

                switch (deleteResult)
                {
                    case DeleteResult.Ok:
                        return RedirectToAction("Index");

                    case DeleteResult.AuthorizationFailed:
                        return RedirectToAccessDenied();

                    case DeleteResult.DbFailed:
                        ModelState.AddModelError(BaseCache.TitleField, ValidationResource.Global_DeleteRecord_ErrorMessage);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
            }

            return View(videoDelete);
        }

        [AllowAnonymous]
        public ActionResult Player(string token)
        {
            VideoPlayer videoPlayer = VideoPlayer.GetModelView(Db, token);
            if (videoPlayer == null)
            {
                return RedirectToAccessDenied();
            }

            return View(videoPlayer);
        }

        [AllowAnonymous]
        public ActionResult VideoDataDetails(int id = 0)
        {
            return VideoData(id, null);
        }

        [AllowAnonymous]
        public ActionResult VideoDataToken(string token)
        {
            return VideoData(0, token);
        }

        private ActionResult VideoData(int id, string token)
        {
            var streamVideoStream = new StreamVideoStream(IsAdmin, id, token, Server);
            if (!streamVideoStream.IsValid)
                throw new Exception("Cannot process video stream.");

            var videoFileInfo = new FileInfo(streamVideoStream.VideoAbsoluteFilePath);
            var videoContentType = HttpResponseBaseService.GetContentType(streamVideoStream.VideoRelativeFilePath);

            return new RangeFilePathResult(videoContentType, videoFileInfo.FullName, videoFileInfo.LastWriteTimeUtc, videoFileInfo.Length);
        }

        private bool IsAccess(Video video)
        {
            return video != null;
        }
    }
}