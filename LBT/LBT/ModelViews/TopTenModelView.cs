// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 06-04-2014
//
// Last Modified By : zmikeska
// Last Modified On : 06-05-2014
// ***********************************************************************
// <copyright file="TopTen.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Web.Mvc;
using LBT.Cache;
using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LBT.Resources;

namespace LBT.ModelViews
{
    /// <summary>
    /// Class TopTenIndex
    /// </summary>
    public class TopTenIndex : BaseModelView
    {
        /// <summary>
        /// Gets or sets the top ten id.
        /// </summary>
        /// <value>The top ten id.</value>
        public int TopTenId { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public TopTenStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the last contact.
        /// </summary>
        /// <value>The last contact.</value>
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime LastContact { get; set; }

        /// <summary>
        /// Gets or sets the actual career stage.
        /// </summary>
        /// <value>The actual career stage.</value>
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public int? ActualCareerStage { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public TopTenRole? Role { get; set; }

        /// <summary>
        /// Gets or sets the lyoness id.
        /// </summary>
        /// <value>The lyoness id.</value>
        public string LyonessId { get; set; }

        /// <summary>
        /// Gets the formatting class.
        /// </summary>
        /// <value>The formatting class.</value>
        public string FormattingClass
        {
            get
            {
                return (DateTime.Now - LastContact).TotalDays > 10
                    ? OldLastContactClass : String.Empty;
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="TopTenIndex"/> class from being created.
        /// </summary>
        private TopTenIndex(TopTen topTen)
        {
            TopTenId = topTen.TopTenId;
            FirstName = topTen.ToUser.FirstName;
            LastName = topTen.ToUser.LastName;
            Status = topTen.Status;
            LastContact = topTen.LastContact;
            ActualCareerStage = topTen.ActualCareerStage;
            Role = topTen.Role;
            LyonessId = topTen.ToUser.LyonessId;
        }

        public static IEnumerable<TopTenIndex> GetIndex(DefaultContext db, int fromUserId, string searchString, string searchStringAccording, string sortOrder)
        {
            TopTenIndex[] topTenIndexList =
                (from topTen in TopTenCache.GetIndex(db, fromUserId, searchStringAccording, searchString, sortOrder)
                 select new TopTenIndex(topTen)).ToArray();

            return topTenIndexList;
        }
    }

    public class TopTenDetails : BaseModelView
    {
        public int TopTenId { get; set; }

        public UserProfileDetails ToUser { get; set; }

        [Display(Name = "TopTen_Status_Name", ResourceType = typeof(FieldResource))]
        public TopTenStatus Status { get; set; }

        [Display(Name = "TopTen_Role_Name", ResourceType = typeof(FieldResource))]
        public string RoleLocalized { get; set; }

        [Display(Name = "TopTen_ShorTermCareerGoal_Name", ResourceType = typeof(FieldResource))]
        public string ShortTermCareerGoal { get; set; }

        [Display(Name = "TopTen_LongTermCareerGoal_Name", ResourceType = typeof(FieldResource))]
        public string LongTermCareerGoal { get; set; }

        [Display(Name = "TopTen_TrainingPhone_Name", ResourceType = typeof(FieldResource))]
        public bool TrainingPhone { get; set; }

        [Display(Name = "TopTen_TrainingVideo_Name", ResourceType = typeof(FieldResource))]
        public bool TrainingVideo { get; set; }

        [Display(Name = "TopTen_TrainingBiPresentation_Name", ResourceType = typeof(FieldResource))]
        public bool TrainingBiPresentation { get; set; }

        [Display(Name = "TopTen_TrainingStLecture_Name", ResourceType = typeof(FieldResource))]
        public bool TrainingStLecture { get; set; }

        [Display(Name = "TopTen_Od1_Name", ResourceType = typeof(FieldResource))]
        public bool Od1 { get; set; }

        [Display(Name = "TopTen_Od2_Name", ResourceType = typeof(FieldResource))]
        public bool Od2 { get; set; }

        [Display(Name = "TopTen_Rhetoric_Name", ResourceType = typeof(FieldResource))]
        public bool Rhetoric { get; set; }

        [Display(Name = "TopTen_TeamLeadershipping_Name", ResourceType = typeof(FieldResource))]
        public bool TeamLeadershipping { get; set; }

        [Display(Name = "TopTen_ConductingMeetings_Name", ResourceType = typeof(FieldResource))]
        public bool ConductingMeetings { get; set; }

        [Display(Name = "TopTen_Sns_Name", ResourceType = typeof(FieldResource))]
        public bool Sns { get; set; }

        [Display(Name = "TopTen_AdditionalTraining_Name", ResourceType = typeof(FieldResource))]
        public bool AdditionalTraining { get; set; }

        [Display(Name = "TopTen_ActualCareerStage_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public int? ActualCareerStage { get; set; }

        [Display(Name = "TopTen_LastContact_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LastContact { get; set; }

        [AllowHtml]
        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Note { get; set; }

        [AllowHtml]
        [Display(Name = "Global_Tasks_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Tasks { get; set; }

        [Display(Name = "TopTen_ShorTermCareerGoal_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string ShortTermCareerGoalDetailView { get { return Grammar.CutTextWithDots(ShortTermCareerGoal, 93); } }

        [Display(Name = "TopTen_LongTermCareerGoal_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string LongTermCareerGoalDetailView { get { return Grammar.CutTextWithDots(LongTermCareerGoal, 93); } }

        private TopTenDetails(TopTen topTen)
        {
            TopTenId = topTen.TopTenId;
            ToUser = UserProfileDetails.GetModelView(topTen.ToUser);
            Status = topTen.Status;
            RoleLocalized = topTen.RoleLocalized;
            ShortTermCareerGoal = topTen.ShortTermCareerGoal;
            LongTermCareerGoal = topTen.LongTermCareerGoal;
            TrainingPhone = topTen.TrainingPhone;
            TrainingVideo = topTen.TrainingVideo;
            TrainingBiPresentation = topTen.TrainingBiPresentation;
            TrainingStLecture = topTen.TrainingStLecture;
            Od1 = topTen.Od1;
            Od2 = topTen.Od2;
            Rhetoric = topTen.Rhetoric;
            TeamLeadershipping = topTen.TeamLeadershipping;
            ConductingMeetings = topTen.ConductingMeetings;
            Sns = topTen.Sns;
            AdditionalTraining = topTen.AdditionalTraining;
            ActualCareerStage = topTen.ActualCareerStage;
            LastContact = topTen.LastContact;
            Note = topTen.Note;
            Tasks = topTen.Tasks;
        }

        public static TopTenDetails GetModelView(TopTen topTen)
        {
            if (topTen == null)
                return null;

            var topTenDetail = new TopTenDetails(topTen);
            return topTenDetail;
        }
    }

    public class TopTenDelete : BaseModelView
    {
        public UserProfileDetails ToUser { get; set; }

        public TopTenDelete(TopTen topTen)
        {
            ToUser = UserProfileDetails.GetModelView(topTen.ToUser);
        }

        public static TopTenDelete GetModelView(TopTen topTen)
        {
            if (topTen == null)
                return null;

            var topTenDelete = new TopTenDelete(topTen);
            return topTenDelete;
        }
    }

    public class TopTenEdit : BaseModelView
    {
        public int TopTenId { get; set; }

        public UserProfileDetails ToUser { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "TopTen_DownlineUser_Name", ResourceType = typeof(FieldResource))]
        public int ToUserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "TopTen_Status_Name", ResourceType = typeof(FieldResource))]
        public TopTenStatus Status { get; set; }

        [Display(Name = "TopTen_Role_Name", ResourceType = typeof(FieldResource))]
        public string RoleLocalized { get; set; }

        [Display(Name = "TopTen_ShorTermCareerGoal_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string ShortTermCareerGoal { get; set; }

        [Display(Name = "TopTen_LongTermCareerGoal_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string LongTermCareerGoal { get; set; }

        [Display(Name = "TopTen_TrainingPhone_Name", ResourceType = typeof(FieldResource))]
        public bool TrainingPhone { get; set; }

        [Display(Name = "TopTen_TrainingVideo_Name", ResourceType = typeof(FieldResource))]
        public bool TrainingVideo { get; set; }

        [Display(Name = "TopTen_TrainingBiPresentation_Name", ResourceType = typeof(FieldResource))]
        public bool TrainingBiPresentation { get; set; }

        [Display(Name = "TopTen_TrainingStLecture_Name", ResourceType = typeof(FieldResource))]
        public bool TrainingStLecture { get; set; }

        [Display(Name = "TopTen_Od1_Name", ResourceType = typeof(FieldResource))]
        public bool Od1 { get; set; }

        [Display(Name = "TopTen_Od2_Name", ResourceType = typeof(FieldResource))]
        public bool Od2 { get; set; }

        [Display(Name = "TopTen_Rhetoric_Name", ResourceType = typeof(FieldResource))]
        public bool Rhetoric { get; set; }

        [Display(Name = "TopTen_TeamLeadershipping_Name", ResourceType = typeof(FieldResource))]
        public bool TeamLeadershipping { get; set; }

        [Display(Name = "TopTen_ConductingMeetings_Name", ResourceType = typeof(FieldResource))]
        public bool ConductingMeetings { get; set; }

        [Display(Name = "TopTen_Sns_Name", ResourceType = typeof(FieldResource))]
        public bool Sns { get; set; }

        [Display(Name = "TopTen_AdditionalTraining_Name", ResourceType = typeof(FieldResource))]
        public bool AdditionalTraining { get; set; }

        [RegularExpression(Regex.OnlyNumberCharacters, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexOnlyNumbers_ErrorMessage")]
        [Range(1, 8, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexOnlyNumbersRange_ErrorMessage")]
        [Display(Name = "TopTen_ActualCareerStage_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public int? ActualCareerStage { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "TopTen_LastContact_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LastContact { get; set; }

        [AllowHtml]
        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Note { get; set; }

        [AllowHtml]
        [Display(Name = "Global_Tasks_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Tasks { get; set; }

        public TopTenEdit()
        {

        }

        public TopTenEdit(TopTen topTen)
        {
            TopTenId = topTen.TopTenId;
            ToUser = UserProfileDetails.GetModelView(topTen.ToUser);
            ToUserId = topTen.ToUserId;
            Status = topTen.Status;
            RoleLocalized = topTen.RoleLocalized;
            ShortTermCareerGoal = topTen.ShortTermCareerGoal;
            LongTermCareerGoal = topTen.LongTermCareerGoal;
            TrainingPhone = topTen.TrainingPhone;
            TrainingVideo = topTen.TrainingVideo;
            TrainingBiPresentation = topTen.TrainingBiPresentation;
            TrainingStLecture = topTen.TrainingStLecture;
            Od1 = topTen.Od1;
            Od2 = topTen.Od2;
            Rhetoric = topTen.Rhetoric;
            TeamLeadershipping = topTen.TeamLeadershipping;
            ConductingMeetings = topTen.ConductingMeetings;
            Sns = topTen.Sns;
            AdditionalTraining = topTen.AdditionalTraining;
            ActualCareerStage = topTen.ActualCareerStage;
            LastContact = topTen.LastContact;
            Note = topTen.Note;
            Tasks = topTen.Tasks;
        }

        public static TopTenEdit GetModelView(TopTen topTen)
        {
            if (topTen == null)
                return null;

            var topTenEdit = new TopTenEdit(topTen);
            return topTenEdit;
        }

        public TopTen GetModel()
        {
            var topTen = new TopTen
                             {
                                 TopTenId = TopTenId,
                                 ToUserId = ToUserId,
                                 LastContact = LastContact,
                                 ShortTermCareerGoal = ShortTermCareerGoal,
                                 LongTermCareerGoal = LongTermCareerGoal,
                                 TrainingPhone = TrainingPhone,
                                 TrainingVideo = TrainingVideo,
                                 TrainingBiPresentation = TrainingBiPresentation,
                                 TrainingStLecture = TrainingStLecture,
                                 Od1 = Od1,
                                 Od2 = Od2,
                                 Rhetoric = Rhetoric,
                                 TeamLeadershipping = TeamLeadershipping,
                                 ConductingMeetings = ConductingMeetings,
                                 Sns = Sns,
                                 AdditionalTraining = AdditionalTraining,
                                 ActualCareerStage = ActualCareerStage,
                                 Tasks = Tasks,
                                 Note = Note,
                                 Status = Status
                             };
            return topTen;
        }
    }

    public class TopTenExport : BaseCache
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "TopTen_Status_Name", ResourceType = typeof(FieldResource))]
        public TopTenStatus Status { get; set; }
    }
}