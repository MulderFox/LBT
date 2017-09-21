using LBT.Cache;
using LBT.DAL;
using LBT.Models;
using LBT.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace LBT.ModelViews
{
    public class TeamIndex : BaseModelView
    {
        public int TeamId { get; set; }

        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        public bool AllowEdit { get; set; }

        private TeamIndex(Team team, int userId, bool isAdmin)
        {
            TeamId = team.TeamId;
            Title = team.Title;
            AllowEdit = team.OwnerId == userId || isAdmin;
        }

        public static TeamIndex[] GetIndex(DefaultContext db, string sortOrder, int userId, bool isAdmin)
        {
            TeamIndex[] teamIndexList = (from team in TeamCache.GetIndex(db, sortOrder)
                                         where
                                             isAdmin || team.OwnerId == userId ||
                                             team.TeamMembers.Any(tm => tm.MemberId == userId)
                                         select new TeamIndex(team, userId, isAdmin)).ToArray();
            return teamIndexList;
        }
    }

    public sealed class TeamEdit : BaseModelView
    {
        public int TeamId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Display(Name = "Team_MemberIds_Name", ResourceType = typeof(FieldResource))]
        public int[] MemberIds { get; set; }

        [AllowHtml]
        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        public string Note { get; set; }

        [AllowHtml]
        [Display(Name = "Global_Tasks_Name", ResourceType = typeof(FieldResource))]
        public string Task { get; set; }

        public TeamEdit()
        {
            
        }

        public TeamEdit(Team team)
        {
            TeamId = team.TeamId;
            Title = team.Title;
            MemberIds = team.TeamMembers.Select(tm => tm.MemberId).ToArray();
            Note = team.Note;
            Task = team.Task;
        }

        public static TeamEdit GetModelView(Team team)
        {
            if (team == null)
                return null;

            var teamEdit = new TeamEdit(team);
            return teamEdit;
        }

        public Team GetModel(int userId, out int[] memberIds)
        {
            memberIds = MemberIds ?? new int[0];

            var team = new Team
            {
                TeamId = TeamId,
                Title = Title,
                OwnerId = userId,
                Task = Task,
                Note = Note
            };
            return team;
        }

        public ModelStateDictionary Validate(DefaultContext db)
        {
            var modelStateDictionary = new ModelStateDictionary();
            ValidateTitle(db, ref modelStateDictionary);

            return modelStateDictionary;
        }

        private void ValidateTitle(DefaultContext db, ref ModelStateDictionary modelStateDictionary)
        {
            if (String.IsNullOrEmpty(Title))
            {
                modelStateDictionary.AddModelError(BaseCache.TitleField, String.Format(ValidationResource.Global_Required_ErrorMessage, FieldResource.Global_Title_Name));
                return;
            }

            Title = Title.Trim();

            if (db.Teams.Any(t => t.TeamId != TeamId && t.Title.Equals(Title, StringComparison.InvariantCultureIgnoreCase)))
            {
                modelStateDictionary.AddModelError(BaseCache.TitleField, String.Format(ValidationResource.Global_Unique_ErrorMesage, FieldResource.Global_Title_Name));
            }
        }
    }

    public sealed class TeamEditNote : BaseModelView
    {
        public int TeamId { get; set; }

        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Display(Name = "Team_Owner_Name", ResourceType = typeof(FieldResource))]
        public string Owner { get; set; }

        [Display(Name = "Global_Tasks_Name", ResourceType = typeof(FieldResource))]
        public string Task { get; set; }

        [AllowHtml]
        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        public string Note { get; set; }

        public TeamEditNote()
        {

        }

        public TeamEditNote(Team team)
        {
            TeamId = team.TeamId;
            Title = team.Title;
            Owner = team.Owner.FullName;
            Task = String.IsNullOrEmpty(team.Task) ? NullDisplayText : team.Task;
            Note = team.Note;
        }

        public static TeamEditNote GetModelView(Team team)
        {
            if (team == null)
                return null;

            var teamEditNote = new TeamEditNote(team);
            return teamEditNote;
        }

        public Team GetModel(int userId)
        {
            var team = new Team
            {
                TeamId = TeamId,
                Title = Title,
                OwnerId = userId,
                Note = Note
            };
            return team;
        }

        public ModelStateDictionary Validate(DefaultContext db)
        {
            var modelStateDictionary = new ModelStateDictionary();
            return modelStateDictionary;
        }
    }

    public sealed class TeamDetails : BaseModelView
    {
        public int TeamId { get; set; }

        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Display(Name = "Team_Owner_Name", ResourceType = typeof(FieldResource))]
        public string OwnerFullName { get; set; }

        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        public string Note { get; set; }

        [Display(Name = "Global_Tasks_Name", ResourceType = typeof(FieldResource))]
        public string Task { get; set; }

        public bool AllowEdit { get; set; }

        public TeamDetails(Team team, int userId, bool isAdmin)
        {
            TeamId = team.TeamId;
            Title = team.Title;
            OwnerFullName = team.Owner.FullName;
            Note = String.IsNullOrEmpty(team.Note) ? NullDisplayText : team.Note;
            Task = String.IsNullOrEmpty(team.Task) ? NullDisplayText : team.Task;
            AllowEdit = team.OwnerId == userId || isAdmin;
        }

        public static TeamDetails GetModelView(Team team, int userId, bool isAdmin)
        {
            if (team == null)
                return null;

            var teamDetails = new TeamDetails(team, userId, isAdmin);
            return teamDetails;
        }
    }

    public sealed class TeamDelete : BaseModelView
    {
        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        public TeamDelete(Team team)
        {
            Title = team.Title;
        }

        public static TeamDelete GetModelView(Team team)
        {
            if (team == null)
                return null;

            var teamDelete = new TeamDelete(team);
            return teamDelete;
        }
    }
}