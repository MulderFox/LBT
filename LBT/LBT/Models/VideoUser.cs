using System.ComponentModel.DataAnnotations.Schema;

namespace LBT.Models
{
    public class VideoUser
    {
        public int VideoUserId { get; set; }

        [ForeignKey("Video")]
        public int VideoId { get; set; }

        [Column("VideoId")]
        public virtual Video Video { get; set; }

        [ForeignKey("UserProfile")]
        public int UserProfileId { get; set; }

        [Column("UserProfileId")]
        public virtual UserProfile UserProfile { get; set; }
    }
}