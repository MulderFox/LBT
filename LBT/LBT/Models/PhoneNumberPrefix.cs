// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 12-20-2013
//
// Last Modified By : zmikeska
// Last Modified On : 01-18-2014
// ***********************************************************************
// <copyright file="PhoneNumberPrefix.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.ComponentModel.DataAnnotations;

namespace LBT.Models
{
    public class PhoneNumberPrefix
    {
        public int PhoneNumberPrefixId { get; set; }

        [Required]
        [StringLength(40)]
        public string Title { get; set; }

        [StringLength(128)]
        public string MatchRegex { get; set; }

        [StringLength(128)]
        public string ReplaceRegex { get; set; }

        [StringLength(40)]
        public string ExportablePrefix { get; set; }
    }
}