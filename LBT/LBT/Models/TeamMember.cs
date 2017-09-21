using System.ComponentModel.DataAnnotations.Schema;

namespace LBT.Models
{
    public class TeamMember
    {
        public int TeamMemberId { get; set; }

        [Column("TeamId")]
        public virtual Team Team { get; set; }

        [ForeignKey("Team")]
        public int TeamId { get; set; }

        [Column("MemberId")]
        public virtual UserProfile Member { get; set; }

        [ForeignKey("Member")]
        public int MemberId { get; set; }
    }
}