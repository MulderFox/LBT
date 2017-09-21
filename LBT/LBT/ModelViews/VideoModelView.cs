using LBT.Cache;
using LBT.Controllers;
using LBT.DAL;
using LBT.Handlers;
using LBT.Helpers;
using LBT.Models;
using LBT.Resources;
using LBT.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LBT.ModelViews
{
    public sealed class VideoIndex : BaseModelView
    {
        public int VideoId { get; set; }

        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        private VideoIndex(Video video)
        {
            VideoId = video.VideoId;
            Title = video.Title;
        }

        public static VideoIndex[] GetIndex(DefaultContext db, string sortOrder)
        {
            VideoIndex[] videoIndexList = VideoCache.GetIndex(db, sortOrder).Select(v => new VideoIndex(v)).ToArray();
            return videoIndexList;
        }
    }

    public class VideoCreateEdit : StoreModelView
    {
        protected readonly string[] AllowedExtensions;

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Display(Name = "Video_VideoUsers_Name", ResourceType = typeof(FieldResource))]
        public int[] UserIds { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [StringLength(20, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_EmailSubject_Name", ResourceType = typeof(FieldResource))]
        public string EmailSubject { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_EmailBody_Name", ResourceType = typeof(FieldResource))]
        public string EmailBody { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Video_EmailSenderBody_Name", ResourceType = typeof(FieldResource))]
        public string EmailSenderBody { get; set; }

        [Display(Name = "Video_AllUsers_Name", ResourceType = typeof(FieldResource))]
        public bool AllUsers { get; set; }

        public string AcceptedExtensions { get { return String.Join(", ", AllowedExtensions); } }

        public VideoCreateEdit()
        {
            AllowedExtensions = new[] { ".mp4" };
            AllUsers = true;
        }

        protected void ValidateEmailBodies(BaseController baseController)
        {
            try
            {
                // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                String.Format(EmailBody, 0, 1, 2, 3);
                // ReSharper restore ReturnValueOfPureMethodIsNotUsed
            }
            catch
            {
                string errorMessage = String.Format(ValidationResource.Video_BadStringFormat_ErrorMessage, FieldResource.Global_EmailBody_Name);
                baseController.ModelState.AddModelError(BaseCache.EmailBodyField, errorMessage);
            }

            try
            {
                // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                String.Format(EmailSenderBody, 0, 1, 2);
                // ReSharper restore ReturnValueOfPureMethodIsNotUsed
            }
            catch
            {
                string errorMessage = String.Format(ValidationResource.Video_BadStringFormat_ErrorMessage, FieldResource.Video_EmailSenderBody_Name);
                baseController.ModelState.AddModelError(BaseCache.EmailSenderBodyField, errorMessage);
            }
        }
    }

    public sealed class VideoCreate : VideoCreateEdit
    {
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Video_File_Name", ResourceType = typeof(FieldResource))]
        public HttpPostedFileBase File { get; set; }

        public VideoCreate()
        {

        }

        public VideoCreate(HttpServerUtilityBase server)
        {
            CalculateFolderRemainMB(server, FileStore.Videos);
        }

        public Video GetModel(out int[] userIds)
        {
            if (String.IsNullOrEmpty(RelativeFilePath))
                throw new Exception("Cannot process empty Relative file path for video file.");

            userIds = UserIds ?? new int[0];

            var video = new Video
                            {
                                Title = Title,
                                RelativeFilePath = RelativeFilePath,
                                EmailSubject = EmailSubject,
                                EmailBody = EmailBody,
                                EmailSenderBody = EmailSenderBody,
                                AllUsers = AllUsers,
                                Duration = VideoDuration
                            };
            return video;
        }

        public void Validate(BaseController baseController)
        {
            ValidateEmailBodies(baseController);
            CalculateFolderRemainMB(baseController.Server, FileStore.Videos);
            ValidateFile(baseController, File, FileStore.Videos, Properties.Settings.Default.MaxVideoUploadSizeMB, allowedExtensions: AllowedExtensions);
        }
    }

    public sealed class VideoEdit : VideoCreateEdit
    {
        public int VideoId { get; set; }

        [Display(Name = "Video_File_Name", ResourceType = typeof(FieldResource))]
        public HttpPostedFileBase File { get; set; }

        public VideoEdit()
        {

        }

        private VideoEdit(Video video, HttpServerUtilityBase server)
        {
            VideoId = video.VideoId;
            Title = video.Title;
            RelativeFilePath = video.RelativeFilePath;
            EmailSubject = video.EmailSubject;
            EmailBody = video.EmailBody;
            EmailSenderBody = video.EmailSenderBody;
            AllUsers = video.AllUsers;
            VideoDuration = video.Duration;
            UserIds = video.VideoUsers.Select(vu => vu.UserProfileId).ToArray();

            CalculateFolderRemainMB(server, FileStore.Videos);
        }

        public static VideoEdit GetModelView(Video video, HttpServerUtilityBase server)
        {
            if (video == null)
                return null;

            var videoEdit = new VideoEdit(video, server);
            return videoEdit;
        }

        public Video GetModel(out int[] userIds)
        {
            userIds = UserIds ?? new int[0];

            var video = new Video
            {
                VideoId = VideoId,
                Title = Title,
                EmailSubject = EmailSubject,
                EmailBody = EmailBody,
                EmailSenderBody = EmailSenderBody,
                AllUsers = AllUsers,
                Duration = VideoDuration,
                RelativeFilePath = RelativeFilePath
            };
            return video;
        }

        public void Validate(BaseController baseController)
        {
            ValidateEmailBodies(baseController);
            ValidateVideoFile(baseController);
        }

        public void ValidateVideoFile(BaseController baseController)
        {
            if (File == null)
                return;

            var dbVideo = VideoCache.GetDetail(baseController.Db, VideoId);
            if (dbVideo == null)
                throw new Exception("Cannot validate VideoEdit due to empty VideoId.");

            RelativeFilePath = dbVideo.RelativeFilePath;
            CalculateFolderRemainMB(baseController.Server, FileStore.Videos);

            string previousFileName = Path.GetFileName(RelativeFilePath);
            ValidateFile(baseController, File, FileStore.Videos, Properties.Settings.Default.MaxVideoUploadSizeMB, previousFileName, AllowedExtensions);
        }
    }

    public sealed class VideoDetails : StoreModelView
    {
        public int VideoId { get; set; }

        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        public string VideoUrl { get; set; }

        public string VideoContentType { get; set; }

        [Display(Name = "Global_EmailSubject_Name", ResourceType = typeof(FieldResource))]
        public string EmailSubject { get; set; }

        [Display(Name = "Global_EmailBody_Name", ResourceType = typeof(FieldResource))]
        public string EmailBody { get; set; }

        [Display(Name = "Video_EmailSenderBody_Name", ResourceType = typeof(FieldResource))]
        public string EmailSenderBody { get; set; }

        public bool AllUsers { get; set; }

        [Display(Name = "Video_VideoUsers_Name", ResourceType = typeof(FieldResource))]
        public string[] VideoUsers { get; set; }

        private VideoDetails(Video video)
        {
            VideoId = video.VideoId;
            Title = video.Title;
            VideoUrl = String.Format("/Video/VideoDataDetails/{0}", VideoId);
            VideoContentType = HttpResponseBaseService.GetContentType(video.RelativeFilePath);
            EmailSubject = video.EmailSubject;
            EmailBody = video.EmailBody;
            EmailSenderBody = video.EmailSenderBody;
            AllUsers = video.AllUsers;
            VideoDuration = video.Duration;

            VideoUsers = video.VideoUsers.Select(vu => vu.UserProfile.FullName).OrderBy(vu => vu).ToArray();
        }

        public static VideoDetails GetModelView(Video video)
        {
            if (video == null)
                return null;

            var videoDetails = new VideoDetails(video);
            return videoDetails;
        }
    }

    public sealed class VideoDelete : BaseModelView
    {
        public int VideoId { get; set; }

        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        private readonly string _relativeFilePath;

        private VideoDelete(Video video)
        {
            VideoId = video.VideoId;
            Title = video.Title;
            _relativeFilePath = video.RelativeFilePath;
        }

        public static VideoDelete GetModelView(Video video)
        {
            if (video == null)
                return null;

            var videoDelete = new VideoDelete(video);
            return videoDelete;
        }

        public ModelStateDictionary Validate(DefaultContext db, HttpServerUtilityBase server)
        {
            var modelStateDictionary = new ModelStateDictionary();

            if (!String.IsNullOrEmpty(_relativeFilePath))
            {
                string absoluteFilePath = FileService.GetAbsoluteFilePath(server, _relativeFilePath);

                try
                {
                    if (File.Exists(absoluteFilePath))
                    {
                        File.Delete(absoluteFilePath);
                    }
                }
                catch (Exception e)
                {
                    Logger.SetLog(e);
                    modelStateDictionary.AddModelError(BaseCache.TitleField, ValidationResource.Video_CannotRemoveFile_ErrorMessage);
                }
            }

            return modelStateDictionary;
        }
    }

    public sealed class VideoPlayer : BaseModelView
    {
        public string Title { get; set; }

        public string VideoUrl { get; set; }

        public string VideoContentType { get; set; }

        private VideoPlayer(DefaultContext db, string token, int videoTokenId)
        {
            Title = ViewResource.Video_VideoNotFound_Text;
            VideoUrl = String.Format("/Video/VideoDataToken?token={0}", HttpUtility.UrlEncode(token));
            VideoContentType = HttpResponseBaseService.GetContentType();

            VideoToken videoToken = db.VideoTokens.Find(videoTokenId);
            if (videoToken == null)
                return;

            Title = videoToken.Video.Title;
            VideoContentType = HttpResponseBaseService.GetContentType(videoToken.Video.RelativeFilePath);
        }

        public static VideoPlayer GetModelView(DefaultContext db, string token)
        {
            if (String.IsNullOrEmpty(token))
                return null;

            string decryptedToken = Cryptography.Decrypt(token);
            if (String.IsNullOrEmpty(decryptedToken))
                return null;

            int videoTokenId;
            if (!Int32.TryParse(decryptedToken, out videoTokenId) || videoTokenId < 1)
                return null;

            var videoPlayer = new VideoPlayer(db, token, videoTokenId);
            return videoPlayer;
        }
    }
}