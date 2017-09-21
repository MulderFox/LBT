using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace LBT.Extensions
{
    // ReSharper disable InconsistentNaming
    public static class TEnumExtensions
    // ReSharper restore InconsistentNaming
    {
        public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var values = from TEnum e in Enum.GetValues(typeof (TEnum))
                         orderby e.ToString(CultureInfo.InvariantCulture)
                         select new { Id = e, Name = e.ToString(CultureInfo.InvariantCulture) };
            return new SelectList(values, "Id", "Name", enumObj);
        }

        public static SelectList ToSelectList<TEnum>(this TEnum enumObj, Dictionary<TEnum, string> translationDictionary)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                         orderby e.ToString(CultureInfo.InvariantCulture)
                         select new { Id = e, Name = translationDictionary[e] };
            return new SelectList(values, "Id", "Name", enumObj);
        }

        public static SelectList ToSelectList<TEnum>(Dictionary<TEnum, string> translationDictionary, object enumObj, params TEnum[] exceptEnumObj)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Except(exceptEnumObj)
                         orderby translationDictionary[e]
                         select new { Id = e, Name = translationDictionary[e] };
            
            return new SelectList(values, "Id", "Name", enumObj);
        }

        public static SelectList ToSelectList<TEnum>(object enumObj, params TEnum[] exceptEnumObj)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Except(exceptEnumObj)
                         orderby e.ToString(CultureInfo.InvariantCulture)
                         select new { Id = e, Name = e.ToString(CultureInfo.InvariantCulture) };

            return new SelectList(values, "Id", "Name", enumObj);
        }
    }
}