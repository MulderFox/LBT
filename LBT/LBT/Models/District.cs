// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 12-20-2013
//
// Last Modified By : zmikeska
// Last Modified On : 01-18-2014
// ***********************************************************************
// <copyright file="District.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LBT.Models
{
    public class District
    {
        public int DistrictId { get; set; }

        [Required]
        [StringLength(128)]
        public string Title { get; set; }

        public virtual ICollection<UserProfile> UserProfiles { get; set; }

        public void CopyFrom(District district)
        {
            Title = district.Title;
        }
    }
}