using LBT.Resources;
using System;
using System.Web.Mvc;

namespace LBT.Helpers
{
    public class CustomModelBinderForDateTime : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value != null && !String.IsNullOrEmpty(value.AttemptedValue))
            {
                DateTime temp;
                if (!DateTime.TryParse(value.AttemptedValue, out temp))
                {
                    string fieldDisplayName = bindingContext.ModelMetadata != null
                                                  ? bindingContext.ModelMetadata.GetDisplayName()
                                                  : bindingContext.ModelName;
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, String.Format(ValidationResource.Validation_DateTime_ErrorMessage, fieldDisplayName));
                    bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);                    
                }

                return temp;
            }

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}