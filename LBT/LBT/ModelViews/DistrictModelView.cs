using LBT.Models;
using LBT.Resources;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace LBT.ModelViews
{
    public class DistrictEdit : BaseModelView
    {
        public int DistrictId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Remote("IsTitleUnique", "District", AdditionalFields = "DistrictId", ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Unique_ErrorMesage")]
        [StringLength(128, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Title { get; set; }

        public DistrictEdit()
        {
            
        }

        public DistrictEdit(District district)
        {
            DistrictId = district.DistrictId;
            Title = district.Title;
        }

        public static DistrictEdit GetModelView(District district)
        {
            if (district == null)
                return null;

            var districtEdit = new DistrictEdit(district);
            return districtEdit;
        }

        public District GetModel()
        {
            var district = new District
                               {
                                   DistrictId = DistrictId,
                                   Title = Title
                               };
            return district;
        }
    }

    public class DistrictIndex : BaseModelView
    {
        public int DistrictId { get; set; }

        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Title { get; set; }

        public DistrictIndex(District district)
        {
            DistrictId = district.DistrictId;
            Title = district.Title;
        }

        public static DistrictIndex[] GetModelView(District[] districts)
        {
            if (districts == null)
                return null;

            DistrictIndex[] districtIndices = districts.Select(d => new DistrictIndex(d)).ToArray();
            return districtIndices;
        }
    }

    public class DistrictDelete : BaseModelView
    {
        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Title { get; set; }

        public DistrictDelete(District district)
        {
            Title = district.Title;
        }

        public static DistrictDelete GetModelView(District district)
        {
            if (district == null)
                return null;

            var districtDelete = new DistrictDelete(district);
            return districtDelete;
        }
    }

    public class DistrictDetails : BaseModelView
    {
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Title { get; set; }

        public DistrictDetails(District district)
        {
            Title = district.Title;
        }

        public static DistrictDetails GetModelView(District district)
        {
            if (district == null)
                return null;

            var districtDetails = new DistrictDetails(district);
            return districtDetails;
        }
    }
}