using System;
using System.Data.Entity;
using LBT.Cache;
using LBT.DAL;
using LBT.Models;
using LBT.Resources;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace LBT.ModelViews
{
    public sealed class ManualTypeIndex : BaseModelView
    {
        public int ManualTypeId { get; set; }

        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Display(Name = "Global_Order_Name", ResourceType = typeof(FieldResource))]
        public int Order { get; set; }

        private ManualTypeIndex(ManualType manualType)
        {
            ManualTypeId = manualType.ManualTypeId;
            Title = manualType.Title;
            Order = manualType.Order;
        }

        public static ManualTypeIndex[] GetIndex(DefaultContext db)
        {
            ManualTypeIndex[] manualTypeIndices = ManualTypeCache.GetIndex(db).Select(mt => new ManualTypeIndex(mt)).ToArray();
            return manualTypeIndices;
        }
    }

    public class ManualTypeCreateEdit : BaseModelView
    {
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Required(ErrorMessageResourceType = typeof (ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Order_Name", ResourceType = typeof (FieldResource))]
        public int Order { get; set; }
        
        protected void ValidateUniqueTitle(DefaultContext db, ref ModelStateDictionary modelStateDictionary, int? modelTypeId = null)
        {
            bool isUnique = ManualTypeCache.IsTitleUnique(db, Title, modelTypeId);
            if (isUnique)
                return;

            string errorMessage = String.Format(ValidationResource.Global_OwnUnique_ErrorMessage, FieldResource.Global_Title_Name);
            modelStateDictionary.AddModelError(BaseCache.TitleField, errorMessage);
        }
    }

    public sealed class ManualTypeCreate : ManualTypeCreateEdit
    {
        public ManualType GetModel()
        {
            var manualType = new ManualType
                                 {
                                     Title = Title,
                                     Order = Order
                                 };
            return manualType;
        }

        public ModelStateDictionary Validate(DefaultContext db)
        {
            var modelStateDictionary = new ModelStateDictionary();

            ValidateUniqueTitle(db, ref modelStateDictionary);

            return modelStateDictionary;
        }
    }

    public sealed class ManualTypeEdit : ManualTypeCreateEdit
    {
        public int ManualTypeId { get; set; }

        public ManualTypeEdit()
        {

        }

        private ManualTypeEdit(ManualType manualType)
        {
            ManualTypeId = manualType.ManualTypeId;
            Title = manualType.Title;
            Order = manualType.Order;
        }

        public static ManualTypeEdit GetModelView(ManualType manualType)
        {
            if (manualType == null)
                return null;

            var manualTypeEdit = new ManualTypeEdit(manualType);
            return manualTypeEdit;
        }

        public ManualType GetModel()
        {
            var manualType = new ManualType
                                 {
                                     ManualTypeId = ManualTypeId,
                                     Title = Title,
                                     Order = Order
                                 };
            return manualType;
        }

        public ModelStateDictionary Validate(DefaultContext db)
        {
            var modelStateDictionary = new ModelStateDictionary();
            
            ValidateUniqueTitle(db, ref modelStateDictionary, ManualTypeId);

            return modelStateDictionary;
        }
    }

    public sealed class ManualTypeDelete : BaseModelView
    {
        public int ManualTypeId { get; set; }

        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        private ManualTypeDelete(ManualType manualType)
        {
            ManualTypeId = manualType.ManualTypeId;
            Title = manualType.Title;
        }

        public static ManualTypeDelete GetModelView(ManualType manualType)
        {
            if (manualType == null)
                return null;

            var manualTypeDelete = new ManualTypeDelete(manualType);
            return manualTypeDelete;
        }

        public ModelStateDictionary Validate(DefaultContext db)
        {
            var modelStateDictionary = new ModelStateDictionary();
            return modelStateDictionary;
        }
    }
}