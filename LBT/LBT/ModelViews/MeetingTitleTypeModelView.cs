using LBT.Models;
using LBT.Resources;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LBT.ModelViews
{
    public class MeetingTitleTypeEdit : BaseModelView
    {
        public int MeetingTitleTypeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [StringLength(50, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Meeting_MeetingType_Name", ResourceType = typeof(FieldResource))]
        public MeetingType MeetingType { get; set; }

        [Display(Name = "Meeting_MeetingType_Name", ResourceType = typeof(FieldResource))]
        public string MeetingTypeCzechText { get; private set; }

        public MeetingTitleTypeEdit()
        {
            MeetingTypeCzechText = Meeting.GetMeetingTypeLocalizedText(MeetingType);
        }

        public MeetingTitleTypeEdit(MeetingTitleType meetingTitleType)
            : this()
        {
            MeetingTitleTypeId = meetingTitleType.MeetingTitleTypeId;
            Title = meetingTitleType.Title;
            MeetingType = meetingTitleType.MeetingType;
        }

        public static MeetingTitleTypeEdit GetModelView(MeetingTitleType meetingTitleType)
        {
            if (meetingTitleType == null)
                return null;

            var meetingTitleTypeEdit = new MeetingTitleTypeEdit(meetingTitleType);
            return meetingTitleTypeEdit;
        }

        public MeetingTitleType GetModel()
        {
            var meetingTitleType = new MeetingTitleType
                                       {
                                           MeetingTitleTypeId = MeetingTitleTypeId,
                                           Title = Title,
                                           MeetingType = MeetingType
                                       };
            return meetingTitleType;
        }
    }

    public class MeetingTitleTypeDetails : BaseModelView
    {
        public int MeetingTitleTypeId { get; set; }

        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Display(Name = "Meeting_MeetingType_Name", ResourceType = typeof(FieldResource))]
        public string MeetingTypeCzechText { get; private set; }

        public MeetingTitleTypeDetails(MeetingTitleType meetingTitleType)
        {
            MeetingTitleTypeId = meetingTitleType.MeetingTitleTypeId;
            Title = meetingTitleType.Title;
            MeetingTypeCzechText = Meeting.GetMeetingTypeLocalizedText(meetingTitleType.MeetingType);
        }

        public static MeetingTitleTypeDetails GetModelView(MeetingTitleType meetingTitleType)
        {
            if (meetingTitleType == null)
                return null;

            var meetingTitleTypeDetails = new MeetingTitleTypeDetails(meetingTitleType);
            return meetingTitleTypeDetails;
        }
    }

    public class MeetingTitleTypeIndex : BaseModelView
    {
        public int MeetingTitleTypeId { get; set; }

        public string Title { get; set; }

        public string MeetingTypeCzechText { get; private set; }

        public MeetingTitleTypeIndex(MeetingTitleType meetingTitleType)
        {
            MeetingTitleTypeId = meetingTitleType.MeetingTitleTypeId;
            Title = meetingTitleType.Title;
            MeetingTypeCzechText = Meeting.GetMeetingTypeLocalizedText(meetingTitleType.MeetingType);
        }

        public static MeetingTitleTypeIndex[] GetModelView(MeetingTitleType[] meetingTitleTypes)
        {
            if (meetingTitleTypes == null)
                return null;

            MeetingTitleTypeIndex[] meetingTitleTypeIndices = meetingTitleTypes.Select(mtt => new MeetingTitleTypeIndex(mtt)).ToArray();

            return meetingTitleTypeIndices;
        }
    }
}