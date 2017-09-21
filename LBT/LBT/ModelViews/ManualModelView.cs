using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;
using LBT.Cache;
using LBT.Controllers;
using LBT.Helpers;
using LBT.Models;
using LBT.Resources;
using LBT.Services;
using PagedList;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LBT.ModelViews
{
    public sealed class ManualIndex : BaseModelView
    {
        public int ManualId { get; set; }

        public string ManualTypeTitle { get; set; }

        public string Title { get; set; }

        public int Order { get; set; }

        private ManualIndex(Manual manual)
        {
            ManualId = manual.ManualId;
            ManualTypeTitle = manual.ManualType.Title;
            Title = manual.Title;
            Order = manual.Order;
        }

        public static IPagedList<ManualIndex> GetViewModel(BaseController baseController, int? page, int pageSize)
        {
            int pageNumber;
            ProcessPaging(page, out pageNumber);

            PopulatePageSize(baseController.ViewBag, pageSize);

            ManualIndex[] manuals = ManualCache.GetIndex(baseController.Db).Select(m => new ManualIndex(m)).ToArray();
            IPagedList<ManualIndex> viewModel = manuals.ToPagedList(pageNumber, pageSize);
            return viewModel;
        }
    }

    public class ManualCreateEdit : StoreModelView
    {
        protected readonly string[] AllowedExtensions;

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "ManualType_Title_Name", ResourceType = typeof(FieldResource))]
        public int ManualTypeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Order_Name", ResourceType = typeof(FieldResource))]
        public int Order { get; set; }

        [Display(Name = "Manual_IsDownloadable_Name", ResourceType = typeof(FieldResource))]
        public bool IsDownloadable { get; set; }

        [Display(Name = "Manual_IsAccessForAuthGuest_Name", ResourceType = typeof(FieldResource))]
        public bool IsAccessForAuthGuest { get; set; }

        public string AcceptedExtensions { get { return String.Join(", ", AllowedExtensions); } }

        public ManualCreateEdit()
        {
            AllowedExtensions = new[] { ".pdf", ".mp4", ".xlsx", ".docx", ".pptx", ".ogg" };
        }

        protected void Ensure(BaseController baseController)
        {
            CalculateFolderRemainMB(baseController.Server, FileStore.Manuals);
            PopulateManualTypeId(baseController.ViewBag, baseController.Db, ManualTypeId == 0 ? null : (object)ManualTypeId);
        }

        protected void ValidateManualType(BaseController baseController)
        {
            if (ManualTypeId == 0)
            {
                baseController.ModelState.AddModelError(BaseCache.ManualTypeIdField, String.Format(ValidationResource.Global_Required_ErrorMessage, FieldResource.ManualType_Title_Name));
            }
        }
    }

    public sealed class ManualCreate : ManualCreateEdit
    {
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Manual_File_Name", ResourceType = typeof(FieldResource))]
        public HttpPostedFileBase File { get; set; }

        public static ManualCreate GetViewModel(BaseController baseController)
        {
            var manualCreate = new ManualCreate();
            manualCreate.Ensure(baseController);

            return manualCreate;
        }

        public static ManualCreate GetViewModel(BaseController baseController, ManualCreate manualCreate)
        {
            manualCreate.Ensure(baseController);
            manualCreate.Validate(baseController);

            if (!baseController.ModelState.IsValid)
                return manualCreate;

            Manual manual = manualCreate.GetModel();
            ManualCache.Insert(baseController.Db, manual);

            return manualCreate;
        }

        private void Validate(BaseController baseController)
        {
            ValidateManualType(baseController);
            ValidateFile(baseController, File, FileStore.Manuals, Properties.Settings.Default.MaxManualUploadSizeMB, allowedExtensions: AllowedExtensions);
        }

        private Manual GetModel()
        {
            if (String.IsNullOrEmpty(RelativeFilePath))
                throw new Exception("Cannot process empty Relative file path for manual file.");

            var manual = new Manual
            {
                ManualTypeId = ManualTypeId,
                Title = Title,
                Order = Order,
                RelativeFilePath = RelativeFilePath,
                IsDownloadable = IsDownloadable,
                IsAccessForAuthGuest = IsAccessForAuthGuest
            };
            return manual;
        }
    }

    public sealed class ManualEdit : ManualCreateEdit
    {
        public int ManualId { get; set; }

        [Display(Name = "Manual_File_Name", ResourceType = typeof(FieldResource))]
        public HttpPostedFileBase File { get; set; }

        public ManualEdit()
        {

        }

        private ManualEdit(Manual manual)
        {
            ManualId = manual.ManualId;
            Title = manual.Title;
            ManualTypeId = manual.ManualTypeId;
            RelativeFilePath = manual.RelativeFilePath;
            Order = manual.Order;
            IsDownloadable = manual.IsDownloadable;
            IsAccessForAuthGuest = manual.IsAccessForAuthGuest;
        }

        public static ManualEdit GetViewModel(BaseController baseController, int id)
        {
            Manual manual = ManualCache.GetDetail(baseController.Db, id);
            if (manual == null)
                return null;

            var manualEdit = new ManualEdit(manual);
            manualEdit.Ensure(baseController);

            return manualEdit;
        }

        public static ManualEdit GetViewModel(BaseController baseController, ManualEdit manualEdit)
        {
            manualEdit.Ensure(baseController);
            manualEdit.Validate(baseController);

            if (!baseController.ModelState.IsValid)
                return manualEdit;

            Manual manual = manualEdit.GetModel();
            bool success = ManualCache.Update(baseController.Db, manual);
            return success ? manualEdit : null;
        }

        private void Validate(BaseController baseController)
        {
            ValidateManualType(baseController);
            ValidateManualFile(baseController);
        }

        private void ValidateManualFile(BaseController baseController)
        {
            if (File == null)
                return;

            var dbManual = ManualCache.GetDetail(baseController.Db, ManualId);
            if (dbManual == null)
                throw new Exception("Cannot validate ManualEdit due to empty ManualId.");

            RelativeFilePath = dbManual.RelativeFilePath;

            string previousFileName = Path.GetFileName(RelativeFilePath);
            ValidateFile(baseController, File, FileStore.Manuals, Properties.Settings.Default.MaxManualUploadSizeMB, previousFileName, AllowedExtensions);
        }

        private Manual GetModel()
        {
            var manual = new Manual
                             {
                                 ManualId = ManualId,
                                 Title = Title,
                                 ManualTypeId = ManualTypeId,
                                 RelativeFilePath = RelativeFilePath,
                                 Order = Order,
                                 IsDownloadable = IsDownloadable,
                                 IsAccessForAuthGuest = IsAccessForAuthGuest
                             };
            return manual;
        }
    }

    public sealed class ManualDetail : BaseModelView
    {
        public int ManualId { get; set; }

        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Display(Name = "ManualType_Title_Name", ResourceType = typeof(FieldResource))]
        public string ManualTypeTitle { get; set; }

        [Display(Name = "Global_Order_Name", ResourceType = typeof(FieldResource))]
        public int Order { get; set; }

        public string Url { get; set; }

        [Display(Name = "Manual_IsDownloadable_Name", ResourceType = typeof(FieldResource))]
        public bool IsDownloadable { get; set; }

        [Display(Name = "Manual_IsAccessForAuthGuest_Name", ResourceType = typeof(FieldResource))]
        public bool IsAccessForAuthGuest { get; set; }

        private ManualDetail(BaseController baseController, Manual manual)
        {
            ManualId = manual.ManualId;
            Title = manual.Title;
            ManualTypeTitle = manual.ManualType.Title;
            Order = manual.Order;
            Url = baseController.Url.Action("Player", "Manual", new { id = manual.ManualId });
            IsDownloadable = manual.IsDownloadable;
            IsAccessForAuthGuest = manual.IsAccessForAuthGuest;
        }

        public static ManualDetail GetViewModel(BaseController baseController, int id)
        {
            Manual manual = ManualCache.GetDetail(baseController.Db, id);
            if (manual == null)
                return null;

            var manualDetail = new ManualDetail(baseController, manual);
            return manualDetail;
        }
    }

    public sealed class ManualDelete : BaseModelView
    {
        public int ManualId { get; set; }

        [Display(Name = "ManualType_Title_Name", ResourceType = typeof(FieldResource))]
        public string ManualType { get; set; }

        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        private readonly string _relativeFilePath;

        private ManualDelete(Manual manual)
        {
            ManualId = manual.ManualId;
            ManualType = manual.ManualType.Title;
            Title = manual.Title;
            _relativeFilePath = manual.RelativeFilePath;
        }

        public static ManualDelete GetViewModel(BaseController baseController, int id)
        {
            Manual manual = ManualCache.GetDetail(baseController.Db, id);
            if (manual == null)
                return null;

            var manualDelete = new ManualDelete(manual);
            return manualDelete;
        }

        public static ManualDelete GetViewModel(BaseController baseController, int id, out DeleteResult deleteResult)
        {
            deleteResult = DeleteResult.Ok;

            Manual manual = ManualCache.GetDetail(baseController.Db, id);
            if (manual == null)
                return null;

            var manualDelete = new ManualDelete(manual);
            manualDelete.Process(baseController);

            if (baseController.ModelState.IsValid)
            {
                deleteResult = ManualCache.Delete(baseController.Db, manual);

                switch (deleteResult)
                {
                    case DeleteResult.Ok:
                    case DeleteResult.AuthorizationFailed:
                        return manualDelete;

                    case DeleteResult.DbFailed:
                        baseController.ModelState.AddModelError(BaseCache.TitleField, ValidationResource.Global_DeleteRecord_ErrorMessage);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return manualDelete;
        }

        private void Process(BaseController baseController)
        {
            ProcessManualFile(baseController);
        }

        private void ProcessManualFile(Controller controller)
        {
            if (String.IsNullOrEmpty(_relativeFilePath))
                return;

            string absoluteFilePath = FileService.GetAbsoluteFilePath(controller.Server, _relativeFilePath);

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
                controller.ModelState.AddModelError(BaseCache.TitleField, ValidationResource.Video_CannotRemoveFile_ErrorMessage);
            }
        }
    }

    public sealed class ManualPlayer : BaseModelView
    {
        public enum PlayerType
        {
            None = 0,
            Video
        }

        public bool NeedPlayer { get; private set; }

        public string PlayerViewName
        {
            get
            {
                switch (_contentType)
                {
                    case HttpResponseBaseService.ContentType.Mp4:
                    case HttpResponseBaseService.ContentType.Ogg:
                        return "PlayerVideo";

                    default:
                        return "PlayerUnknown";
                }
            }
        }

        public ActionResult PlayerDataDefaultActionResult
        {
            get { return _playerDataDefaultActionResult ?? (_playerDataDefaultActionResult = GetPlayerDataDefaultActionResult()); }
        }
        private ActionResult _playerDataDefaultActionResult;

        public ActionResult PlayerDataActionResult
        {
            get { return _playerDataActionResult ?? (_playerDataActionResult = GetPlayerDataActionResult()); }
        }
        private ActionResult _playerDataActionResult;

        public string Title { get; private set; }

        public string PlayerDataUrl { get; private set; }

        public string AbsoluteFilePath { get { return _fileInfo == null ? String.Empty : _fileInfo.FullName; } }

        public string FileName { get { return _fileInfo == null ? String.Empty : _fileInfo.Name; } }

        public string MimeContentType { get; private set; }

        private readonly BaseController _baseController;
        private readonly Manual _manual;

        private bool _isValid;
        private HttpResponseBaseService.ContentType _contentType;
        private FileInfo _fileInfo;

        private ManualPlayer(BaseController baseController, Manual manual)
        {
            _isValid = true;
            _baseController = baseController;
            _manual = manual;
            _contentType = HttpResponseBaseService.ContentType.Txt;

            if (baseController == null || manual == null || (!manual.IsAccessForAuthGuest && baseController.IsAuthenticatedGuest))
            {
                _isValid = false;
                return;
            }

            Title = manual.Title;
            PlayerDataUrl = baseController.Url.Action("PlayerData", "Manual", new { id = manual.ManualId });

            ProcessFindFilePath();
            CheckPlayer();
        }

        public static ManualPlayer GetViewModel(BaseController baseController, int id)
        {
            Manual manual = ManualCache.GetDetail(baseController.Db, id);
            if (manual == null)
                return null;

            var manualPlayer = new ManualPlayer(baseController, manual);
            return manualPlayer;
        }

        public static ManualPlayer GetViewModel(BaseController baseController, string token)
        {
            string decryptedToken = Cryptography.Decrypt(token);
            if (String.IsNullOrEmpty(decryptedToken))
                return null;

            int id;
            if (!Int32.TryParse(decryptedToken, out id))
                return null;

            var manualPlayer = GetViewModel(baseController, id);
            return manualPlayer;
        }

        public void SetContentToView()
        {
            if (!IsValid)
                return;

            SetContentDisposition(true);
        }

        public void SetContentToDownload()
        {
            if (!IsValid)
                return;

            SetContentDisposition(false);
        }

        protected override bool OnIsValid()
        {
            return _isValid;
        }

        private void ProcessFindFilePath()
        {
            if (!_isValid)
                return;

            string filePath = FileService.GetAbsoluteFilePath(_baseController.Server, _manual.RelativeFilePath);
            _fileInfo = new FileInfo(filePath);
            if (!_fileInfo.Exists)
            {
                _isValid = false;
            }

            MimeContentType = HttpResponseBaseService.GetContentType(_fileInfo.FullName, out _contentType);
        }

        private void CheckPlayer()
        {
            if (!IsValid)
                return;

            switch (_contentType)
            {
                case HttpResponseBaseService.ContentType.Txt:
                case HttpResponseBaseService.ContentType.Pdf:
                case HttpResponseBaseService.ContentType.Xlsx:
                case HttpResponseBaseService.ContentType.Docx:
                case HttpResponseBaseService.ContentType.Pptx:
                    NeedPlayer = false;
                    break;

                case HttpResponseBaseService.ContentType.Mp4:
                case HttpResponseBaseService.ContentType.Ogg:
                    NeedPlayer = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private ActionResult GetPlayerDataDefaultActionResult()
        {
            switch (_contentType)
            {
                case HttpResponseBaseService.ContentType.Mp4:
                case HttpResponseBaseService.ContentType.Ogg:
                    string defaultVideoFilePath = FileService.GetAbsoluteFilePath(_baseController.Server, StreamVideoStream.NoVideoFilePath);
                    var fileInfo = new FileInfo(defaultVideoFilePath);
                    var contentType = HttpResponseBaseService.GetContentType(defaultVideoFilePath);
                    return new RangeFilePathResult(contentType, fileInfo.FullName, fileInfo.LastWriteTimeUtc, fileInfo.Length);

                default:
                    throw new Exception("Cannot generate default action result.");
            }
        }

        private ActionResult GetPlayerDataActionResult()
        {
            if (!_isValid)
                return GetPlayerDataDefaultActionResult();

            switch (_contentType)
            {
                case HttpResponseBaseService.ContentType.Mp4:
                case HttpResponseBaseService.ContentType.Ogg:
                    return new RangeFilePathResult(MimeContentType, _fileInfo.FullName, _fileInfo.LastWriteTimeUtc, _fileInfo.Length);

                default:
                    throw new Exception("Cannot generate action result.");
            }
        }

        private void SetContentDisposition(bool inline)
        {
            var contentDisposition = new ContentDisposition
            {
                FileName = FileName,
                Inline = inline
            };
            _baseController.Response.AppendHeader("Content-Disposition", contentDisposition.ToString());
        }
    }

    public sealed class ManualDashboard : BaseModelView
    {
        public class ManualTypeItem
        {
            private const string ManualTypesOddBlockCssClass = "ManualTypesOddBlock";
            private const string ManualTypesEvenBlockCssClass = "ManualTypesEvenBlock";

            public ManualType ManualType { get; private set; }
            public Manual[] Manuals { get; private set; }
            public bool IsMore { get; private set; }
            public string CssClass { get; private set; }

            public ManualTypeItem(ManualType manualType, Manual[] manuals, bool isMore, bool isOddBlock)
            {
                ManualType = manualType;
                Manuals = manuals;
                IsMore = isMore;
                CssClass = isOddBlock ? ManualTypesOddBlockCssClass : ManualTypesEvenBlockCssClass;
            }
        }

        public class Manual
        {
            public string Title { get; private set; }
            public string Token { get; private set; }
            public bool CanRead { get; private set; }
            public bool CanDownload { get; private set; }

            public Manual(BaseController baseController, Models.Manual manual)
            {
                string token = Cryptography.Encrypt(manual.ManualId.ToString(CultureInfo.InvariantCulture));
                bool canRead = !baseController.IsAuthenticatedGuest || baseController.IsAuthenticatedGuest && manual.IsAccessForAuthGuest;
                bool canDownload = canRead && manual.IsDownloadable;

                Title = manual.Title;
                Token = token;
                CanRead = canRead;
                CanDownload = canDownload;
            }
        }

        public class ManualType
        {
            public int Id { get; private set; }
            public string Title { get; private set; }

            public ManualType(int id, string title)
            {
                Id = id;
                Title = title;
            }
        }

        public ManualTypeItem[] ManualTypeItems { get; private set; }

        public ManualType[] ManualTypes { get; private set; }

        private readonly BaseController _baseController;
        private bool _isOddBlock = true;

        public ManualDashboard(BaseController baseController, IEnumerable<Models.ManualType> manualTypes)
        {
            _baseController = baseController;
            Models.ManualType[] orderedManualTypes = manualTypes.OrderBy(mt => mt.Order).ToArray();
            ManualTypeItems = orderedManualTypes.Select(GetManualTypeItem).ToArray();
            ManualTypes = orderedManualTypes.Select(GetManualType).ToArray();
        }

        public static ManualDashboard GetViewModel(BaseController baseController)
        {
            Models.ManualType[] manualTypes = ManualTypeCache.GetIndex(baseController.Db);
            if (manualTypes == null)
                return null;

            var manualDashboard = new ManualDashboard(baseController, manualTypes);
            return manualDashboard;
        }

        private static ManualType GetManualType(Models.ManualType manualType)
        {
            var localManualType = new ManualType(manualType.ManualTypeId, manualType.Title);
            return localManualType;
        }

        private ManualTypeItem GetManualTypeItem(Models.ManualType manualType)
        {
            var localManualType = GetManualType(manualType);
            Manual[] manuals = manualType.Manuals.OrderBy(m => m.Order).Take(5).Select(GetManual).ToArray();
            bool isMore = manualType.Manuals.Count > 5;
            var manualTypeItem = new ManualTypeItem(localManualType, manuals, isMore, _isOddBlock);
            _isOddBlock = !_isOddBlock;
            return manualTypeItem;
        }

        private Manual GetManual(Models.Manual manual)
        {
            var localManual = new Manual(_baseController, manual);
            return localManual;
        }
    }

    public sealed class ManualsByTypeIndex : BaseModelView
    {
        public string Title { get; private set; }

        public IPagedList<ManualDashboard.Manual> Manuals { get; private set; }

        private ManualsByTypeIndex(BaseController baseController, ManualType manualType, int pageNumber, int pageSize)
        {
            Title = String.Format("{0} - {1}", ViewResource.ManualsByType_Index_Title_Text, manualType.Title);
            Manuals = manualType.Manuals.OrderBy(m => m.Order).Select(m => new ManualDashboard.Manual(baseController, m)).ToPagedList(pageNumber, pageSize);
        }

        public static ManualsByTypeIndex GetViewModel(BaseController baseController, int id, int? page, int pageSize)
        {
            int pageNumber;
            ProcessPaging(page, out pageNumber);

            PopulatePageSize(baseController.ViewBag, pageSize);

            ManualType manualType = ManualTypeCache.GetDetail(baseController.Db, id);
            if (manualType == null)
                return null;

            var viewModel = new ManualsByTypeIndex(baseController, manualType, pageNumber, pageSize);
            return viewModel;
        }
    }
}