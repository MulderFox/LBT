using LBT.ModelViews;
using System;

namespace LBT.Helpers
{
    public class Grammar
    {
        public static string CutTextWithDots(string text, int maximumTextLength)
        {
            string cuttedText = String.IsNullOrEmpty(text)
                                    ? BaseModelView.NullDisplayText
                                    : text.Length > maximumTextLength
                                          ? String.Format("{0}...", text.Substring(0, maximumTextLength - 3))
                                          : text;
            return cuttedText;
        }

        public static string ConvertDatetimeToText(DateTime? dateTime)
        {
            string text = !dateTime.HasValue ? String.Empty : dateTime.Value.ToString("dd.MM.yyyy");
            return text;
        }
    }
}