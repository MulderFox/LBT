using LBT.Cache;
using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using PagedList;
using LBT.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using static System.String;

namespace LBT.ModelViews
{
    public static class MeetingCommon
    {
        public static string GetPaymentInfo(DefaultContext db, Meeting meeting, string lyonessId)
        {
            if (meeting.BankAccount == null)
                return $"  {MailResource.MeetingModelView_NoPaymentInfo_Text}";

            var stringBuilder = new StringBuilder();
            string paymentInfo = Format(MailResource.MeetingModelView_PaymentInfo_Text,
                                               meeting.BankAccount.AccountId,
                                               meeting.BankAccount.IBAN,
                                               meeting.BankAccount.SWIFT,
                                               meeting.SpecificSymbol,
                                               BankAccountHistory.GetVs(lyonessId),
                                               meeting.Price.GetValueOrDefault(),
                                               meeting.BankAccount?.CurrencyType);

            string[] paymentInfoRows = paymentInfo.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string paymentInfoRow in paymentInfoRows)
            {
                stringBuilder.AppendLine($"  {paymentInfoRow}");
            }

            if (meeting.SecondBankAccount == null)
            {
                return stringBuilder.ToString();
            }

            stringBuilder.AppendLine($"\n{ViewResource.Meeting_PaymentInfoOr_Text}\n");

            string secondPaymentInfo = Format(MailResource.MeetingModelView_PaymentInfo_Text,
                                                     meeting.SecondBankAccount.AccountId,
                                                     meeting.SecondBankAccount.IBAN,
                                                     meeting.SecondBankAccount.SWIFT,
                                                     meeting.SpecificSymbol,
                                                     BankAccountHistory.GetVs(lyonessId),
                                                     meeting.SecondPrice.GetValueOrDefault(),
                                                     meeting.SecondBankAccount.CurrencyType);

            paymentInfoRows = secondPaymentInfo.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string paymentInfoRow in paymentInfoRows)
            {
                stringBuilder.AppendLine($"  {paymentInfoRow}");
            }

