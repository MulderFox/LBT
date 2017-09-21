//// ***********************************************************************
//// Assembly         : LBT
//// Author           : zmikeska
//// Created          : 01-13-2014
////
//// Last Modified By : zmikeska
//// Last Modified On : 01-13-2014
//// ***********************************************************************
//// <copyright file="SharedMSPContacts.cs" company="Zdeněk Mikeska">
////     Copyright (c) Zdeněk Mikeska. All rights reserved.
//// </copyright>
//// <summary></summary>
//// ***********************************************************************

//using LBT.Filters;
//using LBT.Resource;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using LBT.Resources;

//namespace LBT.Models
//{
//    // ReSharper disable InconsistentNaming
//    /// <summary>
//    /// Class SharedMSPContact
//    /// </summary>
//    public class SharedMSPContact
//    // ReSharper restore InconsistentNaming
//    {
//        /// <summary>
//        /// Gets or sets the shared MSP contact id.
//        /// </summary>
//        /// <value>The shared MSP contact id.</value>
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int SharedMSPContactId { get; set; }

//        /// <summary>
//        /// Gets or sets from user id.
//        /// </summary>
//        /// <value>From user id.</value>
//        [ForeignKey("FromUser")]
//        public int FromUserId { get; set; }

//        /// <summary>
//        /// Gets or sets to user id.
//        /// </summary>
//        /// <value>To user id.</value>
//        [ForeignKey("ToUser")]
//        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
//        [Display(Name = "SharedContact_ToUserId_Name")]
//        public int ToUserId { get; set; }

//        /// <summary>
//        /// Gets or sets from user.
//        /// </summary>
//        /// <value>From user.</value>
//        [Column("FromUserId")]
//        public virtual UserProfile FromUser { get; set; }

//        /// <summary>
//        /// Gets or sets to user.
//        /// </summary>
//        /// <value>To user.</value>
//        [Column("ToUserId")]
//        public virtual UserProfile ToUser { get; set; }
//    }
//}