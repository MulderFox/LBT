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

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using LBT.ModelViews;
using LBT.Resources;

namespace LBT.Models
{
    /// <summary>
    /// Enum TopTenStatus
    /// </summary>
    public enum TopTenStatus
    {
        /// <summary>
        /// The A
        /// </summary>
        A,
        /// <summary>
        /// The B
        /// </summary>
        B,
        /// <summary>
        /// The C
        /// </summary>
        C
    }

    /// <summary>
    /// Enum TopTenRole
    /// </summary>
    public enum TopTenRole
    {
        /// <summary>
        /// The inviting
        /// </summary>
        Inviting = 0,
        /// <summary>
        /// The presenting
        /// </summary>
        Presenting = 1,
        /// <summary>
        /// The m1
        /// </summary>
        M1 = 2,
        /// <summary>
        /// The m2
        /// </summary>
        M2 = 3,
        /// <summary>
        /// The lyoness leader
        /// </summary>
        LyonessLeader = 4
    }

    public class TopTen
    {
        public int TopTenId { get; set; }

        [Column("FromUserId")]
        public virtual UserProfile FromUser { get; set; }

        [ForeignKey("FromUser")]
        public int FromUserId { get; set; }

        [Column("ToUserId")]
        public virtual UserProfile ToUser { get; set; }

        [ForeignKey("ToUser")]
        [Required]
        public int ToUserId { get; set; }

        [Required]
        public TopTenStatus Status { get; set; }

        public TopTenRole? Role { get; set; }

        [NotMapped]
        public string RoleLocalized
        {
            get
            {
                if (Role == null)
                    return BaseModelView.NullDisplayText;

                switch (Role.Value)
                {
                    default:
                        return ListItemsResource.TopTenRole_Inviting;

                    case TopTenRole.Presenting:
                        return ListItemsResource.TopTenRole_Presenting;

                    case TopTenRole.M1:
                        return ListItemsResource.TopTenRole_M1;

                    case TopTenRole.M2:
                        return ListItemsResource.TopTenRole_M2;

                    case TopTenRole.LyonessLeader:
                        return ListItemsResource.TopTenRole_LyonessLeader;
                }
            }
        }

        public string ShortTermCareerGoal { get; set; }

        public string LongTermCareerGoal { get; set; }

        public bool TrainingPhone { get; set; }

        public bool TrainingVideo { get; set; }

        public bool TrainingBiPresentation { get; set; }

        public bool TrainingStLecture { get; set; }

        public bool Od1 { get; set; }

        public bool Od2 { get; set; }

        public bool Rhetoric { get; set; }

        public bool TeamLeadershipping { get; set; }

        public bool ConductingMeetings { get; set; }

        public bool Sns { get; set; }

        public bool AdditionalTraining { get; set; }

        public int? ActualCareerStage { get; set; }

        [Required]
        public DateTime LastContact { get; set; }

        [AllowHtml]
        public string Note { get; set; }

        [AllowHtml]
        public string Tasks { get; set; }

        public void CopyFrom(TopTen topTen)
        {
            LastContact = topTen.LastContact;
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
            Tasks = topTen.Tasks;
            Note = topTen.Note;
            Status = topTen.Status;
        }
    }
}