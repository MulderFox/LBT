using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LBT.Extensions
{
    public static class ValidationExtensions
    {
        public static IHtmlString MyValidationSummary(this HtmlHelper htmlHelper)
        {
            var formContextForClientValidation = htmlHelper.ViewContext.ClientValidationEnabled ? htmlHelper.ViewContext.FormContext : null;
            if (htmlHelper.ViewData.ModelState.IsValid)
            {
                if (formContextForClientValidation == null)
                    return null;

                if (htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
                    return null;
            }

            var stringBuilder = new StringBuilder();
            var ulBuilder = new TagBuilder("ul");

            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix, out modelState))
            {
                foreach (ModelError error in modelState.Errors)
                {
                    string userErrorMessageOrDefault = error.ErrorMessage;
                    if (String.IsNullOrEmpty(userErrorMessageOrDefault))
                        continue;

                    var liBuilder = new TagBuilder("li") {InnerHtml = userErrorMessageOrDefault};
                    stringBuilder.AppendLine(liBuilder.ToString(TagRenderMode.Normal));
                }
            }

            if (stringBuilder.Length == 0)
            {
                stringBuilder.AppendLine("<li style=\"display:none\"></li>");
            }
            
            ulBuilder.InnerHtml = stringBuilder.ToString();

            var divBuilder = new TagBuilder("div");
            divBuilder.AddCssClass(htmlHelper.ViewData.ModelState.IsValid ? HtmlHelper.ValidationSummaryValidCssClassName : HtmlHelper.ValidationSummaryCssClassName);
            divBuilder.InnerHtml = ulBuilder.ToString(TagRenderMode.Normal);
            if (formContextForClientValidation != null)
            {
                if (!htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
                {
                    divBuilder.GenerateId("validationSummary");
                    formContextForClientValidation.ValidationSummaryId = divBuilder.Attributes["id"];
                    formContextForClientValidation.ReplaceValidationSummary = false;
                }
            }

            return new HtmlString(divBuilder.ToString(TagRenderMode.Normal));
        }
    }
}