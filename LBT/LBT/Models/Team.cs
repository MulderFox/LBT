using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LBT.Models
{
    public class Team
    {
        public int TeamId { get; set; }

        [Required]
        public string Title { get; set; }

        [Column("OwnerId")]
        public virtual UserProfile Owner { get; set; }

        [ForeignKey("Owner")]
        public int OwnerId { get; set; }

        public virtual ICollection<TeamMember> TeamMembers { get; set; }

        public string Note { get; set; }

        public string Task { get; set; }

        public void CopyFrom(Team team)
        {
            Title = team.Title;
            Note = team.Note;
            Task = team.Task;
        }

        public void CopyNoteFrom(Team team)
        {
            Note = team.Note;
        }
    }
}