using LBT.Cache;
using LBT.Controllers;
using LBT.Helpers;
using LBT.Resources;
using LBT.Services;
using LBT.Services.ffMpeg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace LBT.ModelViews
{
    public abstract partial class BaseModelView
    {
        public const string ClaAccessExpiredClass = "ClaAccessExpired";
        public const string MeetingAttendeeFreezedClass = "MeetingAttendeeFreezed";
        public const string MeetingAttendeeRegisteredClass = "MeetingAttendeeRegistered";
        public const string MeetingAttendeeReservedClass = "MeetingAttendeeReserved";
        public const string OldLastContactClass = "OldLastContact";
        public const string ShowUsingCookiesClass = "ShowUsingCookies";

        public const string NullDisplayText = "---";
        public const string NullDisplayTextForInteger = "0";

        public bool IsValid { get { return OnIsValid(); } }

        protected BaseController BaseController;

        protected static string ProcessVocabulary(string template, Dictionary<string, string> vocabulary)
        {
            return vocabulary.Aggregate(template,
                                        (current, vocabularyPair) =>
                                        current.Replace(String.Format("{{{0}}}", vocabularyPair.Key),
                                                        vocabularyPair.Value));
        }

        protected static void ProcessPaging(int? page, out int pageNumber)
        {
            pageNumber = page ?? 1;
        }

        protected virtual bool OnIsValid()
        {
            return BaseController == null || BaseController.ModelState.IsValid;
        }

        protected string FixMultilines(string multilineText)
        {
            IEnumerable<string> multilines = multilineText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).Where(s => !String.IsNullOrEmpty(s));
            string newMultilineText = String.Join(Environment.NewLine, multilines);
            return newMultilineText;
        }

        protected void SetStatusMessage(string message)
        {
            BaseController.TempData[BaseController.StatusMessageTempKey] = message;
        }

        protected void SetSuccessMessage(string message)
        {
            BaseController.TempData[BaseController.SuccessMessageTempKey] = message;
        }

        protected string GetStatusMessage()
        {
            object message;
            if (!BaseController.TempData.TryGetValue(BaseController.StatusMessageTempKey, out message))
                return null;

            return message.ToString();
        }

        protected string GetSuccessMessage()
        {
            object message;
            if (!BaseController.TempData.TryGetValue(BaseController.SuccessMessageTempKey, out message))
                return null;

            return message.ToString();
        }
    }

    public abstract class StoreModelView : BaseModelView
    {
        private const string VideosStore = "Videos";
        private const string ManualsStore = "Store\\Manuals";

        public string RelativeFilePath { get; set; }

        public int FolderRemainMB { get; set; }

        public string VideoFileInfoText { get { return String.Format(ViewResource.Video_FileInfo_Text, FolderRemainMB); } }

        public string ManualFileInfoText { get { return String.Format(ViewResource.Manual_FileInfo_Text, FolderRemainMB); } }

        public int VideoDuration { get; set; }

        protected void CalculateFolderRemainMB(HttpServerUtilityBase server, FileStore fileStore)
        {
            string absoluteFolderPath;
            string relativeFolderPath;
            int folderQuota;
            GetFolderPath(server, fileStore, out absoluteFolderPath, out relativeFolderPath, out folderQuota);

            var folderSize = (int)Math.Ceiling(FileService.DirSize(absoluteFolderPath, true) / 1048576M);
            FolderRemainMB = folderQuota - folderSize;
        }

        protected void ValidateFile(BaseController baseController, HttpPostedFileBase file, FileStore fileStore, int maxContentLengthMB, string previousFileName = null, string[] allowedExtensions = null)
        {
            if (!baseController.ModelState.IsValid)
                return;

            if (file == null)
            {
                baseController.ModelState.AddModelError(BaseCache.FileField, String.Format(ValidationResource.Global_Required_ErrorMessage, FieldResource.Global_ImportFile_Name));
                return;
            }

            string fixedFileName = Path.GetFileName(file.FileName) ?? "___error.nfo";
            if (allowedExtensions != null && allowedExtensions.Length > 0)
            {
                string fileExtension = Path.GetExtension(fixedFileName);
                if (allowedExtensions.All(ae => !ae.Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase)))
                {
                    baseController.ModelState.AddModelError(BaseCache.FileField, ValidationResource.Global_IncorrectFileFormat_ErrorMessage);
                    return;
                }
            }

            if (fixedFileName.Length > 100)
            {
                baseController.ModelState.AddModelError(BaseCache.FileField, ValidationResource.Global_BadFileName_ErrorMessage);
                return;
            }

            if (file.ContentLength == 0)
            {
                baseController.ModelState.AddModelError(BaseCache.FileField, ValidationResource.Global_FileIsEmpty_ErrorMessage);
                return;
            }

            if (file.ContentLength > 1024 * 1024 * (long)maxContentLengthMB)
            {
                baseController.ModelState.AddModelError(BaseCache.FileField, String.Format(ValidationResource.Global_FileIsTooLarge_ErrorMessage, maxContentLengthMB));
                return;
            }

            if (fileStore == FileStore.None)
                return;

            if (file.ContentLength > FolderRemainMB * (long)1048576)
            {
                baseController.ModelState.AddModelError(BaseCache.FileField, ValidationResource.Global_StoreIsFull_ErrorMessage);
                return;
            }

            string absoluteFolderPath;
            string relativeFolderPath;
            int folderQuota;
            GetFolderPath(baseController.Server, fileStore, out absoluteFolderPath, out relativeFolderPath, out folderQuota);

            string absolutePreviousFilePath = String.IsNullOrEmpty(previousFileName) ? null : Path.Combine(absoluteFolderPath, previousFileName);
            string absoluteFilePath = Path.Combine(absoluteFolderPath, fixedFileName);
            if (File.Exists(absoluteFilePath) && !absoluteFilePath.Equals(absolutePreviousFilePath, StringComparison.InvariantCultureIgnoreCase))
            {
                baseController.ModelState.AddModelError(BaseCache.FileField, ValidationResource.Global_FileExists_ErrorMessage);
                return;
            }

            if (!String.IsNullOrEmpty(absolutePreviousFilePath) && File.Exists(absolutePreviousFilePath))
            {
                File.Delete(absolutePreviousFilePath);
            }

            try
            {
                file.SaveAs(absoluteFilePath);

                RelativeFilePath = Path.Combine(relativeFolderPath, fixedFileName);

                var converter = new Converter(baseController.Server);
                VideoFile videoFile = converter.GetVideoInfo(absoluteFilePath);
                VideoDuration = Convert.ToInt32(videoFile.Duration.TotalSeconds);
            }
            catch (Exception e)
            {
                try
                {
                    if (File.Exists(absoluteFilePath))
                    {
                        File.Delete(absoluteFilePath);
                    }
                }
                catch (Exception ex)
                {
                    Logger.SetLog(ex);
                }

                Logger.SetLog(e);
                baseController.ModelState.AddModelError(BaseCache.FileField, ValidationResource.Global_FileCannotBeSaved_ErrorMessage);
            }
        }

        private void GetFolderPath(HttpServerUtilityBase server, FileStore fileStore, out string absoluteFolderPath, out string relativeFolderPath, out int folderQuota)
        {
            switch (fileStore)
            {
                case FileStore.Videos:
                    absoluteFolderPath = FileService.GetAbsoluteFilePath(server, VideosStore);
                    relativeFolderPath = VideosStore;
                    folderQuota = Properties.Settings.Default.VideosFolderQuotaMB;
                    break;

                case FileStore.Manuals:
                    absoluteFolderPath = FileService.GetAbsoluteFilePath(server, ManualsStore);
                    relativeFolderPath = ManualsStore;
                    folderQuota = Properties.Settings.Default.ManualsFolderQuotaMB;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("fileStore");
            }
        }
    }

    public enum Access
    {
        Read,
        Write
    }

    public enum FileStore
    {
        None,
        Videos,
        Manuals
    }
}