            return stringBuilder.ToString();
        }

        public static string GetMeetingDetail(Meeting meeting)
        {
            var stringBuilder = new StringBuilder();
            string meetingDetail = Format(MailResource.MeetingModelView_MeetingDetail_Text,
                                                 meeting.Title,
                                                 meeting.Started,
                                                 meeting.Finished,
                                                 meeting.City,
                                                 meeting.AddressLine1,
                                                 meeting.PrimaryAndSecondaryOrganizerFullName,
                                                 meeting.Lecturer,
                                                 !IsNullOrEmpty(meeting.BringYourOwn) ? meeting.BringYourOwn : BaseModelView.NullDisplayText,
                                                 HtmlToText.ConvertHtml(meeting.Note));

            string[] meetingDetailRows = meetingDetail.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (string meetingDetailRow in meetingDetailRows)
            {
                stringBuilder.AppendLine($"  {meetingDetailRow}");
            }

            return stringBuilder.ToString();
        }
    }

    public class DashboardIndex : BaseModelView
    {
        public MeetingBusinessInfoIndex MeetingBusinessInfoIndex { get; set; }

        public MeetingWebinarIndex MeetingWebinarIndex { get; set; }

        public MeetingMspEveningIndex MeetingMspEveningIndex { get; set; }

        public MeetingSetkaniTymuIndex MeetingSetkaniTymuIndex { get; set; }

        public MeetingSkoleniDavidaKotaskaIndex MeetingSkoleniDavidaKotaskaIndex { get; set; }

        public MeetingOstatniIndex MeetingOstatniIndex { get; set; }
    }

    public abstract class MeetingIndex : BaseModelView
    {
        public int MeetingId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime Date { get; set; }

        public string Day { get; set; }

        public string Time { get; set; }

        public string Organizer { get; set; }

        public int FreeCapacity { get; set; }

        public string Lecturer { get; set; }

        public Access Access { get; set; }

        public bool Private { get; set; }

        protected MeetingIndex(MeetingCache.MeetingCacheBase meetingCacheBase, bool isAdmin, int userId)
        {
            MeetingId = meetingCacheBase.MeetingId;
            Date = meetingCacheBase.Started;
            Day = meetingCacheBase.Started.ToString("ddd", CultureInfo.CreateSpecificCulture("cs-CZ"));
            Time = meetingCacheBase.Started.ToString("HH:mm");
            Organizer = meetingCacheBase.PrimaryAndSecondaryOrganizerFullName;
            FreeCapacity = meetingCacheBase.Capacity - meetingCacheBase.FillCapacity;
            Lecturer = meetingCacheBase.Lecturer;
            Access = isAdmin || meetingCacheBase.OrganizerId == userId || meetingCacheBase.SecondaryOrganizerId == userId ? Access.Write : Access.Read;
            Private = meetingCacheBase.Private;
        }
    }

    public class MeetingBusinessInfoIndex : MeetingIndex
    {
        public string City { get; set; }

        public string Street { get; set; }

        private MeetingBusinessInfoIndex(MeetingCache.MeetingCacheBusinessInfo meetingCacheBusinessInfo, bool isAdmin, int userId)
            : base(meetingCacheBusinessInfo, isAdmin, userId)
        {
            City = meetingCacheBusinessInfo.City;
            Street = meetingCacheBusinessInfo.AddressLine1;
        }

        public static MeetingBusinessInfoIndex GetNearestMeeting(DefaultContext db, bool isAdmin, int userId)
        {
            var meetings = (IEnumerable<MeetingCache.MeetingCacheBusinessInfo>)(isAdmin
                                                                                     ? MeetingCache.GetIndexForAdmin(db, MeetingType.BusinessInfo)
                                                                                     : MeetingCache.GetIndexForUser(db, MeetingType.BusinessInfo, userId, false));
            MeetingCache.MeetingCacheBusinessInfo meeting = meetings.OrderBy(m => m.Started).FirstOrDefault();
            if (meeting == null)
                return null;

            var meetingBusinessInfoIndex = new MeetingBusinessInfoIndex(meeting, isAdmin, userId);
            return meetingBusinessInfoIndex;
        }

        public static IEnumerable<MeetingBusinessInfoIndex> GetIndexRows(DefaultContext db, int userId, bool isAdmin, bool showAll, bool getArchive, string searchString, string searchStringAccording, string sortOrder)
        {
            IEnumerable<MeetingCache.MeetingCacheBusinessInfo> meetings;
            if (!getArchive)
            {
                meetings = isAdmin
                               ? (IEnumerable<MeetingCache.MeetingCacheBusinessInfo>)MeetingCache.GetIndexForAdmin(db, MeetingType.BusinessInfo)
                               : (IEnumerable<MeetingCache.MeetingCacheBusinessInfo>)MeetingCache.GetIndexForUser(db, MeetingType.BusinessInfo, userId, showAll);
            }
            else
            {
                meetings = (IEnumerable<MeetingCache.MeetingCacheBusinessInfo>)MeetingCache.GetArchiveIndex(db, MeetingType.BusinessInfo, isAdmin, userId);
            }

            MeetingCache.ProcessSearchingAndSorting(ref meetings, searchString, searchStringAccording, sortOrder);

            var indexRows = meetings.ToArray();
            MeetingBusinessInfoIndex[] meetingBusinessInfoIndex = indexRows.Select(m => new MeetingBusinessInfoIndex(m, isAdmin, userId)).ToArray();
            return meetingBusinessInfoIndex;
        }
    }

    public class MeetingWebinarIndex : MeetingIndex
    {
        public int FillCapacity { get; set; }

        private MeetingWebinarIndex(MeetingCache.MeetingCacheWebinar meetingCacheWebinar, bool isAdmin, int userId)
            : base(meetingCacheWebinar, isAdmin, userId)
        {
            FillCapacity = meetingCacheWebinar.FillCapacity;
        }

        public static MeetingWebinarIndex GetNearestMeeting(DefaultContext db, bool isAdmin, int userId)
        {
            var meetings = (IEnumerable<MeetingCache.MeetingCacheWebinar>)(isAdmin
                                                                                     ? MeetingCache.GetIndexForAdmin(db, MeetingType.Webinar)
                                                                                     : MeetingCache.GetIndexForUser(db, MeetingType.Webinar, userId, false));
            MeetingCache.MeetingCacheWebinar meeting = meetings.OrderBy(m => m.Started).FirstOrDefault();
            if (meeting == null)
                return null;

            var meetingWebinarIndex = new MeetingWebinarIndex(meeting, isAdmin, userId);
            return meetingWebinarIndex;
        }

        public static IEnumerable<MeetingWebinarIndex> GetIndexRows(DefaultContext db, int userId, bool isAdmin, bool showAll, bool getArchive, string searchString, string searchStringAccording, string sortOrder)
        {
            IEnumerable<MeetingCache.MeetingCacheWebinar> meetings;
            if (!getArchive)
            {
                meetings = isAdmin
                               ? (IEnumerable<MeetingCache.MeetingCacheWebinar>)MeetingCache.GetIndexForAdmin(db, MeetingType.Webinar)
                               : (IEnumerable<MeetingCache.MeetingCacheWebinar>)MeetingCache.GetIndexForUser(db, MeetingType.Webinar, userId, showAll);
            }
            else
            {
                meetings = (IEnumerable<MeetingCache.MeetingCacheWebinar>)MeetingCache.GetArchiveIndex(db, MeetingType.Webinar, isAdmin, userId);
            }

            MeetingCache.ProcessSearchingAndSorting(ref meetings, searchString, searchStringAccording, sortOrder);

            var indexRows = meetings.ToArray();
            MeetingWebinarIndex[] meetingWebinarIndex = indexRows.Select(m => new MeetingWebinarIndex(m, isAdmin, userId)).ToArray();
            return meetingWebinarIndex;
        }
    }

    public class MeetingMspEveningIndex : MeetingIndex
    {
        public string City { get; set; }

        public string Street { get; set; }

        private MeetingMspEveningIndex(MeetingCache.MeetingCacheMspEvening meetingCacheMspEvening, bool isAdmin, int userId)
            : base(meetingCacheMspEvening, isAdmin, userId)
        {
            City = meetingCacheMspEvening.City;
            Street = meetingCacheMspEvening.AddressLine1;
        }

        public static MeetingMspEveningIndex GetNearestMeeting(DefaultContext db, bool isAdmin, int userId)
        {
            var meetings = (IEnumerable<MeetingCache.MeetingCacheMspEvening>)(isAdmin
                                                                                ? MeetingCache.GetIndexForAdmin(db, MeetingType.MspEvening)
                                                                                : MeetingCache.GetIndexForUser(db, MeetingType.MspEvening, userId, false));

            var meeting = meetings.OrderBy(m => m.Started).FirstOrDefault();
            if (meeting == null)
                return null;

            var meetingMspEveningIndex = new MeetingMspEveningIndex(meeting, isAdmin, userId);
            return meetingMspEveningIndex;
        }

        public static IEnumerable<MeetingMspEveningIndex> GetIndexRows(DefaultContext db, int userId, bool isAdmin, bool showAll, bool getArchive, string searchString, string searchStringAccording, string sortOrder)
        {
            IEnumerable<MeetingCache.MeetingCacheMspEvening> meetings;
            if (!getArchive)
            {
                meetings = isAdmin
                               ? (IEnumerable<MeetingCache.MeetingCacheMspEvening>)MeetingCache.GetIndexForAdmin(db, MeetingType.MspEvening)
                               : (IEnumerable<MeetingCache.MeetingCacheMspEvening>)MeetingCache.GetIndexForUser(db, MeetingType.MspEvening, userId, showAll);
            }
            else
            {
                meetings = (IEnumerable<MeetingCache.MeetingCacheMspEvening>)MeetingCache.GetArchiveIndex(db, MeetingType.MspEvening, isAdmin, userId);
            }

            MeetingCache.ProcessSearchingAndSorting(ref meetings, searchString, searchStringAccording, sortOrder);

            var indexRows = meetings.ToArray();
            MeetingMspEveningIndex[] mspEveningInfoIndex = indexRows.Select(m => new MeetingMspEveningIndex(m, isAdmin, userId)).ToArray();
            return mspEveningInfoIndex;
        }
    }

    public class MeetingSetkaniTymuIndex : MeetingIndex
    {
        public string City { get; set; }

        public string Street { get; set; }

        public string MeetingTitleType { get; set; }

        public string Price { get; set; }

        private MeetingSetkaniTymuIndex(MeetingCache.MeetingCacheSetkaniTymu meetingCacheSetkaniTymu, bool isAdmin, int userId)
            : base(meetingCacheSetkaniTymu, isAdmin, userId)
        {
            City = meetingCacheSetkaniTymu.City;
            Street = meetingCacheSetkaniTymu.AddressLine1;
            MeetingTitleType = meetingCacheSetkaniTymu.MeetingTitleType;
            Price = meetingCacheSetkaniTymu.Price.HasValue && meetingCacheSetkaniTymu.CurrencyType.HasValue
                ? $"{meetingCacheSetkaniTymu.Price.GetValueOrDefault():N0} {meetingCacheSetkaniTymu.CurrencyType}"
                : NullDisplayText;
        }

        public static MeetingSetkaniTymuIndex GetNearestMeeting(DefaultContext db, bool isAdmin, int userId)
        {
            var meetings = (IEnumerable<MeetingCache.MeetingCacheSetkaniTymu>)(isAdmin
                                                                                    ? MeetingCache.GetIndexForAdmin(db, MeetingType.SetkaniTymu)
                                                                                    : MeetingCache.GetIndexForUser(db, MeetingType.SetkaniTymu, userId, false));

            var meeting = meetings.OrderBy(m => m.Started).FirstOrDefault();
            if (meeting == null)
                return null;

            var meetingSetkaniTymuIndex = new MeetingSetkaniTymuIndex(meeting, isAdmin, userId);
            return meetingSetkaniTymuIndex;
        }

        public static IEnumerable<MeetingSetkaniTymuIndex> GetIndexRows(DefaultContext db, int userId, bool isAdmin, bool showAll, bool getArchive, string searchString, string searchStringAccording, string sortOrder)
        {
            IEnumerable<MeetingCache.MeetingCacheSetkaniTymu> meetings;
            if (!getArchive)
            {
                meetings = isAdmin
                               ? (IEnumerable<MeetingCache.MeetingCacheSetkaniTymu>)MeetingCache.GetIndexForAdmin(db, MeetingType.SetkaniTymu)
                               : (IEnumerable<MeetingCache.MeetingCacheSetkaniTymu>)MeetingCache.GetIndexForUser(db, MeetingType.SetkaniTymu, userId, showAll);
            }
            else
            {
                meetings = (IEnumerable<MeetingCache.MeetingCacheSetkaniTymu>)MeetingCache.GetArchiveIndex(db, MeetingType.SetkaniTymu, isAdmin, userId);
            }

            MeetingCache.ProcessSearchingAndSorting(ref meetings, searchString, searchStringAccording, sortOrder);

            var indexRows = meetings.ToArray();
            MeetingSetkaniTymuIndex[] meetingSetkaniTymuIndices = indexRows.Select(m => new MeetingSetkaniTymuIndex(m, isAdmin, userId)).ToArray();
            return meetingSetkaniTymuIndices;
        }
    }

    public class MeetingSkoleniDavidaKotaskaIndex : MeetingIndex
    {
        public string City { get; set; }

        public string Street { get; set; }

        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        public string Price { get; set; }

        private MeetingSkoleniDavidaKotaskaIndex(MeetingCache.MeetingCacheSkoleniDavidaKotaska meetingCacheSkoleniDavidaKotaska, bool isAdmin, int userId)
            : base(meetingCacheSkoleniDavidaKotaska, isAdmin, userId)
        {
            City = meetingCacheSkoleniDavidaKotaska.City;
            Street = meetingCacheSkoleniDavidaKotaska.AddressLine1;
            Title = meetingCacheSkoleniDavidaKotaska.Title;
            Price = meetingCacheSkoleniDavidaKotaska.Price.HasValue && meetingCacheSkoleniDavidaKotaska.CurrencyType.HasValue
                ? $"{meetingCacheSkoleniDavidaKotaska.Price.GetValueOrDefault():N0} {meetingCacheSkoleniDavidaKotaska.CurrencyType}"
                : NullDisplayText;
        }

        public static MeetingSkoleniDavidaKotaskaIndex GetNearestMeeting(DefaultContext db, bool isAdmin, int userId)
        {
            var meetings = (IEnumerable<MeetingCache.MeetingCacheSkoleniDavidaKotaska>)(isAdmin
                                                                        ? MeetingCache.GetIndexForAdmin(db, MeetingType.SkoleniDavidaKotaska)
                                                                        : MeetingCache.GetIndexForUser(db, MeetingType.SkoleniDavidaKotaska, userId, false));

            var meeting = meetings.OrderBy(m => m.Started).FirstOrDefault();
            if (meeting == null)
                return null;

            var meetingSkoleniDavidaKotaskaIndex = new MeetingSkoleniDavidaKotaskaIndex(meeting, isAdmin, userId);
            return meetingSkoleniDavidaKotaskaIndex;
        }

        public static IEnumerable<MeetingSkoleniDavidaKotaskaIndex> GetIndexRows(DefaultContext db, int userId, bool isAdmin, bool showAll, bool getArchive, string searchString, string searchStringAccording, string sortOrder)
        {
            IEnumerable<MeetingCache.MeetingCacheSkoleniDavidaKotaska> meetings;
            if (!getArchive)
            {
                meetings = isAdmin
                               ? (IEnumerable<MeetingCache.MeetingCacheSkoleniDavidaKotaska>)MeetingCache.GetIndexForAdmin(db, MeetingType.SkoleniDavidaKotaska)
                               : (IEnumerable<MeetingCache.MeetingCacheSkoleniDavidaKotaska>)MeetingCache.GetIndexForUser(db, MeetingType.SkoleniDavidaKotaska, userId, showAll);
            }
            else
            {
                meetings = (IEnumerable<MeetingCache.MeetingCacheSkoleniDavidaKotaska>)MeetingCache.GetArchiveIndex(db, MeetingType.SkoleniDavidaKotaska, isAdmin, userId);
            }

            MeetingCache.ProcessSearchingAndSorting(ref meetings, searchString, searchStringAccording, sortOrder);

            var indexRows = meetings.ToArray();
            MeetingSkoleniDavidaKotaskaIndex[] meetingSkoleniDavidaKotaskaIndices = indexRows.Select(m => new MeetingSkoleniDavidaKotaskaIndex(m, isAdmin, userId)).ToArray();
            return meetingSkoleniDavidaKotaskaIndices;
        }
    }

    public class MeetingOstatniIndex : MeetingIndex
    {
        public string City { get; set; }

        public string Street { get; set; }

        public string MeetingTitleType { get; set; }

        public string Price { get; set; }

        private MeetingOstatniIndex(MeetingCache.MeetingCacheOstatni meetingCacheOstatni, bool isAdmin, int userId)
            : base(meetingCacheOstatni, isAdmin, userId)
        {
            City = meetingCacheOstatni.City;
            Street = meetingCacheOstatni.AddressLine1;
            MeetingTitleType = meetingCacheOstatni.MeetingTitleType;
            Price = meetingCacheOstatni.Price.HasValue && meetingCacheOstatni.CurrencyType.HasValue
                ? $"{meetingCacheOstatni.Price.GetValueOrDefault():N0} {meetingCacheOstatni.CurrencyType}"
                : NullDisplayText;
        }

        public static MeetingOstatniIndex GetNearestMeeting(DefaultContext db, bool isAdmin, int userId)
        {
            var meetings = (IEnumerable<MeetingCache.MeetingCacheOstatni>)(isAdmin
                                                            ? MeetingCache.GetIndexForAdmin(db, MeetingType.Ostatni)
                                                            : MeetingCache.GetIndexForUser(db, MeetingType.Ostatni, userId, false));

            var meeting = meetings.OrderBy(m => m.Started).FirstOrDefault();
            if (meeting == null)
                return null;

            var meetingOstatniIndex = new MeetingOstatniIndex(meeting, isAdmin, userId);
            return meetingOstatniIndex;
        }

        public static IEnumerable<MeetingOstatniIndex> GetIndexRows(DefaultContext db, int userId, bool isAdmin, bool showAll, bool getArchive, string searchString, string searchStringAccording, string sortOrder)
        {
            IEnumerable<MeetingCache.MeetingCacheOstatni> meetings;
            if (!getArchive)
            {
                meetings = isAdmin
                               ? (IEnumerable<MeetingCache.MeetingCacheOstatni>)MeetingCache.GetIndexForAdmin(db, MeetingType.Ostatni)
                               : (IEnumerable<MeetingCache.MeetingCacheOstatni>)MeetingCache.GetIndexForUser(db, MeetingType.Ostatni, userId, showAll);
            }
            else
            {
                meetings = (IEnumerable<MeetingCache.MeetingCacheOstatni>)MeetingCache.GetArchiveIndex(db, MeetingType.Ostatni, isAdmin, userId);
            }

            MeetingCache.ProcessSearchingAndSorting(ref meetings, searchString, searchStringAccording, sortOrder);

            var indexRows = meetings.ToArray();
            MeetingOstatniIndex[] meetingOstatniIndices = indexRows.Select(m => new MeetingOstatniIndex(m, isAdmin, userId)).ToArray();
            return meetingOstatniIndices;
        }
    }

    public interface IMeetingEdit
    {
        ModelStateDictionary Validate(DefaultContext db);
    }

    public abstract class MeetingEdit : BaseModelView
    {
        public int? MeetingId { get; set; }

        public int? OrganizerId { get; set; }

        [Display(Name = "Meeting_SecondaryOrganizer_Name", ResourceType = typeof(FieldResource))]
        public int? SecondaryOrganizerId { get; set; }

        [Display(Name = "Meeting_MeetingKind_Name", ResourceType = typeof(FieldResource))]
        public MeetingKind MeetingKind { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Meeting_StartTime_Name", ResourceType = typeof(FieldResource))]
        [RegularExpression(Regex.Time, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexTime_ErrorMessage")]
        public string StartTime { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Meeting_FinishTime_Name", ResourceType = typeof(FieldResource))]
        [RegularExpression(Regex.Time, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexTime_ErrorMessage")]
        public string FinishTime { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Range(0, Int32.MaxValue, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexOnlyNumbers_ErrorMessage")]
        [Display(Name = "Meeting_Capacity_Name", ResourceType = typeof(FieldResource))]
        public int Capacity { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Meeting_MainLeader_Name", ResourceType = typeof(FieldResource))]
        public int MainLeaderId { get; set; }

        [Display(Name = "Meeting_SecondaryLeader_Name", ResourceType = typeof(FieldResource))]
        public int? SecondaryLeaderId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [StringLength(20, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Meeting_Lecturer_Name", ResourceType = typeof(FieldResource))]
        public string Lecturer { get; set; }

        [AllowHtml]
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        public string Note { get; set; }

        [Display(Name = "Meeting_IsMeetingPrivate_Name", ResourceType = typeof(FieldResource))]
        public bool Private { get; set; }

        protected MeetingEdit()
        {
            Private = true;
        }

        protected MeetingEdit(Meeting meeting)
            : this()
        {
            if (meeting == null)
                return;

            MeetingId = meeting.MeetingId;
            OrganizerId = meeting.OrganizerId;
            SecondaryOrganizerId = meeting.SecondaryOrganizerId;
            StartTime = meeting.Started.ToString("HH:mm");
            FinishTime = meeting.Finished.ToString("HH:mm");
            Capacity = meeting.Capacity;
            MainLeaderId = meeting.MainLeaderId;
            SecondaryLeaderId = meeting.SecondaryLeaderId;
            Lecturer = meeting.Lecturer;
            Note = meeting.Note;
            Private = meeting.Private;
        }

        protected Meeting GetCommonModel(int userId)
        {
            var meeting = new Meeting
            {
                OrganizerId = OrganizerId.HasValue ? OrganizerId.GetValueOrDefault() : userId,
                SecondaryOrganizerId = SecondaryOrganizerId,
                Capacity = Capacity,
                MainLeaderId = MainLeaderId,
                SecondaryLeaderId = SecondaryLeaderId,
                Lecturer = Lecturer,
                Note = Note,
                Private = Private
            };
            if (MeetingId.HasValue)
            {
                meeting.MeetingId = MeetingId.GetValueOrDefault();
            }

            return meeting;
        }

        protected void CommonValidates(DefaultContext db, ref ModelStateDictionary modelStateDictionary)
        {
            ValidateCapacity(db, MeetingId, Capacity, ref modelStateDictionary);
            ValidateSecondaryLeader(db, MainLeaderId, ref modelStateDictionary);
            ValidateSecondaryOrganizer(db, MainLeaderId, SecondaryLeaderId, ref modelStateDictionary);
        }

        protected void ValidateStartAndFinishTime(DateTime dateStartTime, DateTime? dateFinishTime, DateTime? registerDeadline, string startTime, string finishTime, ref ModelStateDictionary modelStateDictionary)
        {
            if (IsNullOrEmpty(startTime))
            {
                modelStateDictionary.AddModelError(BaseCache.StartTimeField, Format(ValidationResource.Global_Required_ErrorMessage, FieldResource.Meeting_StartTime_Name));
            }

            if (IsNullOrEmpty(finishTime))
            {
                modelStateDictionary.AddModelError(BaseCache.FinishTimeField, Format(ValidationResource.Global_Required_ErrorMessage, FieldResource.Meeting_FinishTime_Name));
            }

            if (!IsNullOrEmpty(startTime) && !System.Text.RegularExpressions.Regex.IsMatch(startTime, Regex.Time))
            {
                modelStateDictionary.AddModelError(BaseCache.StartTimeField, Format(ValidationResource.Global_RegexTime_ErrorMessage, FieldResource.Meeting_StartTime_Name));
            }

            if (!IsNullOrEmpty(finishTime) && !System.Text.RegularExpressions.Regex.IsMatch(finishTime, Regex.Time))
            {
                modelStateDictionary.AddModelError(BaseCache.FinishTimeField, Format(ValidationResource.Global_RegexTime_ErrorMessage, FieldResource.Meeting_FinishTime_Name));
            }

            if (modelStateDictionary.Count > 0)
                return;

            DateTime started = DateTime.Parse($"{dateStartTime.ToString("yyyy-MM-dd")}T{startTime}");
            DateTime finished = DateTime.Parse($"{(dateFinishTime ?? dateStartTime).ToString("yyyy-MM-dd")}T{finishTime}");
            if (started > finished)
            {
                modelStateDictionary.AddModelError(BaseCache.FinishTimeField, ValidationResource.Meeting_StartedGreaterThanFinished_ErrorMessage);
            }

            if (finished < DateTime.Now)
            {
                modelStateDictionary.AddModelError(dateFinishTime.HasValue ? BaseCache.DateFinishTimeField : BaseCache.DateField, ValidationResource.Meeting_FinishedLowerThanNow_ErrorMessage);
            }

            if (registerDeadline.HasValue && registerDeadline.Value > started)
            {
                modelStateDictionary.AddModelError(BaseCache.RegisterDeadlineField, ValidationResource.Meeting_RegisterDeadlineGreaterThanStarted_ErrorMessage);
            }
        }

        protected void ValidateCapacity(DefaultContext db, int? meetingId, int capacity, ref ModelStateDictionary modelStateDictionary)
        {
            if (!meetingId.HasValue)
                return;

            Meeting meeting = MeetingCache.GetDetail(db, meetingId.GetValueOrDefault());
            if (meeting.MeetingAttendees.Count > capacity)
            {
                modelStateDictionary.AddModelError(BaseCache.CapacityField, ValidationResource.Meeting_CapacityLowerThanRegistered_ErrorMessage);
            }
        }

        protected void ValidateSecondaryLeader(DefaultContext db, int mainLeaderId, ref ModelStateDictionary modelStateDictionary)
        {
            if (!SecondaryLeaderId.HasValue)
                return;

            int[] downlineAndUplineUsersIds = UserProfileCache.GetDownlineAndUplineUsersIds(db, mainLeaderId);
            bool isSecondaryLeaderValid = !downlineAndUplineUsersIds.Contains(SecondaryLeaderId.GetValueOrDefault());
            if (isSecondaryLeaderValid)
                return;

            modelStateDictionary.AddModelError(BaseCache.SecondaryLeaderIdField, ValidationResource.Meeting_BadSecondaryLeader_ErrorMessage);
        }

        protected void ValidateSecondaryOrganizer(DefaultContext db, int mainLeaderId, int? secondaryLeaderId, ref ModelStateDictionary modelStateDictionary)
        {
            if (!SecondaryOrganizerId.HasValue)
                return;

            int[] downlineUserIds = UserProfileCache.GetDownlineUserIds(db, mainLeaderId, secondaryLeaderId);
            bool isSecondaryOrganizerValid = downlineUserIds.Contains(SecondaryOrganizerId.GetValueOrDefault());
            if (isSecondaryOrganizerValid)
                return;

            modelStateDictionary.AddModelError(BaseCache.SecondaryOrganizerIdField, ValidationResource.Meeting_BadSecondaryOrganizer_ErrorMessage);
        }

        protected void ValidatePrice(DefaultContext db, int bankAccountId, int price, int? secondBankAccountId, ref int? secondPrice, ref ModelStateDictionary modelStateDictionary)
        {
            if (secondPrice.GetValueOrDefault() == 0)
            {
                secondPrice = null;
            }

            if ((secondBankAccountId.GetValueOrDefault() != 0 && secondPrice.GetValueOrDefault() == 0) || (secondBankAccountId.GetValueOrDefault() == 0 && secondPrice.GetValueOrDefault() != 0))
            {
                modelStateDictionary.AddModelError(BaseCache.EmptyField, ValidationResource.Meeting_SecondBankAccount_ErrorMessage);
            }

            if (!MeetingId.HasValue)
                return;

            Meeting meeting = MeetingCache.GetDetail(db, MeetingId.GetValueOrDefault());
            if (meeting == null)
                return;

            if ((meeting.Price != price || meeting.BankAccountId != bankAccountId || meeting.SecondPrice != secondPrice || meeting.SecondBankAccountId != secondBankAccountId) && meeting.MeetingAttendees.Any())
            {
                modelStateDictionary.AddModelError(BaseCache.EmptyField, ValidationResource.Meeting_CannotChangePrice_ErrorMessage);
            }
        }
    }

    public class MeetingBusinessInfoEdit : MeetingEdit, IMeetingEdit
    {
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_City_Name", ResourceType = typeof(FieldResource))]
        public string City { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Meeting_AddressLine1_Name", ResourceType = typeof(FieldResource))]
        public string AddressLine1 { get; set; }

        [Display(Name = "Meeting_MeetingType_Name", ResourceType = typeof(FieldResource))]
        public MeetingType MeetingType { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Date_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime Date { get; set; }

        public MeetingBusinessInfoEdit()
        {

        }

        private MeetingBusinessInfoEdit(Meeting meeting)
            : base(meeting)
        {
            City = meeting.City;
            AddressLine1 = meeting.AddressLine1;
            MeetingType = MeetingType.BusinessInfo;
            MeetingKind = MeetingKind.Public;
            Date = meeting.Started.Date;
        }

        public static MeetingBusinessInfoEdit GetModelView(Meeting meeting)
        {
            if (meeting == null || meeting.MeetingType != MeetingType.BusinessInfo)
                return null;

            var meetingBusinessInfo = new MeetingBusinessInfoEdit(meeting);
            return meetingBusinessInfo;
        }

        public Meeting GetModel(int userId)
        {
            DateTime started = DateTime.Parse($"{Date.ToString("yyyy-MM-dd")}T{StartTime}");
            DateTime finished = DateTime.Parse($"{Date.ToString("yyyy-MM-dd")}T{FinishTime}");

            Meeting meeting = GetCommonModel(userId);
            meeting.Title = ListItemsResource.MeetingType_Bi;
            meeting.MeetingKind = MeetingKind.Public;
            meeting.MeetingType = MeetingType.BusinessInfo;
            meeting.Started = started;
            meeting.Finished = finished;
            meeting.City = City;
            meeting.AddressLine1 = AddressLine1;

            return meeting;
        }

        public ModelStateDictionary Validate(DefaultContext db)
        {
            var modelStateDictionary = new ModelStateDictionary();
            CommonValidates(db, ref modelStateDictionary);
            ValidateStartAndFinishTime(Date, null, null, StartTime, FinishTime, ref modelStateDictionary);

            return modelStateDictionary;
        }
    }

    public class MeetingWebinarEdit : MeetingEdit, IMeetingEdit
    {
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Meeting_WebinarUrl_Name", ResourceType = typeof(FieldResource))]
        public string WebinarUrl { get; set; }

        [Display(Name = "Meeting_MeetingType_Name", ResourceType = typeof(FieldResource))]
        public MeetingType MeetingType { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Date_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime Date { get; set; }

        public MeetingWebinarEdit()
        {

        }

        private MeetingWebinarEdit(Meeting meeting)
            : base(meeting)
        {
            MeetingType = MeetingType.Webinar;
            MeetingKind = MeetingKind.Public;
            Date = meeting.Started.Date;
            WebinarUrl = meeting.WebinarUrl;
        }

        public static MeetingWebinarEdit GetModelView(Meeting meeting)
        {
            if (meeting == null || meeting.MeetingType != MeetingType.Webinar)
                return null;

            var meetingWebinar = new MeetingWebinarEdit(meeting);
            return meetingWebinar;
        }

        public Meeting GetModel(int userId)
        {
            DateTime started = DateTime.Parse($"{Date.ToString("yyyy-MM-dd")}T{StartTime}");
            DateTime finished = DateTime.Parse($"{Date.ToString("yyyy-MM-dd")}T{FinishTime}");

            Meeting meeting = GetCommonModel(userId);
            meeting.Title = ListItemsResource.MeetingType_Webinar;
            meeting.MeetingKind = MeetingKind.Public;
            meeting.MeetingType = MeetingType.Webinar;
            meeting.Started = started;
            meeting.Finished = finished;
            meeting.WebinarUrl = WebinarUrl;
            meeting.Capacity = Int32.MaxValue;

            return meeting;
        }

        public ModelStateDictionary Validate(DefaultContext db)
        {
            Capacity = Int32.MaxValue;
            var modelStateDictionary = new ModelStateDictionary();
            CommonValidates(db, ref modelStateDictionary);
            ValidateStartAndFinishTime(Date, null, null, StartTime, FinishTime, ref modelStateDictionary);

            return modelStateDictionary;
        }
    }

    public class MeetingMspEveningEdit : MeetingEdit, IMeetingEdit
    {
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_City_Name", ResourceType = typeof(FieldResource))]
        public string City { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Meeting_AddressLine1_Name", ResourceType = typeof(FieldResource))]
        public string AddressLine1 { get; set; }

        [Display(Name = "Meeting_MeetingType_Name", ResourceType = typeof(FieldResource))]
        public MeetingType MeetingType { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Date_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime Date { get; set; }

        public MeetingMspEveningEdit()
        {

        }

        private MeetingMspEveningEdit(Meeting meeting)
            : base(meeting)
        {
            City = meeting.City;
            AddressLine1 = meeting.AddressLine1;
            MeetingType = MeetingType.MspEvening;
            MeetingKind = MeetingKind.MSP;
            Date = meeting.Started.Date;
        }

        public static MeetingMspEveningEdit GetModelView(Meeting meeting)
        {
            if (meeting == null || meeting.MeetingType != MeetingType.MspEvening)
                return null;

            var meetingMspEvening = new MeetingMspEveningEdit(meeting);
            return meetingMspEvening;
        }

        public Meeting GetModel(int userId)
        {
            DateTime started = DateTime.Parse($"{Date.ToString("yyyy-MM-dd")}T{StartTime}");
            DateTime finished = DateTime.Parse($"{Date.ToString("yyyy-MM-dd")}T{FinishTime}");

            Meeting meeting = GetCommonModel(userId);
            meeting.Title = ListItemsResource.MeetingType_MspEvening;
            meeting.MeetingKind = MeetingKind.MSP;
            meeting.MeetingType = MeetingType.MspEvening;
            meeting.Started = started;
            meeting.Finished = finished;
            meeting.City = City;
            meeting.AddressLine1 = AddressLine1;

            return meeting;
        }

        public ModelStateDictionary Validate(DefaultContext db)
        {
            var modelStateDictionary = new ModelStateDictionary();
            CommonValidates(db, ref modelStateDictionary);
            ValidateStartAndFinishTime(Date, null, null, StartTime, FinishTime, ref modelStateDictionary);

            return modelStateDictionary;
        }
    }

    public class MeetingSetkaniTymuEdit : MeetingEdit, IMeetingEdit
    {
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_City_Name", ResourceType = typeof(FieldResource))]
        public string City { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Meeting_AddressLine1_Name", ResourceType = typeof(FieldResource))]
        public string AddressLine1 { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Meeting_MeetingType_Name", ResourceType = typeof(FieldResource))]
        public int MeetingTitleTypeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Meeting_DateStartTime_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime DateStartTime { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Meeting_DateFinishTime_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime DateFinishTime { get; set; }

        [Display(Name = "Meeting_InvitationCardUrl_Name", ResourceType = typeof(FieldResource))]
        public string InvitationCardUrl { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Range(0, Int32.MaxValue, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexOnlyNumbers_ErrorMessage")]
        [Display(Name = "Meeting_Price_Name", ResourceType = typeof(FieldResource))]
        public int Price { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexOnlyNumbers_ErrorMessage")]
        [Display(Name = "Meeting_Price_Name", ResourceType = typeof(FieldResource))]
        public int? SecondPrice { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_BankAccount_Name", ResourceType = typeof(FieldResource))]
        public int BankAccountId { get; set; }

        [Display(Name = "Global_SecondBankAccount_Name", ResourceType = typeof(FieldResource))]
        public int? SecondBankAccountId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Meeting_RegisterDeadline_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime RegisterDeadline { get; set; }

        [AllowHtml]
        [Display(Name = "Meeting_BringYourOwn_Name", ResourceType = typeof(FieldResource))]
        public string BringYourOwn { get; set; }

        public MeetingSetkaniTymuEdit()
        {

        }

        private MeetingSetkaniTymuEdit(Meeting meeting)
            : base(meeting)
        {
            City = meeting.City;
            AddressLine1 = meeting.AddressLine1;
            MeetingTitleTypeId = meeting.MeetingTitleTypeId.GetValueOrDefault();
            MeetingKind = MeetingKind.Public;
            Title = meeting.Title;
            DateStartTime = meeting.Started.Date;
            DateFinishTime = meeting.Finished.Date;
            InvitationCardUrl = meeting.InvitationCardUrl;
            Price = meeting.Price.GetValueOrDefault();
            SecondPrice = meeting.SecondPrice;
            BankAccountId = meeting.BankAccountId.GetValueOrDefault();
            SecondBankAccountId = meeting.SecondBankAccountId;
            RegisterDeadline = meeting.RegisterDeadline.GetValueOrDefault();
            BringYourOwn = meeting.BringYourOwn;
        }

        public static MeetingSetkaniTymuEdit GetModelView(Meeting meeting)
        {
            if (meeting == null || meeting.MeetingType != MeetingType.SetkaniTymu)
                return null;

            var meetingViewSetkaniTymu = new MeetingSetkaniTymuEdit(meeting);
            return meetingViewSetkaniTymu;
        }

        public Meeting GetModel(int userId)
        {
            DateTime started = DateTime.Parse($"{DateStartTime.ToString("yyyy-MM-dd")}T{StartTime}");
            DateTime finished = DateTime.Parse($"{DateFinishTime.ToString("yyyy-MM-dd")}T{FinishTime}");

            Meeting meeting = GetCommonModel(userId);
            meeting.MeetingTitleTypeId = MeetingTitleTypeId;
            meeting.Title = Title;
            meeting.MeetingType = MeetingType.SetkaniTymu;
            meeting.MeetingKind = MeetingKind.Public;
            meeting.Started = started;
            meeting.Finished = finished;
            meeting.InvitationCardUrl = InvitationCardUrl;
            meeting.Chargeable = true;
            meeting.Price = Price;
            meeting.SecondPrice = SecondPrice;
            meeting.BankAccountId = BankAccountId;
            meeting.SecondBankAccountId = SecondBankAccountId;
            meeting.RegisterDeadline = RegisterDeadline;
            meeting.BringYourOwn = BringYourOwn;
            meeting.City = City;
            meeting.AddressLine1 = AddressLine1;

            return meeting;
        }

        public ModelStateDictionary Validate(DefaultContext db)
        {
            var modelStateDictionary = new ModelStateDictionary();
            CommonValidates(db, ref modelStateDictionary);
            ValidateStartAndFinishTime(DateStartTime, DateFinishTime, RegisterDeadline, StartTime, FinishTime, ref modelStateDictionary);

            int? secondPrice = SecondPrice;
            ValidatePrice(db, BankAccountId, Price, SecondBankAccountId, ref secondPrice, ref modelStateDictionary);
            SecondPrice = secondPrice;

            return modelStateDictionary;
        }
    }

    public class MeetingSkoleniDavidaKotaskaEdit : MeetingEdit, IMeetingEdit
    {
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_City_Name", ResourceType = typeof(FieldResource))]
        public string City { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Meeting_AddressLine1_Name", ResourceType = typeof(FieldResource))]
        public string AddressLine1 { get; set; }

        [Display(Name = "Meeting_MeetingType_Name", ResourceType = typeof(FieldResource))]
        public MeetingType MeetingType { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Date_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime Date { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Range(0, Int32.MaxValue, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexOnlyNumbers_ErrorMessage")]
        [Display(Name = "Meeting_Price_Name", ResourceType = typeof(FieldResource))]
        public int Price { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_BankAccount_Name", ResourceType = typeof(FieldResource))]
        public int BankAccountId { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexOnlyNumbers_ErrorMessage")]
        [Display(Name = "Meeting_Price_Name", ResourceType = typeof(FieldResource))]
        public int? SecondPrice { get; set; }

        [Display(Name = "Global_SecondBankAccount_Name", ResourceType = typeof(FieldResource))]
        public int? SecondBankAccountId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Meeting_RegisterDeadline_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime RegisterDeadline { get; set; }

        [AllowHtml]
        [Display(Name = "Meeting_BringYourOwn_Name", ResourceType = typeof(FieldResource))]
        public string BringYourOwn { get; set; }

        public MeetingSkoleniDavidaKotaskaEdit()
        {

        }

        private MeetingSkoleniDavidaKotaskaEdit(Meeting meeting)
            : base(meeting)
        {
            City = meeting.City;
            AddressLine1 = meeting.AddressLine1;
            MeetingType = MeetingType.SkoleniDavidaKotaska;
            MeetingKind = MeetingKind.Public;
            Title = meeting.Title;
            Date = meeting.Started.Date;
            Price = meeting.Price.GetValueOrDefault();
            SecondPrice = meeting.SecondPrice;
            BankAccountId = meeting.BankAccountId.GetValueOrDefault();
            SecondBankAccountId = meeting.SecondBankAccountId;
            RegisterDeadline = meeting.RegisterDeadline.GetValueOrDefault();
            BringYourOwn = meeting.BringYourOwn;
        }

        public static MeetingSkoleniDavidaKotaskaEdit GetModelView(Meeting meeting)
        {
            if (meeting == null || meeting.MeetingType != MeetingType.SkoleniDavidaKotaska)
                return null;

            var meetingViewSkoleniDavidaKotaska = new MeetingSkoleniDavidaKotaskaEdit(meeting);
            return meetingViewSkoleniDavidaKotaska;
        }

        public Meeting GetModel(int userId)
        {
            DateTime started = DateTime.Parse($"{Date.ToString("yyyy-MM-dd")}T{StartTime}");
            DateTime finished = DateTime.Parse($"{Date.ToString("yyyy-MM-dd")}T{FinishTime}");

            Meeting meeting = GetCommonModel(userId);
            meeting.Title = Title;
            meeting.MeetingType = MeetingType.SkoleniDavidaKotaska;
            meeting.MeetingKind = MeetingKind.Public;
            meeting.Started = started;
            meeting.Finished = finished;
            meeting.Chargeable = true;
            meeting.Price = Price;
            meeting.SecondPrice = SecondPrice;
            meeting.BankAccountId = BankAccountId;
            meeting.SecondBankAccountId = SecondBankAccountId;
            meeting.RegisterDeadline = RegisterDeadline;
            meeting.BringYourOwn = BringYourOwn;
            meeting.City = City;
            meeting.AddressLine1 = AddressLine1;

            return meeting;
        }

        public ModelStateDictionary Validate(DefaultContext db)
        {
            var modelStateDictionary = new ModelStateDictionary();
            CommonValidates(db, ref modelStateDictionary);
            ValidateStartAndFinishTime(Date, null, RegisterDeadline, StartTime, FinishTime, ref modelStateDictionary);

            int? secondPrice = SecondPrice;
            ValidatePrice(db, BankAccountId, Price, SecondBankAccountId, ref secondPrice, ref modelStateDictionary);
            SecondPrice = secondPrice;

            return modelStateDictionary;
        }
    }

    public class MeetingOstatniEdit : MeetingEdit, IMeetingEdit
    {
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_City_Name", ResourceType = typeof(FieldResource))]
        public string City { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Meeting_AddressLine1_Name", ResourceType = typeof(FieldResource))]
        public string AddressLine1 { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Meeting_MeetingType_Name", ResourceType = typeof(FieldResource))]
        public int MeetingTitleTypeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Date_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime Date { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexOnlyNumbers_ErrorMessage")]
        [Display(Name = "Meeting_Price_Name", ResourceType = typeof(FieldResource))]
        public int? Price { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexOnlyNumbers_ErrorMessage")]
        [Display(Name = "Meeting_Price_Name", ResourceType = typeof(FieldResource))]
        public int? SecondPrice { get; set; }

        [Display(Name = "Global_BankAccount_Name", ResourceType = typeof(FieldResource))]
        public int? BankAccountId { get; set; }

        [Display(Name = "Global_SecondBankAccount_Name", ResourceType = typeof(FieldResource))]
        public int? SecondBankAccountId { get; set; }

        [Display(Name = "Meeting_RegisterDeadline_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? RegisterDeadline { get; set; }

        public MeetingOstatniEdit()
        {

        }

        private MeetingOstatniEdit(Meeting meeting)
            : base(meeting)
        {
            City = meeting.City;
            AddressLine1 = meeting.AddressLine1;
            MeetingTitleTypeId = meeting.MeetingTitleTypeId.GetValueOrDefault();
            MeetingKind = MeetingKind.Public;
            Date = meeting.Started.Date;
            Price = meeting.Price;
            SecondPrice = meeting.SecondPrice;
            BankAccountId = meeting.BankAccountId;
            SecondBankAccountId = meeting.SecondBankAccountId;
            RegisterDeadline = meeting.RegisterDeadline;
        }

        public static MeetingOstatniEdit GetModelView(Meeting meeting)
        {
            if (meeting == null || meeting.MeetingType != MeetingType.Ostatni)
                return null;

            var meetingViewOstatni = new MeetingOstatniEdit(meeting);
            return meetingViewOstatni;
        }

        public Meeting GetModel(DefaultContext db, int userId)
        {
            DateTime started = DateTime.Parse($"{Date.ToString("yyyy-MM-dd")}T{StartTime}");
            DateTime finished = DateTime.Parse($"{Date.ToString("yyyy-MM-dd")}T{FinishTime}");
            MeetingTitleType meetingTitleType = db.MeetingTitleTypes.Find(MeetingTitleTypeId);

            Meeting meeting = GetCommonModel(userId);
            meeting.MeetingTitleTypeId = MeetingTitleTypeId;
            meeting.Title = meetingTitleType != null ? meetingTitleType.Title : ListItemsResource.MeetingType_Ostatni;
            meeting.MeetingKind = MeetingKind.Public;
            meeting.MeetingType = MeetingType.Ostatni;
            meeting.Started = started;
            meeting.Finished = finished;
            meeting.Chargeable = Price.GetValueOrDefault(0) != 0;
            meeting.Price = Price;
            meeting.SecondPrice = SecondPrice;
            meeting.BankAccountId = BankAccountId;
            meeting.SecondBankAccountId = SecondBankAccountId;
            meeting.RegisterDeadline = RegisterDeadline;
            meeting.City = City;
            meeting.AddressLine1 = AddressLine1;

            return meeting;
        }

        public ModelStateDictionary Validate(DefaultContext db)
        {
            var modelStateDictionary = new ModelStateDictionary();
            CommonValidates(db, ref modelStateDictionary);
            ValidateStartAndFinishTime(Date, null, RegisterDeadline, StartTime, FinishTime, ref modelStateDictionary);
            ValidatePrice(db, ref modelStateDictionary);

            return modelStateDictionary;
        }

        private void ValidatePrice(DefaultContext db, ref ModelStateDictionary modelStateDictionary)
        {
            if (Price.GetValueOrDefault() == 0)
            {
                Price = null;
            }

            if (SecondPrice.GetValueOrDefault() == 0)
            {
                SecondPrice = null;
            }

            bool isError = Price.HasValue && (BankAccountId.GetValueOrDefault() == 0 || !RegisterDeadline.HasValue);
            isError |= BankAccountId.GetValueOrDefault() != 0 && (!Price.HasValue || !RegisterDeadline.HasValue);
            if (isError)
            {
                modelStateDictionary.AddModelError(BaseCache.EmptyField, ValidationResource.Meeting_PriceBankAccountRegisterDeadline_ErrorMessage);
            }

            isError = SecondPrice.HasValue && (!Price.HasValue || BankAccountId.GetValueOrDefault() == 0 || !RegisterDeadline.HasValue);
            isError |= SecondBankAccountId.GetValueOrDefault() != 0 && (!Price.HasValue || BankAccountId.GetValueOrDefault() == 0 || !RegisterDeadline.HasValue);
            if (isError)
            {
                modelStateDictionary.AddModelError(BaseCache.EmptyField, ValidationResource.Meeting_SecondBankAccountWithoutFirstBankAccount_ErrorMessage);
            }


            isError = SecondPrice.HasValue && SecondBankAccountId.GetValueOrDefault() == 0;
            isError |= SecondBankAccountId.GetValueOrDefault() != 0 && !SecondPrice.HasValue;
            if (isError)
            {
                modelStateDictionary.AddModelError(BaseCache.EmptyField, ValidationResource.Meeting_SecondBankAccount_ErrorMessage);
            }

            if (!MeetingId.HasValue)
                return;

            Meeting meeting = MeetingCache.GetDetail(db, MeetingId.GetValueOrDefault());
            if (meeting == null)
                return;

            if ((meeting.Price != Price || meeting.BankAccountId != BankAccountId || meeting.SecondPrice != SecondPrice || meeting.SecondBankAccountId != SecondBankAccountId) && meeting.MeetingAttendees.Any())
            {
                modelStateDictionary.AddModelError(BaseCache.EmptyField, ValidationResource.Meeting_CannotChangePrice_ErrorMessage);
            }
        }
    }

    public class MeetingDetailAttendee : BaseModelView
    {
        public int MeetingAttendeeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string LastNameForDownline { get; set; }

        public string LyonessId { get; set; }

        public string PaidAmmount { get; set; }

        public string FormattingClass { get; set; }

        public string City { get; set; }

        public string Leader { get; set; }

        public bool AccessUnsign { get; set; }

        public bool AccessLocking { get; set; }

        public bool IsReservationLocked { get; set; }
    }

    public abstract class MeetingDetail : BaseModelView
    {
        #region Properties

        public int MeetingId { get; set; }

        [Display(Name = "Meeting_MeetingKind_Name", ResourceType = typeof(FieldResource))]
        public MeetingKind MeetingKind { get; set; }

        [Display(Name = "Meeting_Organizer_Name", ResourceType = typeof(FieldResource))]
        public string Organizer { get; set; }

        [Display(Name = "Meeting_FreeCapacity_Name", ResourceType = typeof(FieldResource))]
        public int FreeCapacity { get; set; }

        [Display(Name = "Meeting_Capacity_Name", ResourceType = typeof(FieldResource))]
        public int Capacity { get; set; }

        [Display(Name = "Meeting_MainLeader_Name", ResourceType = typeof(FieldResource))]
        public string MainLeader { get; set; }

        [Display(Name = "Meeting_SecondaryLeader_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string SecondaryLeader { get; set; }

        [Display(Name = "BankAccount_AccountId_Name", ResourceType = typeof(FieldResource))]
        public string AccountId { get; set; }

        [Display(Name = "BankAccount_SecondAccountId_Name", ResourceType = typeof(FieldResource))]
        public string SecondAccountId { get; set; }

        [Display(Name = "Meeting_Lecturer_Name", ResourceType = typeof(FieldResource))]
        public string Lecturer { get; set; }

        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        public string Note { get; set; }

        public MeetingDetailAttendee[] MeetingAttendees { get; set; }

        public IPagedList<MeetingSignUnsignAttendee> SignedAttendees { get; set; }

        public IPagedList<MeetingSignUnsignAttendee> Attendees { get; set; }

        public MeetingSignUnsignAttendee UserAttendee { get; set; }

        public bool AccessSignUnsign { get; set; }

        public bool AccessEdit { get; set; }

        #endregion

        #region Fields

        public DateTime Finished;
        public int MainLeaderId;
        public int? SecondaryLeaderId;

        public int OrganizerId;
        public bool IsArchive;

        private int[] _downlineUserIds;
        private bool _isDownlineUserIdsSet;
        private bool _availableForProcess;

        #endregion

        #region Constructors & Destructors

        protected MeetingDetail(Meeting meeting, DefaultContext db, int userId, bool isAdmin)
        {
            if (meeting == null)
                return;

            MeetingId = meeting.MeetingId;
            Finished = meeting.Finished;

            Organizer = meeting.Organizer.FullNameWithoutLyonessId;
            if (meeting.SecondaryOrganizerId.HasValue)
            {
                Organizer = $"{Organizer}, {meeting.SecondaryOrganizer.FullNameWithoutLyonessId}";
            }

            FreeCapacity = meeting.Capacity - meeting.MeetingAttendees.Count;
            Capacity = meeting.Capacity;
            MainLeader = meeting.MainLeader.FullNameWithoutLyonessId;
            MainLeaderId = meeting.MainLeaderId;
            SecondaryLeader = meeting.SecondaryLeaderId.HasValue
                                  ? meeting.SecondaryLeader.FullNameWithoutLyonessId
                                  : Empty;
            SecondaryLeaderId = meeting.SecondaryLeaderId;
            OrganizerId = meeting.OrganizerId;
            Lecturer = meeting.Lecturer;
            Note = meeting.Note;

            SignedAttendees = new PagedList<MeetingSignUnsignAttendee>(new MeetingSignUnsignAttendee[0], 1, 1);

            IsArchive = meeting.Finished <= DateTime.Now;

            int[] downlineUsersIdsWithoutAdmins = UserProfileCache.GetDownlineUserIdsWithoutAdmins(db, meeting.MainLeaderId, SecondaryLeaderId);
            AccessSignUnsign = isAdmin || downlineUsersIdsWithoutAdmins.Contains(userId);

            AccessEdit = isAdmin || meeting.OrganizerId == userId || meeting.SecondaryOrganizerId == userId;

            _availableForProcess = true;
        }

        #endregion

        private static string GetAccountIdText(BankAccount bankAccount)
        {
            if (bankAccount == null)
                return NullDisplayText;

            var accountId = new StringBuilder();
            accountId.Append(bankAccount.AccountId);

            if (bankAccount.CurrencyType == CurrencyType.CZK)
            {
                return accountId.ToString();
            }

            accountId.Append($"; IBAN/SWIFT: {bankAccount.IBAN}/{bankAccount.SWIFT}");
            return accountId.ToString();
        }

        protected void SetAccountIdText(BankAccount bankAccount, BankAccount secondBankAccount)
        {
            AccountId = GetAccountIdText(bankAccount);
            SecondAccountId = GetAccountIdText(secondBankAccount);
        }

        protected bool ShowContactAcronym(DefaultContext db, int userId, int registrarId)
        {
            if (!_isDownlineUserIdsSet)
            {
                _downlineUserIds = UserProfileCache.GetDownlineUserIds(db, userId);
                _isDownlineUserIdsSet = true;
            }

            bool showContactAcronym = _downlineUserIds == null || !_downlineUserIds.Contains(registrarId);
            return showContactAcronym;
        }

        protected void ProcessSignUsign(Meeting meeting, IQueryable<PeopleContact> peopleContacts, DefaultContext db, int id, bool isAdmin, int registrarId, string searchString, string searchStringAccording, string sortOrder, string signedSortOrder, int pageNumber, int pageSize, int signedPageNumber)
        {
            if (!_availableForProcess)
                return;

            _availableForProcess = false;

            if (meeting.RegisterDeadline.GetValueOrDefault(DateTime.MaxValue) < DateTime.Now)
                return;

            var attendees = new List<MeetingSignUnsignAttendee>();
            var signedAttendees = new List<MeetingSignUnsignAttendee>();
            foreach (PeopleContact peopleContact in peopleContacts)
            {
                bool signed = false;
                if (meeting.MeetingAttendees != null && meeting.MeetingAttendees.Any())
                {
                    signed = meeting.MeetingAttendees.Any(ma => ma.AttendeeId == peopleContact.PeopleContactId);
                }

                if (!signed)
                {
                    var attendee = new MeetingSignUnsignAttendee
                    {
                        AttendeeId = peopleContact.PeopleContactId,
                        Signed = false,
                        FirstName = peopleContact.FirstName,
                        LastName = peopleContact.LastName,
                        City = peopleContact.City
                    };
                    attendees.Add(attendee);
                }
                else
                {
                    var signedAttendee = new MeetingSignUnsignAttendee
                    {
                        AttendeeId = peopleContact.PeopleContactId,
                        Signed = true,
                        FirstName = peopleContact.FirstName,
                        LastName = peopleContact.LastName,
                        City = peopleContact.City
                    };
                    signedAttendees.Add(signedAttendee);
                }
            }

            MeetingCache.ProcessSearchingAndSorting(ref attendees, ref signedAttendees, searchString, searchStringAccording, sortOrder, signedSortOrder);

            Attendees = attendees.ToPagedList(pageNumber, pageSize);
            SignedAttendees = signedAttendees.ToPagedList(signedPageNumber, pageSize);

            UserProfile userProfile = UserProfileCache.GetDetail(db, registrarId);
            UserAttendee = new MeetingSignUnsignAttendee
            {
                AttendeeId = registrarId,
                City = userProfile.City,
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                Signed = meeting.MeetingAttendees != null && meeting.MeetingAttendees.Any(ma => ma.UserAttendeeId == registrarId)
            };

            _availableForProcess = true;
        }

        protected void ProcessUserSignUsign(Meeting meeting, DefaultContext db, int id, bool isAdmin, int userId, string searchString, string searchStringAccording, string sortOrder, string signedSortOrder, int pageNumber, int pageSize, int signedPageNumber)
        {
            if (!_availableForProcess)
                return;

            _availableForProcess = false;

            if (meeting.RegisterDeadline.GetValueOrDefault(DateTime.MaxValue) < DateTime.Now)
                return;

            IEnumerable<UserProfileIndex> userProfiles = isAdmin
                                                             ? UserProfileIndex.GetUserProfileIndexForAdmin(db, userId, searchString, searchStringAccording, sortOrder)
                                                             : UserProfileIndex.GetUserProfileIndex(db, userId, searchString, searchStringAccording, sortOrder);

            var attendees = new List<MeetingSignUnsignAttendee>();
            var signedAttendees = new List<MeetingSignUnsignAttendee>();
            foreach (UserProfileIndex userProfile in userProfiles)
            {
                bool signed = false;
                if (meeting.MeetingAttendees != null && meeting.MeetingAttendees.Any())
                {
                    signed = meeting.MeetingAttendees.Any(ma => ma.UserAttendeeId == userProfile.UserId);
                }

                if (!signed)
                {
                    var attendee = new MeetingSignUnsignAttendee
                    {
                        AttendeeId = userProfile.UserId,
                        Signed = false,
                        FirstName = userProfile.FirstName,
                        LastName = userProfile.LastName,
                        City = userProfile.City
                    };
                    attendees.Add(attendee);
                }
                else
                {
                    var signedAttendee = new MeetingSignUnsignAttendee
                    {
                        AttendeeId = userProfile.UserId,
                        Signed = true,
                        FirstName = userProfile.FirstName,
                        LastName = userProfile.LastName,
                        City = userProfile.City
                    };
                    signedAttendees.Add(signedAttendee);
                }
            }

            MeetingCache.ProcessSearchingAndSorting(ref attendees, ref signedAttendees, searchString, searchStringAccording, sortOrder, signedSortOrder);

            Attendees = attendees.ToPagedList(pageNumber, pageSize);
            SignedAttendees = signedAttendees.ToPagedList(signedPageNumber, pageSize);

            UserProfile user = UserProfileCache.GetDetail(db, userId);
            UserAttendee = new MeetingSignUnsignAttendee
            {
                AttendeeId = user.UserId,
                City = user.City,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Signed = meeting.MeetingAttendees != null && meeting.MeetingAttendees.Any(ma => ma.UserAttendeeId == user.UserId)
            };

            _availableForProcess = true;
        }
    }

    public class MeetingBusinessInfoDetail : MeetingDetail
    {
        [Display(Name = "Global_City_Name", ResourceType = typeof(FieldResource))]
        public string City { get; set; }

        [Display(Name = "Meeting_AddressLine1_Name", ResourceType = typeof(FieldResource))]
        public string AddressLine1 { get; set; }

        [Display(Name = "Meeting_MeetingType_Name", ResourceType = typeof(FieldResource))]
        public MeetingType MeetingType { get; set; }

        [Display(Name = "Global_Date_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime Date { get; set; }

        [Display(Name = "Meeting_StartTime_Name", ResourceType = typeof(FieldResource))]
        public string StartTime { get; set; }

        [Display(Name = "Meeting_FinishTime_Name", ResourceType = typeof(FieldResource))]
        public string FinishTime { get; set; }

        public MeetingBusinessInfoDetail(Meeting meeting, DefaultContext db, int userId, bool isAdmin)
            : base(meeting, db, userId, isAdmin)
        {
            if (meeting == null || meeting.MeetingType != MeetingType.BusinessInfo)
                return;

            City = meeting.City;
            AddressLine1 = meeting.AddressLine1;
            MeetingType = MeetingType.BusinessInfo;
            MeetingKind = MeetingKind.Public;
            Date = meeting.Started.Date;
            StartTime = meeting.Started.ToString("HH:mm");
            FinishTime = meeting.Finished.ToString("HH:mm");

            var attendees = new List<MeetingDetailAttendee>();
            foreach (MeetingAttendee meetingAttendee in meeting.MeetingAttendees.Where(ma => ma.AttendeeId.HasValue || ma.UserAttendeeId.HasValue))
            {
                MeetingDetailAttendee attendee;
                if (meetingAttendee.AttendeeId.HasValue)
                {
                    bool showContactAcronym = ShowContactAcronym(db, userId, meetingAttendee.Attendee.RegistrarId);
                    attendee = new MeetingDetailAttendee
                    {
                        MeetingAttendeeId = meetingAttendee.MeetingAttendeeId,
                        FirstName = showContactAcronym
                                                       ? meetingAttendee.Attendee.FirstNameAcronym
                                                       : meetingAttendee.Attendee.FirstName,
                        LastName = showContactAcronym
                                                      ? meetingAttendee.Attendee.LastNameAcronym
                                                      : meetingAttendee.Attendee.LastName,
                        City = meetingAttendee.Attendee.CityIndexView,
                        LyonessId = meetingAttendee.Attendee.LyonessId,
                        Leader = meetingAttendee.Attendee.Registrar.FullNameWithoutLyonessId,
                        AccessUnsign =
                                           isAdmin || meeting.OrganizerId == userId ||
                                           meeting.SecondaryOrganizerId == userId ||
                                           meetingAttendee.Attendee.RegistrarId == userId
                    };
                }
                else
                {
                    attendee = new MeetingDetailAttendee
                    {
                        MeetingAttendeeId = meetingAttendee.MeetingAttendeeId,
                        FirstName = meetingAttendee.UserAttendee.FirstName,
                        LastName = meetingAttendee.UserAttendee.LastName,
                        City = meetingAttendee.UserAttendee.CityIndexView,
                        LyonessId = meetingAttendee.UserAttendee.LyonessId,
                        Leader = meetingAttendee.UserAttendee.Registrar == null
                         ? NullDisplayText
                         : meetingAttendee.UserAttendee.Registrar.FullNameWithoutLyonessId,
                        AccessUnsign = isAdmin || meeting.OrganizerId == userId || meeting.SecondaryOrganizerId == userId || meetingAttendee.UserAttendeeId == userId
                    };
                }

                attendees.Add(attendee);
            }

            MeetingAttendees = attendees.ToArray();
        }

        public MeetingBusinessInfoDetail(Meeting meeting, DefaultContext db, int userId, bool isAdmin, string sortOrder)
            : this(meeting, db, userId, isAdmin)
        {
            MeetingAttendees = MeetingCache.ProcessSorting(MeetingAttendees, sortOrder);
        }
    }

    public class MeetingWebinarDetail : MeetingDetail
    {
        [Display(Name = "Meeting_MeetingType_Name", ResourceType = typeof(FieldResource))]
        public MeetingType MeetingType { get; set; }

        [Display(Name = "Global_Date_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime Date { get; set; }

        [Display(Name = "Meeting_StartTime_Name", ResourceType = typeof(FieldResource))]
        public string StartTime { get; set; }

        [Display(Name = "Meeting_FinishTime_Name", ResourceType = typeof(FieldResource))]
        public string FinishTime { get; set; }

        [Display(Name = "Meeting_WebinarUrl_Name", ResourceType = typeof(FieldResource))]
        public string WebinarUrl { get; set; }

        [Display(Name = "Meeting_FillCapacity_Name", ResourceType = typeof(FieldResource))]
        public int FillCapacity { get; set; }

        public MeetingWebinarDetail(Meeting meeting, DefaultContext db, int userId, bool isAdmin)
            : base(meeting, db, userId, isAdmin)
        {
            if (meeting == null || meeting.MeetingType != MeetingType.Webinar)
                return;

            MeetingType = MeetingType.Webinar;
            MeetingKind = MeetingKind.Public;
            Date = meeting.Started.Date;
            StartTime = meeting.Started.ToString("HH:mm");
            FinishTime = meeting.Finished.ToString("HH:mm");
            WebinarUrl = meeting.WebinarUrl;
            FillCapacity = meeting.FillCapacity;

            var attendees = new List<MeetingDetailAttendee>();
            foreach (MeetingAttendee meetingAttendee in meeting.MeetingAttendees.Where(ma => ma.AttendeeId.HasValue || ma.UserAttendeeId.HasValue))
            {
                MeetingDetailAttendee attendee;
                if (meetingAttendee.AttendeeId.HasValue)
                {
                    bool showContactAcronym = ShowContactAcronym(db, userId, meetingAttendee.Attendee.RegistrarId);
                    attendee = new MeetingDetailAttendee
                    {
                        MeetingAttendeeId = meetingAttendee.MeetingAttendeeId,
                        FirstName = showContactAcronym
                                        ? meetingAttendee.Attendee.FirstNameAcronym
                                        : meetingAttendee.Attendee.FirstName,
                        LastName = showContactAcronym
                                       ? meetingAttendee.Attendee.LastNameAcronym
                                       : meetingAttendee.Attendee.LastName,
                        City = meetingAttendee.Attendee.CityIndexView,
                        LyonessId = meetingAttendee.Attendee.LyonessId,
                        Leader = meetingAttendee.Attendee.Registrar.FullNameWithoutLyonessId,
                        AccessUnsign =
                            isAdmin || meeting.OrganizerId == userId ||
                            meeting.SecondaryOrganizerId == userId ||
                            meetingAttendee.Attendee.RegistrarId == userId
                    };
                }
                else
                {
                    attendee = new MeetingDetailAttendee
                    {
                        MeetingAttendeeId = meetingAttendee.MeetingAttendeeId,
                        FirstName = meetingAttendee.UserAttendee.FirstName,
                        LastName = meetingAttendee.UserAttendee.LastName,
                        City = meetingAttendee.UserAttendee.CityIndexView,
                        LyonessId = meetingAttendee.UserAttendee.LyonessId,
                        Leader = meetingAttendee.UserAttendee.Registrar == null
                         ? NullDisplayText
                         : meetingAttendee.UserAttendee.Registrar.FullNameWithoutLyonessId,
                        AccessUnsign = isAdmin || meeting.OrganizerId == userId || meeting.SecondaryOrganizerId == userId || meetingAttendee.UserAttendeeId == userId
                    };
                }

                attendees.Add(attendee);
            }

            MeetingAttendees = attendees.ToArray();
        }

        public MeetingWebinarDetail(Meeting meeting, DefaultContext db, int userId, bool isAdmin, string sortOrder)
            : this(meeting, db, userId, isAdmin)
        {
            MeetingAttendees = MeetingCache.ProcessSorting(MeetingAttendees, sortOrder);
        }
    }

    public class MeetingMspEveningDetail : MeetingDetail
    {
        [Display(Name = "Global_City_Name", ResourceType = typeof(FieldResource))]
        public string City { get; set; }

        [Display(Name = "Meeting_AddressLine1_Name", ResourceType = typeof(FieldResource))]
        public string AddressLine1 { get; set; }

        [Display(Name = "Meeting_MeetingType_Name", ResourceType = typeof(FieldResource))]
        public MeetingType MeetingType { get; set; }

        [Display(Name = "Global_Date_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime Date { get; set; }

        [Display(Name = "Meeting_StartTime_Name", ResourceType = typeof(FieldResource))]
        public string StartTime { get; set; }

        [Display(Name = "Meeting_FinishTime_Name", ResourceType = typeof(FieldResource))]
        public string FinishTime { get; set; }

        public MeetingMspEveningDetail(Meeting meeting, DefaultContext db, int userId, bool isAdmin)
            : base(meeting, db, userId, isAdmin)
        {
            if (meeting == null || meeting.MeetingType != MeetingType.MspEvening)
                return;

            City = meeting.City;
            AddressLine1 = meeting.AddressLine1;
            MeetingType = MeetingType.MspEvening;
            MeetingKind = MeetingKind.MSP;
            Date = meeting.Started.Date;
            StartTime = meeting.Started.ToString("HH:mm");
            FinishTime = meeting.Finished.ToString("HH:mm");

            var attendees = new List<MeetingDetailAttendee>();
            foreach (MeetingAttendee meetingAttendee in meeting.MeetingAttendees.Where(ma => ma.AttendeeId.HasValue || ma.UserAttendeeId.HasValue))
            {
                MeetingDetailAttendee attendee;
                if (meetingAttendee.AttendeeId.HasValue)
                {
                    bool showContactAcronym = ShowContactAcronym(db, userId, meetingAttendee.Attendee.RegistrarId);
                    attendee = new MeetingDetailAttendee
                    {
                        MeetingAttendeeId = meetingAttendee.MeetingAttendeeId,
                        FirstName = showContactAcronym ? meetingAttendee.Attendee.FirstNameAcronym : meetingAttendee.Attendee.FirstName,
                        LastName = showContactAcronym ? meetingAttendee.Attendee.LastNameAcronym : meetingAttendee.Attendee.LastName,
                        City = meetingAttendee.Attendee.CityIndexView,
                        LyonessId = meetingAttendee.Attendee.LyonessId,
                        Leader = meetingAttendee.Attendee.Registrar.FullNameWithoutLyonessId,
                        AccessUnsign = isAdmin || meeting.OrganizerId == userId || meeting.SecondaryOrganizerId == userId || meetingAttendee.Attendee.RegistrarId == userId
                    };
                }
                else
                {
                    attendee = new MeetingDetailAttendee
                    {
                        MeetingAttendeeId = meetingAttendee.MeetingAttendeeId,
                        FirstName = meetingAttendee.UserAttendee.FirstName,
                        LastName = meetingAttendee.UserAttendee.LastName,
                        City = meetingAttendee.UserAttendee.CityIndexView,
                        LyonessId = meetingAttendee.UserAttendee.LyonessId,
                        Leader = meetingAttendee.UserAttendee.Registrar == null
                         ? NullDisplayText
                         : meetingAttendee.UserAttendee.Registrar.FullNameWithoutLyonessId,
                        AccessUnsign = isAdmin || meeting.OrganizerId == userId || meeting.SecondaryOrganizerId == userId || meetingAttendee.UserAttendeeId == userId
                    };
                }

                attendees.Add(attendee);
            }

            MeetingAttendees = attendees.ToArray();
        }

        public MeetingMspEveningDetail(Meeting meeting, DefaultContext db, int userId, bool isAdmin, string sortOrder)
            : this(meeting, db, userId, isAdmin)
        {
            MeetingAttendees = MeetingCache.ProcessSorting(MeetingAttendees, sortOrder);
        }
    }

    public class MeetingSetkaniTymuDetail : MeetingDetail
    {
        [Display(Name = "Global_City_Name", ResourceType = typeof(FieldResource))]
        public string City { get; set; }

        [Display(Name = "Meeting_AddressLine1_Name", ResourceType = typeof(FieldResource))]
        public string AddressLine1 { get; set; }

        [Display(Name = "Meeting_MeetingType_Name", ResourceType = typeof(FieldResource))]
        public string MeetingTitleType { get; set; }

        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Display(Name = "Meeting_StartTime_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime DateStartTime { get; set; }

        [Display(Name = "Meeting_FinishTime_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime DateFinishTime { get; set; }

        [Display(Name = "Meeting_InvitationCardUrl_Name", ResourceType = typeof(FieldResource))]
        public string InvitationCardUrl { get; set; }

        [Display(Name = "Meeting_Price_Name", ResourceType = typeof(FieldResource))]
        public string Price { get; set; }

        [Display(Name = "Meeting_Price_Name", ResourceType = typeof(FieldResource))]
        public string SecondPrice { get; set; }

        [Display(Name = "Meeting_RegisterDeadline_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime RegisterDeadline { get; set; }

        [Display(Name = "Meeting_SpecificSymbol_Name", ResourceType = typeof(FieldResource))]
        public int Ss { get; set; }

        [AllowHtml]
        [Display(Name = "Meeting_BringYourOwn_Name", ResourceType = typeof(FieldResource))]
        public string BringYourOwn { get; set; }

        public MeetingSetkaniTymuDetail(Meeting meeting, DefaultContext db, int userId, bool isAdmin)
            : base(meeting, db, userId, isAdmin)
        {
            if (meeting == null || meeting.MeetingType != MeetingType.SetkaniTymu)
                return;

            City = meeting.City;
            AddressLine1 = meeting.AddressLine1;
            MeetingTitleType = meeting.MeetingTitleType.Title;
            Title = meeting.Title;
            MeetingKind = MeetingKind.Public;
            Title = meeting.Title;
            DateStartTime = meeting.Started;
            DateFinishTime = meeting.Finished;

            InvitationCardUrl = meeting.InvitationCardUrl;
            Price = meeting.Price.HasValue && meeting.BankAccount != null ? $"{meeting.Price.GetValueOrDefault():N0} {meeting.BankAccount.CurrencyType}" : NullDisplayText;
            SecondPrice = meeting.SecondPrice.HasValue && meeting.SecondBankAccount != null ? $"{meeting.SecondPrice.GetValueOrDefault():N0} {meeting.SecondBankAccount.CurrencyType}" : NullDisplayText;
            RegisterDeadline = meeting.RegisterDeadline.GetValueOrDefault();

            SetAccountIdText(meeting.BankAccount, meeting.SecondBankAccount);

            Ss = meeting.MeetingId;
            BringYourOwn = meeting.BringYourOwn;

            AccessSignUnsign &= DateTime.Now <= meeting.RegisterDeadline.GetValueOrDefault();

            int[] currentUserDownlineUserIds = UserProfileCache.GetDownlineUserIdsWithoutAdmins(db, userId) ?? new int[0];
            var attendees = new List<MeetingDetailAttendee>();
            foreach (MeetingAttendee meetingAttendee in meeting.MeetingAttendees.Where(ma => ma.AttendeeId.HasValue || ma.UserAttendeeId.HasValue))
            {
                var formattingClass = new StringBuilder();
                formattingClass.Append($"{(!meetingAttendee.Registered.HasValue ? MeetingAttendeeReservedClass : MeetingAttendeeRegisteredClass)} ");
                formattingClass.Append(meetingAttendee.Reserved.GetValueOrDefault().Date == DateTime.MaxValue.Date ? MeetingAttendeeFreezedClass : Empty);

                string paidAmmount = $"{meetingAttendee.PaidAmount:N0} {meeting.BankAccount?.CurrencyType ?? default(CurrencyType)}";
                if (meeting.SecondBankAccountId != null)
                {
                    paidAmmount += $", {meetingAttendee.SecondPaidAmount:N0} {meeting.SecondBankAccount?.CurrencyType}";
                }

                MeetingDetailAttendee attendee;
                if (meetingAttendee.AttendeeId.HasValue)
                {
                    attendee = new MeetingDetailAttendee
                    {
                        MeetingAttendeeId = meetingAttendee.MeetingAttendeeId,
                        FirstName = meetingAttendee.Attendee.FirstName,
                        LastName = meetingAttendee.Attendee.LastName,
                        City = meetingAttendee.Attendee.CityIndexView,
                        LyonessId = meetingAttendee.Attendee.LyonessId,
                        PaidAmmount = paidAmmount,
                        FormattingClass = formattingClass.ToString(),
                        Leader = meetingAttendee.Attendee.Registrar.FullNameWithoutLyonessId,
                        AccessUnsign = isAdmin || meeting.OrganizerId == userId || meeting.SecondaryOrganizerId == userId || !meetingAttendee.Registered.HasValue && meetingAttendee.Reserved.GetValueOrDefault().Date != DateTime.MaxValue.Date && meetingAttendee.Attendee.RegistrarId == userId,
                        AccessLocking = !meetingAttendee.Registered.HasValue && (isAdmin || meeting.OrganizerId == userId || meeting.SecondaryOrganizerId == userId),
                        IsReservationLocked = meetingAttendee.Reserved.GetValueOrDefault().Date == DateTime.MaxValue.Date
                    };
                }
                else
                {
                    attendee = new MeetingDetailAttendee
                    {
                        MeetingAttendeeId = meetingAttendee.MeetingAttendeeId,
                        FirstName = meetingAttendee.UserAttendee.FirstName,
                        LastName = meetingAttendee.UserAttendee.LastName,
                        City = meetingAttendee.UserAttendee.CityIndexView,
                        LyonessId = meetingAttendee.UserAttendee.LyonessId,
                        PaidAmmount = paidAmmount,
                        FormattingClass = formattingClass.ToString(),
                        Leader = meetingAttendee.UserAttendee.Registrar == null
                         ? NullDisplayText
                         : meetingAttendee.UserAttendee.Registrar.FullNameWithoutLyonessId,
                        AccessUnsign = isAdmin || meeting.OrganizerId == userId || meeting.SecondaryOrganizerId == userId || !meetingAttendee.Registered.HasValue && meetingAttendee.Reserved.GetValueOrDefault().Date != DateTime.MaxValue.Date && (meetingAttendee.UserAttendeeId == userId || currentUserDownlineUserIds.Contains(meetingAttendee.UserAttendeeId.GetValueOrDefault())),
                        AccessLocking = !meetingAttendee.Registered.HasValue && (isAdmin || meeting.OrganizerId == userId || meeting.SecondaryOrganizerId == userId),
                        IsReservationLocked = meetingAttendee.Reserved.GetValueOrDefault().Date == DateTime.MaxValue.Date
                    };
                }

                attendees.Add(attendee);
            }

            MeetingAttendees = attendees.ToArray();
        }

        public MeetingSetkaniTymuDetail(Meeting meeting, DefaultContext db, int userId, bool isAdmin, string sortOrder)
            : this(meeting, db, userId, isAdmin)
        {
            MeetingAttendees = MeetingCache.ProcessSorting(MeetingAttendees, sortOrder);
        }
    }

    public class MeetingSkoleniDavidaKotaskaDetail : MeetingDetail
    {
        [Display(Name = "Global_City_Name", ResourceType = typeof(FieldResource))]
        public string City { get; set; }

        [Display(Name = "Meeting_AddressLine1_Name", ResourceType = typeof(FieldResource))]
        public string AddressLine1 { get; set; }

        [Display(Name = "Meeting_MeetingType_Name", ResourceType = typeof(FieldResource))]
        public MeetingType MeetingType { get; set; }

        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Display(Name = "Global_Date_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime Date { get; set; }

        [Display(Name = "Meeting_StartTime_Name", ResourceType = typeof(FieldResource))]
        public string StartTime { get; set; }

        [Display(Name = "Meeting_FinishTime_Name", ResourceType = typeof(FieldResource))]
        public string FinishTime { get; set; }

        [Display(Name = "Meeting_Price_Name", ResourceType = typeof(FieldResource))]
        public string Price { get; set; }

        [Display(Name = "Meeting_Price_Name", ResourceType = typeof(FieldResource))]
        public string SecondPrice { get; set; }

        [Display(Name = "Meeting_RegisterDeadline_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime RegisterDeadline { get; set; }

        [Display(Name = "Meeting_SpecificSymbol_Name", ResourceType = typeof(FieldResource))]
        public int Ss { get; set; }

        [Display(Name = "Meeting_BringYourOwn_Name", ResourceType = typeof(FieldResource))]
        public string BringYourOwn { get; set; }

        public MeetingSkoleniDavidaKotaskaDetail(Meeting meeting, DefaultContext db, int userId, bool isAdmin)
            : base(meeting, db, userId, isAdmin)
        {
            if (meeting == null || meeting.MeetingType != MeetingType.SkoleniDavidaKotaska)
                return;

            City = meeting.City;
            AddressLine1 = meeting.AddressLine1;
            MeetingType = MeetingType.SkoleniDavidaKotaska;
            MeetingKind = MeetingKind.Public;
            Title = meeting.Title;
            Date = meeting.Started.Date;
            StartTime = meeting.Started.ToString("HH:mm");
            FinishTime = meeting.Finished.ToString("HH:mm");

            Price = meeting.Price.HasValue && meeting.BankAccount != null ? $"{meeting.Price.GetValueOrDefault():N0} {meeting.BankAccount.CurrencyType}" : NullDisplayText;
            SecondPrice = meeting.SecondPrice.HasValue && meeting.SecondBankAccount != null ? $"{meeting.SecondPrice.GetValueOrDefault():N0} {meeting.SecondBankAccount.CurrencyType}" : NullDisplayText;
            RegisterDeadline = meeting.RegisterDeadline.GetValueOrDefault();

            SetAccountIdText(meeting.BankAccount, meeting.SecondBankAccount);

            Ss = meeting.MeetingId;
            BringYourOwn = meeting.BringYourOwn;

            AccessSignUnsign &= DateTime.Now <= meeting.RegisterDeadline.GetValueOrDefault();

            int[] currentUserDownlineUserIds = UserProfileCache.GetDownlineUserIdsWithoutAdmins(db, userId) ?? new int[0];
            var attendees = new List<MeetingDetailAttendee>();
            foreach (MeetingAttendee meetingAttendee in meeting.MeetingAttendees.Where(ma => ma.AttendeeId.HasValue || ma.UserAttendeeId.HasValue))
            {
                var formattingClass = new StringBuilder();
                formattingClass.Append($"{(!meetingAttendee.Registered.HasValue ? MeetingAttendeeReservedClass : MeetingAttendeeRegisteredClass)} ");
                formattingClass.Append(meetingAttendee.Reserved.GetValueOrDefault().Date == DateTime.MaxValue.Date ? MeetingAttendeeFreezedClass : Empty);

                string paidAmmount = $"{meetingAttendee.PaidAmount:N0} {meeting.BankAccount?.CurrencyType ?? default(CurrencyType)}";
                if (meeting.SecondBankAccountId != null)
                {
                    paidAmmount += $", {meetingAttendee.SecondPaidAmount:N0} {meeting.SecondBankAccount?.CurrencyType}";
                }

                MeetingDetailAttendee attendee;
                if (meetingAttendee.AttendeeId.HasValue)
                {
                    attendee = new MeetingDetailAttendee
                    {
                        MeetingAttendeeId = meetingAttendee.MeetingAttendeeId,
                        FirstName = meetingAttendee.Attendee.FirstName,
                        LastName = meetingAttendee.Attendee.LastName,
                        LyonessId = meetingAttendee.Attendee.LyonessId,
                        PaidAmmount = paidAmmount,
                        FormattingClass = formattingClass.ToString(),
                        City = meetingAttendee.Attendee.CityIndexView,
                        Leader = meetingAttendee.Attendee.Registrar.FullNameWithoutLyonessId,
                        AccessUnsign = isAdmin || meeting.OrganizerId == userId || meeting.SecondaryOrganizerId == userId || !meetingAttendee.Registered.HasValue && meetingAttendee.Reserved.GetValueOrDefault().Date != DateTime.MaxValue.Date && meetingAttendee.Attendee.RegistrarId == userId,
                        AccessLocking = !meetingAttendee.Registered.HasValue && (isAdmin || meeting.OrganizerId == userId || meeting.SecondaryOrganizerId == userId),
                        IsReservationLocked = meetingAttendee.Reserved.GetValueOrDefault().Date == DateTime.MaxValue.Date
                    };
                }
                else
                {
                    attendee = new MeetingDetailAttendee
                    {
                        MeetingAttendeeId = meetingAttendee.MeetingAttendeeId,
                        FirstName = meetingAttendee.UserAttendee.FirstName,
                        LastName = meetingAttendee.UserAttendee.LastName,
                        LyonessId = meetingAttendee.UserAttendee.LyonessId,
                        PaidAmmount = paidAmmount,
                        FormattingClass = formattingClass.ToString(),
                        City = meetingAttendee.UserAttendee.CityIndexView,
                        Leader = meetingAttendee.UserAttendee.Registrar == null
                         ? NullDisplayText
                         : meetingAttendee.UserAttendee.Registrar.FullNameWithoutLyonessId,
                        AccessUnsign = isAdmin || meeting.OrganizerId == userId || meeting.SecondaryOrganizerId == userId || !meetingAttendee.Registered.HasValue && meetingAttendee.Reserved.GetValueOrDefault().Date != DateTime.MaxValue.Date && (meetingAttendee.UserAttendeeId == userId || currentUserDownlineUserIds.Contains(meetingAttendee.UserAttendeeId.GetValueOrDefault())),
                        AccessLocking = !meetingAttendee.Registered.HasValue && (isAdmin || meeting.OrganizerId == userId || meeting.SecondaryOrganizerId == userId),
                        IsReservationLocked = meetingAttendee.Reserved.GetValueOrDefault().Date == DateTime.MaxValue.Date
                    };
                }

                attendees.Add(attendee);
            }

            MeetingAttendees = attendees.ToArray();
        }

        public MeetingSkoleniDavidaKotaskaDetail(Meeting meeting, DefaultContext db, int userId, bool isAdmin, string sortOrder)
            : this(meeting, db, userId, isAdmin)
        {
            MeetingAttendees = MeetingCache.ProcessSorting(MeetingAttendees, sortOrder);
        }
    }

    public class MeetingOstatniDetail : MeetingDetail
    {
        [Display(Name = "Global_City_Name", ResourceType = typeof(FieldResource))]
        public string City { get; set; }

        [Display(Name = "Meeting_AddressLine1_Name", ResourceType = typeof(FieldResource))]
        public string AddressLine1 { get; set; }

        [Display(Name = "Meeting_MeetingType_Name", ResourceType = typeof(FieldResource))]
        public string MeetingTitleType { get; set; }

        [Display(Name = "Global_Date_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime Date { get; set; }

        [Display(Name = "Meeting_StartTime_Name", ResourceType = typeof(FieldResource))]
        public string StartTime { get; set; }

        [Display(Name = "Meeting_FinishTime_Name", ResourceType = typeof(FieldResource))]
        public string FinishTime { get; set; }

        [Display(Name = "Meeting_Price_Name", ResourceType = typeof(FieldResource))]
        public string Price { get; set; }

        [Display(Name = "Meeting_Price_Name", ResourceType = typeof(FieldResource))]
        public string SecondPrice { get; set; }

        [Display(Name = "Meeting_RegisterDeadline_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true, NullDisplayText = NullDisplayText)]
        public DateTime? RegisterDeadline { get; set; }

        public bool ShowRegisterDeadline => RegisterDeadline.HasValue;

        [Display(Name = "Meeting_SpecificSymbol_Name", ResourceType = typeof(FieldResource))]
        public int Ss { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeetingOstatniDetail"/> class.
        /// </summary>
        /// <param name="meeting">The meeting.</param>
        /// <param name="db">The db.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="isAdmin">if set to <c>true</c> [is admin].</param>
        public MeetingOstatniDetail(Meeting meeting, DefaultContext db, int userId, bool isAdmin)
            : base(meeting, db, userId, isAdmin)
        {
            if (meeting == null || meeting.MeetingType != MeetingType.Ostatni)
                return;

            City = meeting.City;
            AddressLine1 = meeting.AddressLine1;
            MeetingTitleType = meeting.MeetingTitleType.Title;
            MeetingKind = MeetingKind.Public;
            Date = meeting.Started.Date;
            StartTime = meeting.Started.ToString("HH:mm");
            FinishTime = meeting.Finished.ToString("HH:mm");

            Price = meeting.Price.HasValue && meeting.BankAccount != null ? $"{meeting.Price.GetValueOrDefault():N0} {meeting.BankAccount.CurrencyType}" : NullDisplayText;
            SecondPrice = meeting.SecondPrice.HasValue && meeting.SecondBankAccount != null ? $"{meeting.SecondPrice.GetValueOrDefault():N0} {meeting.SecondBankAccount.CurrencyType}" : NullDisplayText;
            RegisterDeadline = meeting.RegisterDeadline;

            SetAccountIdText(meeting.BankAccount, meeting.SecondBankAccount);

            Ss = meeting.MeetingId;

            int[] currentUserDownlineUserIds = UserProfileCache.GetDownlineUserIdsWithoutAdmins(db, userId) ?? new int[0];
            var attendees = new List<MeetingDetailAttendee>();
            foreach (MeetingAttendee meetingAttendee in meeting.MeetingAttendees.Where(ma => ma.AttendeeId.HasValue || ma.UserAttendeeId.HasValue))
            {
                var formattingClass = new StringBuilder();
                formattingClass.Append($"{(!meetingAttendee.Registered.HasValue ? MeetingAttendeeReservedClass : MeetingAttendeeRegisteredClass)} ");
                formattingClass.Append(meetingAttendee.Reserved.GetValueOrDefault().Date == DateTime.MaxValue.Date ? MeetingAttendeeFreezedClass : Empty);

                string paidAmmount = $"{meetingAttendee.PaidAmount:N0} {meeting.BankAccount?.CurrencyType ?? default(CurrencyType)}";
                if (meeting.SecondBankAccountId != null)
                {
                    paidAmmount += $", {meetingAttendee.SecondPaidAmount:N0} {meeting.SecondBankAccount?.CurrencyType}";
                }

                MeetingDetailAttendee attendee;
                if (meetingAttendee.AttendeeId.HasValue)
                {
                    bool showContactAcronym = ShowContactAcronym(db, userId, meetingAttendee.Attendee.RegistrarId);
                    attendee = new MeetingDetailAttendee
                    {
                        MeetingAttendeeId = meetingAttendee.MeetingAttendeeId,
                        FirstName = showContactAcronym ? meetingAttendee.Attendee.FirstNameAcronym : meetingAttendee.Attendee.FirstName,
                        LastName = showContactAcronym ? meetingAttendee.Attendee.LastNameAcronym : meetingAttendee.Attendee.LastName,
                        City = meetingAttendee.Attendee.CityIndexView,
                        LyonessId = meetingAttendee.Attendee.LyonessId,
                        PaidAmmount = paidAmmount,
                        FormattingClass = formattingClass.ToString(),
                        Leader = meetingAttendee.Attendee.Registrar.FullNameWithoutLyonessId,
                        AccessUnsign = isAdmin || meeting.OrganizerId == userId || meeting.SecondaryOrganizerId == userId || !meetingAttendee.Registered.HasValue && meetingAttendee.Reserved.GetValueOrDefault().Date != DateTime.MaxValue.Date && meetingAttendee.Attendee.RegistrarId == userId,
                        AccessLocking = !meetingAttendee.Registered.HasValue && (isAdmin || meeting.OrganizerId == userId || meeting.SecondaryOrganizerId == userId),
                        IsReservationLocked = meetingAttendee.Reserved.GetValueOrDefault().Date == DateTime.MaxValue.Date
                    };
                }
                else
                {
                    attendee = new MeetingDetailAttendee
                    {
                        MeetingAttendeeId = meetingAttendee.MeetingAttendeeId,
                        FirstName = meetingAttendee.UserAttendee.FirstName,
                        LastName = meetingAttendee.UserAttendee.LastName,
                        City = meetingAttendee.UserAttendee.CityIndexView,
                        LyonessId = meetingAttendee.UserAttendee.LyonessId,
                        PaidAmmount = paidAmmount,
                        FormattingClass = formattingClass.ToString(),
                        Leader = meetingAttendee.UserAttendee.Registrar == null
                                                 ? NullDisplayText
                                                 : meetingAttendee.UserAttendee.Registrar.FullNameWithoutLyonessId,
                        AccessUnsign = isAdmin || meeting.OrganizerId == userId || meeting.SecondaryOrganizerId == userId || !meetingAttendee.Registered.HasValue && meetingAttendee.Reserved.GetValueOrDefault().Date != DateTime.MaxValue.Date && (meetingAttendee.UserAttendeeId == userId || currentUserDownlineUserIds.Contains(meetingAttendee.UserAttendeeId.GetValueOrDefault())),
                        AccessLocking = !meetingAttendee.Registered.HasValue && (isAdmin || meeting.OrganizerId == userId || meeting.SecondaryOrganizerId == userId),
                        IsReservationLocked = meetingAttendee.Reserved.GetValueOrDefault().Date == DateTime.MaxValue.Date
                    };
                }

                attendees.Add(attendee);
            }

            MeetingAttendees = attendees.ToArray();
        }

        public MeetingOstatniDetail(Meeting meeting, DefaultContext db, int userId, bool isAdmin, string sortOrder)
            : this(meeting, db, userId, isAdmin)
        {
            MeetingAttendees = MeetingCache.ProcessSorting(MeetingAttendees, sortOrder);
        }
    }

    public class MeetingSignUnsignAttendee : BaseModelView
    {
        public int AttendeeId { get; set; }

        public bool Signed { get; set; }

        [Display(Name = "Global_FirstName_Name", ResourceType = typeof(FieldResource))]
        public string FirstName { get; set; }

        [Display(Name = "Global_LastName_Name", ResourceType = typeof(FieldResource))]
        public string LastName { get; set; }

        [Display(Name = "Meeting_Residence_Name", ResourceType = typeof(FieldResource))]
        public string City { get; set; }
    }

    public class MeetingBusinessInfoSignUnsign : MeetingBusinessInfoDetail
    {
        public MeetingBusinessInfoSignUnsign(Meeting meeting, DefaultContext db, int id, bool isAdmin, int registrarId, string searchString, string searchStringAccording, string sortOrder, string signedSortOrder, int pageNumber, int pageSize, int signedPageNumber)
            : base(meeting, db, id, isAdmin)
        {
            IQueryable<PeopleContact> peopleContacts = PeopleContactCache.GetIndex(db, registrarId);

            ProcessSignUsign(meeting, peopleContacts, db, id, isAdmin, registrarId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
        }
    }

    public class MeetingBusinessInfoUserSignUnsign : MeetingBusinessInfoDetail
    {
        public MeetingBusinessInfoUserSignUnsign(Meeting meeting, DefaultContext db, int id, bool isAdmin, int userId, string searchString, string searchStringAccording, string sortOrder, string signedSortOrder, int pageNumber, int pageSize, int signedPageNumber)
            : base(meeting, db, id, isAdmin)
        {
            ProcessUserSignUsign(meeting, db, id, isAdmin, userId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
        }
    }

    public class MeetingWebinarSignUnsign : MeetingWebinarDetail
    {
        public MeetingWebinarSignUnsign(Meeting meeting, DefaultContext db, int id, bool isAdmin, int registrarId, string searchString, string searchStringAccording, string sortOrder, string signedSortOrder, int pageNumber, int pageSize, int signedPageNumber)
            : base(meeting, db, id, isAdmin)
        {
            IQueryable<PeopleContact> peopleContacts = PeopleContactCache.GetIndex(db, registrarId);

            ProcessSignUsign(meeting, peopleContacts, db, id, isAdmin, registrarId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
        }
    }

    public class MeetingMspEveningSignUnsign : MeetingMspEveningDetail
    {
        public MeetingMspEveningSignUnsign(Meeting meeting, DefaultContext db, int id, bool isAdmin, int registrarId, string searchString, string searchStringAccording, string sortOrder, string signedSortOrder, int pageNumber, int pageSize, int signedPageNumber)
            : base(meeting, db, id, isAdmin)
        {
            IQueryable<PeopleContact> peopleContacts = PeopleContactCache.GetIndex(db, registrarId);

            ProcessSignUsign(meeting, peopleContacts, db, id, isAdmin, registrarId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
        }
    }

    public class MeetingSetkaniTymuSignUnsign : MeetingSetkaniTymuDetail
    {
        public MeetingSetkaniTymuSignUnsign(Meeting meeting, DefaultContext db, int id, bool isAdmin, int registrarId, string searchString, string searchStringAccording, string sortOrder, string signedSortOrder, int pageNumber, int pageSize, int signedPageNumber)
            : base(meeting, db, id, isAdmin)
        {
            IQueryable<PeopleContact> peopleContacts = PeopleContactCache.GetIndex(db, registrarId)
                .Where(pc => !IsNullOrEmpty(pc.LyonessId) && !IsNullOrEmpty(pc.PhoneNumber1) && !IsNullOrEmpty(pc.Email1));

            ProcessSignUsign(meeting, peopleContacts, db, id, isAdmin, registrarId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
        }
    }

    public class MeetingSkoleniDavidaKotaskaSignUnsign : MeetingSkoleniDavidaKotaskaDetail
    {
        public MeetingSkoleniDavidaKotaskaSignUnsign(Meeting meeting, DefaultContext db, int id, bool isAdmin, int registrarId, string searchString, string searchStringAccording, string sortOrder, string signedSortOrder, int pageNumber, int pageSize, int signedPageNumber)
            : base(meeting, db, id, isAdmin)
        {
            IQueryable<PeopleContact> peopleContacts = PeopleContactCache.GetIndex(db, registrarId)
                .Where(pc => !IsNullOrEmpty(pc.LyonessId) && !IsNullOrEmpty(pc.PhoneNumber1) && !IsNullOrEmpty(pc.Email1));

            ProcessSignUsign(meeting, peopleContacts, db, id, isAdmin, registrarId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
        }
    }

    public class MeetingSetkaniTymuUserSignUnsign : MeetingSetkaniTymuDetail
    {
        public MeetingSetkaniTymuUserSignUnsign(Meeting meeting, DefaultContext db, int id, bool isAdmin, int userId, string searchString, string searchStringAccording, string sortOrder, string signedSortOrder, int pageNumber, int pageSize, int signedPageNumber)
            : base(meeting, db, id, isAdmin)
        {
            ProcessUserSignUsign(meeting, db, id, isAdmin, userId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
        }
    }

    public class MeetingSkoleniDavidaKotaskaUserSignUnsign : MeetingSkoleniDavidaKotaskaDetail
    {
        public MeetingSkoleniDavidaKotaskaUserSignUnsign(Meeting meeting, DefaultContext db, int id, bool isAdmin, int userId, string searchString, string searchStringAccording, string sortOrder, string signedSortOrder, int pageNumber, int pageSize, int signedPageNumber)
            : base(meeting, db, id, isAdmin)
        {
            ProcessUserSignUsign(meeting, db, id, isAdmin, userId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
        }
    }

    public class MeetingOstatniSignUnsign : MeetingOstatniDetail
    {
        public MeetingOstatniSignUnsign(Meeting meeting, DefaultContext db, int id, bool isAdmin, int registrarId, string searchString, string searchStringAccording, string sortOrder, string signedSortOrder, int pageNumber, int pageSize, int signedPageNumber)
            : base(meeting, db, id, isAdmin)
        {
            IQueryable<PeopleContact> peopleContacts = PeopleContactCache.GetIndex(db, registrarId);

            ProcessSignUsign(meeting, peopleContacts, db, id, isAdmin, registrarId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
        }
    }

    public class MeetingOstatniUserSignUnsign : MeetingOstatniDetail
    {
        public MeetingOstatniUserSignUnsign(Meeting meeting, DefaultContext db, int id, bool isAdmin, int userId, string searchString, string searchStringAccording, string sortOrder, string signedSortOrder, int pageNumber, int pageSize, int signedPageNumber)
            : base(meeting, db, id, isAdmin)
        {
            ProcessUserSignUsign(meeting, db, id, isAdmin, userId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
        }
    }
}