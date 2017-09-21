using System.Linq;
using LBT.Cache;
using LBT.Models;
using LBT.Resources;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace LBT.ModelViews
{
    public class PhoneNumberPrefixEdit : BaseModelView
    {
        public int PhoneNumberPrefixId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Remote("IsTitleUnique", "PhoneNumberPrefix", AdditionalFields = BaseCache.PhoneNumberPrefixIdField, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Unique_ErrorMesage")]
        [StringLength(40, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [StringLength(128, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "PhoneNumberPrefix_MatchRegex_Name", ResourceType = typeof(FieldResource))]
        public string MatchRegex { get; set; }

        [StringLength(128, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "PhoneNumberPrefix_ReplaceRegex_Name", ResourceType = typeof(FieldResource))]
        public string ReplaceRegex { get; set; }

        [StringLength(40, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "PhoneNumberPrefix_ExportablePrefix_Name", ResourceType = typeof(FieldResource))]
        public string ExportablePrefix { get; set; }

        public PhoneNumberPrefixEdit()
        {
            
        }

        public PhoneNumberPrefixEdit(PhoneNumberPrefix phoneNumberPrefix)
        {
            PhoneNumberPrefixId = phoneNumberPrefix.PhoneNumberPrefixId;
            Title = phoneNumberPrefix.Title;
            MatchRegex = phoneNumberPrefix.MatchRegex;
            ReplaceRegex = phoneNumberPrefix.ReplaceRegex;
            ExportablePrefix = phoneNumberPrefix.ExportablePrefix;
        }

        public static PhoneNumberPrefixEdit GetModelView(PhoneNumberPrefix phoneNumberPrefix)
        {
            if (phoneNumberPrefix == null)
                return null;

            var phoneNumberPrefixEdit = new PhoneNumberPrefixEdit(phoneNumberPrefix);
            return phoneNumberPrefixEdit;
        }

        public PhoneNumberPrefix GetModel()
        {
            var phoneNumberPrefix = new PhoneNumberPrefix
                                        {
                                            PhoneNumberPrefixId = PhoneNumberPrefixId,
                                            Title = Title,
                                            MatchRegex = MatchRegex,
                                            ReplaceRegex = ReplaceRegex,
                                            ExportablePrefix = ExportablePrefix,
                                        };
            return phoneNumberPrefix;
        }
    }

    public class PhoneNumberPrefixDetails : BaseModelView
    {
        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        public PhoneNumberPrefixDetails(PhoneNumberPrefix phoneNumberPrefix)
        {
            Title = phoneNumberPrefix.Title;
        }

        public static PhoneNumberPrefixDetails GetModelView(PhoneNumberPrefix phoneNumberPrefix)
        {
            if (phoneNumberPrefix == null)
                return null;

            var numberPrefixDetails = new PhoneNumberPrefixDetails(phoneNumberPrefix);
            return numberPrefixDetails;
        }
    }

    public class PhoneNumberPrefixIndex : BaseModelView
    {
        public int PhoneNumberPrefixId { get; set; }

        public string Title { get; set; }

        public PhoneNumberPrefixIndex(PhoneNumberPrefix phoneNumberPrefix)
        {
            PhoneNumberPrefixId = phoneNumberPrefix.PhoneNumberPrefixId;
            Title = phoneNumberPrefix.Title;
        }

        public static PhoneNumberPrefixIndex[] GetModelView(PhoneNumberPrefix[] phoneNumberPrefixes)
        {
            if (phoneNumberPrefixes == null)
                return null;

            PhoneNumberPrefixIndex[] phoneNumberPrefixIndices = phoneNumberPrefixes.Select(pnp => new PhoneNumberPrefixIndex(pnp)).ToArray();
            return phoneNumberPrefixIndices;
        }
    }
}