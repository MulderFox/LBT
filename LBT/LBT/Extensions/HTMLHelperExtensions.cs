using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Xml.Linq;

namespace LBT.Extensions
{
    public static class HTMLHelperExtensions
    {
        public static MvcHtmlString CalenderTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            MvcHtmlString mvcHtmlString = System.Web.Mvc.Html.InputExtensions.TextBoxFor(htmlHelper, expression, htmlAttributes ?? new { @class = "text-box single-line datepicker" });
            XDocument xDoc = XDocument.Parse(mvcHtmlString.ToHtmlString());
            XElement xElement = xDoc.Element("input");
            if (xElement != null)
            {
                XAttribute valueAttribute = xElement.Attribute("value");
                if (valueAttribute != null && valueAttribute.Value != String.Empty)
                {
                    DateTime date;
                    if (DateTime.TryParse(valueAttribute.Value, out date))
                    {
                        valueAttribute.Value = date.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
                    }

                    DateTime defaultDate = default(DateTime);
                    if (valueAttribute.Value == defaultDate.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern))
                    {
                        valueAttribute.Value = String.Empty;                        
                    }
                }

                XAttribute xAttribute;
                valueAttribute = xElement.Attribute("class");
                if (valueAttribute == null)
                {
                    xAttribute = new XAttribute("class", "text-box single-line datepicker");
                    xElement.Add(xAttribute);
                }
                else if (!valueAttribute.Value.Contains("datepicker"))
                {
                    valueAttribute.Value = String.Format("{0} datepicker", valueAttribute.Value);
                }

                valueAttribute = xElement.Attribute("data-val");
                if (valueAttribute == null)
                {
                    xAttribute = new XAttribute("data-val", "false");
                    xElement.Add(xAttribute);
                }
                else
                {
                    valueAttribute.Value = "false";
                }
            }

            return new MvcHtmlString(xDoc.ToString());
        }

        public static MvcHtmlString MultiSelectBox(this HtmlHelper htmlHelper, string name)
        {
            return htmlHelper.MultiSelectBox(name, null, null);
        }

        public static MvcHtmlString MultiSelectBox(this HtmlHelper htmlHelper, string name, object htmlAttributes)
        {
            return htmlHelper.MultiSelectBox(name, null, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString MultiSelectBox(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullHtmlFieldName))
            {
                throw new ArgumentException("name");
            }

            bool flag = false;
            if (selectList == null)
            {
                selectList = htmlHelper.GetSelectData(name);
                flag = true;
            }
            object obj = null;
            if (!flag && !String.IsNullOrEmpty(name))
            {
                obj = htmlHelper.ViewData.Eval(name);
            }
            if (obj != null)
            {
                selectList = GetSelectListWithDefaultValue(selectList, obj);
            }

            var sectionTag = new TagBuilder("section");
            sectionTag.AddCssClass("multiSelectBox");

            if (htmlAttributes.ContainsKey("id"))
            {
                sectionTag.GenerateId(htmlAttributes["id"].ToString());
                htmlAttributes.Remove("id");
            }

            var divTag = new TagBuilder("div");

            string selectLeftTagName = String.Format("{0}_leftValues", fullHtmlFieldName);
            var selectLeftTag = new TagBuilder("select");
            selectLeftTag.GenerateId(selectLeftTagName);
            selectLeftTag.MergeAttribute("style", "width: 160px;");
            selectLeftTag.MergeAttribute("size", "5");
            selectLeftTag.MergeAttributes(htmlAttributes, true);
            selectLeftTag.MergeAttribute("multiple", "multiple", true);

            string selectRightTagName = fullHtmlFieldName;
            var selectRightTag = new TagBuilder("select");
            selectRightTag.GenerateId(selectRightTagName);
            selectRightTag.MergeAttribute("name", selectRightTagName);
            selectRightTag.MergeAttribute("style", "width: 160px;");
            selectRightTag.MergeAttribute("size", "5");
            selectRightTag.MergeAttributes(htmlAttributes, true);
            selectRightTag.MergeAttribute("multiple", "multiple", true);

            object selectedValue = ((SelectList)selectList).SelectedValue;
            if (selectedValue != null && !(selectedValue is IEnumerable<int>))
                throw new Exception("Please set selected values as IEnumerable<int>.");

            var selectedValues = (IEnumerable<int>)selectedValue;
            foreach (SelectListItem item in selectList)
            {
                var optionTag = new TagBuilder("option");
                optionTag.MergeAttribute("value", item.Value);
                optionTag.InnerHtml = item.Text;

                if (selectedValues != null && selectedValues.Contains(Convert.ToInt32(item.Value)))
                {
                    selectRightTag.InnerHtml += optionTag.ToString();
                }
                else
                {
                    selectLeftTag.InnerHtml += optionTag.ToString();
                }
            }

            divTag.InnerHtml += selectLeftTag.ToString();
            sectionTag.InnerHtml += divTag.ToString();

            divTag = new TagBuilder("div");

            var inputRightTag = new TagBuilder("input");
            inputRightTag.GenerateId(String.Format("{0}_btnRight", fullHtmlFieldName));
            inputRightTag.MergeAttribute("type", "button");
            inputRightTag.MergeAttribute("value", ">>");

            divTag.InnerHtml += inputRightTag.ToString();

            var inputLeftTag = new TagBuilder("input");
            inputLeftTag.GenerateId(String.Format("{0}_btnLeft", fullHtmlFieldName));
            inputLeftTag.MergeAttribute("type", "button");
            inputLeftTag.MergeAttribute("value", "<<");
            divTag.InnerHtml += inputLeftTag.ToString();
            sectionTag.InnerHtml += divTag.ToString();

            divTag = new TagBuilder("div");
            divTag.InnerHtml += selectRightTag.ToString();
            sectionTag.InnerHtml += divTag.ToString();

            htmlHelper.ViewBag.AdditionalScripts = String.Format(Properties.Resources.MultiSelectListBoxTemplate, fullHtmlFieldName);

            return new MvcHtmlString(sectionTag.ToString());
        }

        private static IEnumerable<SelectListItem> GetSelectData(this HtmlHelper htmlHelper, string name)
        {
            object obj = null;
            if (htmlHelper.ViewData != null)
            {
                obj = htmlHelper.ViewData.Eval(name);
            }
            if (obj == null)
            {
                throw new InvalidOperationException();
            }
            var enumerable = obj as IEnumerable<SelectListItem>;
            if (enumerable == null)
            {
                throw new InvalidOperationException();
            }
            return enumerable;
        }

        private static IEnumerable<SelectListItem> GetSelectListWithDefaultValue(IEnumerable<SelectListItem> selectList, object defaultValue)
        {
            IEnumerable enumerable = new[] { defaultValue };
            IEnumerable<string> collection =
                from object value in enumerable
                select Convert.ToString(value, CultureInfo.CurrentCulture);
            var hashSet = new HashSet<string>(collection, StringComparer.OrdinalIgnoreCase);
            var list = new List<SelectListItem>();
            foreach (SelectListItem current in selectList)
            {
                current.Selected = ((current.Value != null) ? hashSet.Contains(current.Value) : hashSet.Contains(current.Text));
                list.Add(current);
            }
            return list;
        }
    }
}