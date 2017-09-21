// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-06-2014
//
// Last Modified By : zmikeska
// Last Modified On : 01-08-2014
// ***********************************************************************
// <copyright file="SharedContacts.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LBT.Models
{
    public class SharedContact
    {
        public int SharedContactId { get; set; }

        [ForeignKey("FromUser")]
        public int FromUserId { get; set; }

        [ForeignKey("ToUser")]
        [Required]
        public int ToUserId { get; set; }

        [Column("FromUserId")]
        public virtual UserProfile FromUser { get; set; }

        [Column("ToUserId")]
        public virtual UserProfile ToUser { get; set; }
    }
}