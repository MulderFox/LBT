using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LBT.Models
{
    public class VideoToken
    {
        public int VideoTokenId { get; set; }

        [ForeignKey("Video")]
        public int VideoId { get; set; }

        [Column("VideoId")]
        public virtual Video Video { get; set; }

        [ForeignKey("Sender")]
        public int? SenderId { get; set; }

        [Column("SenderId")]
        public virtual UserProfile Sender { get; set; }

        [ForeignKey("Recipient")]
        public int? RecipientId { get; set; }

        [Column("RecipientId")]
        public virtual PeopleContact Recipient { get; set; }

        public bool IsPlayedByRecipient { get; set; }

        public DateTime Expired { get; set; }

        public void CopyFrom(VideoToken videoToken)
        {
            VideoId = videoToken.VideoId;
            SenderId = videoToken.SenderId;
            RecipientId = videoToken.RecipientId;
            IsPlayedByRecipient = videoToken.IsPlayedByRecipient;
            Expired = videoToken.Expired;
        }
    }
}