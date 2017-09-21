//// ***********************************************************************
//// Assembly         : LBT
//// Author           : zmikeska
//// Created          : 12-20-2013
////
//// Last Modified By : zmikeska
//// Last Modified On : 01-18-2014
//// ***********************************************************************
//// <copyright file="MSPContact.cs" company="Zdeněk Mikeska">
////     Copyright (c) Zdeněk Mikeska. All rights reserved.
//// </copyright>
//// <summary></summary>
//// ***********************************************************************

//using LBT.Filters;
//using LBT.Helpers;
//using LBT.ModelViews;
//using LBT.Resource;
//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using LBT.Resources;

//namespace LBT.Models
//{
//    /// <summary>
//    /// Class MSPContact
//    /// </summary>
//    [Table("MSPContact")]
//    public class MSPContact : Contact
//    {
//        /// <summary>
//        /// Gets or sets the MSP contact id.
//        /// </summary>
//        /// <value>The MSP contact id.</value>
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        // ReSharper disable InconsistentNaming
//        public int MSPContactId { get; set; }
//        // ReSharper restore InconsistentNaming

//        /// <summary>
//        /// Gets or sets the ICO.
//        /// </summary>
//        /// <value>The ICO.</value>
//        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
//        [Range(0, Int32.MaxValue, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexOnlyNumbers_ErrorMessage")]
//        //[Remote("IsIcoUnique", "MSPContact", AdditionalFields = "MSPContactId", ErrorMessage = ViewResource.UniqueErrorMessage)]
//        [StringLength(40, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
//        [Display(Name = OldViewResource.ICODisplayName)]
//        public string ICO { get; set; }

//        /// <summary>
//        /// Gets or sets the name of the company.
//        /// </summary>
//        /// <value>The name of the company.</value>
//        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
//        [StringLength(128, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
//        [Display(Name = OldViewResource.CompanyNameDisplayName)]
//        public string CompanyName { get; set; }

//        /// <summary>
//        /// Gets or sets the contact person.
//        /// </summary>
//        /// <value>The contact person.</value>
//        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
//        [StringLength(128, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
//        [Display(Name = OldViewResource.ContactPersonDisplayName)]
//        public string ContactPerson { get; set; }

//        /// <summary>
//        /// Gets or sets the address.
//        /// </summary>
//        /// <value>The address.</value>
//        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
//        [Display(Name = OldViewResource.AddressDisplayName)]
//        public string Address { get; set; }

//        /// <summary>
//        /// Gets or sets the phone number prefix1 id.
//        /// </summary>
//        /// <value>The phone number prefix1 id.</value>
//        [ForeignKey("PhoneNumberPrefix1")]
//        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
//        [Display(Name = OldViewResource.PhoneNumberPrefix1DisplayName)]
//        public int PhoneNumberPrefix1Id { get; set; }

//        /// <summary>
//        /// Gets or sets the phone number1.
//        /// </summary>
//        /// <value>The phone number1.</value>
//        [RegularExpression(Regex.OnlyNumberCharacters, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexOnlyNumbers_ErrorMessage")]
//        [StringLength(40, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
//        [Display(Name = "Global_PhoneNumber1_Name", ResourceType = typeof(FieldResource))]
//        [DisplayFormat(NullDisplayText = BaseModelView.NullDisplayText)]
//        public string PhoneNumber1 { get; set; }

//        /// <summary>
//        /// Gets or sets the business info location.
//        /// </summary>
//        /// <value>The business info location.</value>
//        [Display(Name = OldViewResource.BusinessInfoLocationDisplayName)]
//        [DisplayFormat(NullDisplayText = BaseModelView.NullDisplayText)]
//        public string BusinessInfoLocation { get; set; }

//        /// <summary>
//        /// Gets or sets the team meeting location.
//        /// </summary>
//        /// <value>The team meeting location.</value>
//        [Display(Name = OldViewResource.TeamMeetingLocationDisplayName)]
//        [DisplayFormat(NullDisplayText = BaseModelView.NullDisplayText)]
//        public string TeamMeetingLocation { get; set; }

//        /// <summary>
//        /// Gets or sets a value indicating whether [MSP active].
//        /// </summary>
//        /// <value><c>true</c> if [MSP active]; otherwise, <c>false</c>.</value>
//        [Display(Name = OldViewResource.MspActiveDisplayName)]
//        public bool MSPActive { get; set; }
//    }
//}