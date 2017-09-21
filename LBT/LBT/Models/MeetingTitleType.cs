using System.ComponentModel.DataAnnotations;

namespace LBT.Models
{
    public class MeetingTitleType
    {
        public int MeetingTitleTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        public MeetingType MeetingType { get; set; }

        public void CopyFrom(MeetingTitleType meetingTitleType)
        {
            Title = meetingTitleType.Title;
        }
    }
}