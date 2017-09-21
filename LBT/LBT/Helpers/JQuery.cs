using System.Threading;
using System.Web;

namespace LBT.Helpers
{
    public static class JQuery
    {
        /// <summary>
        /// Convert a .NET date format to jQuery
        /// </summary>
        public static string ConvertDateFormatToJQuery(string format)
        {
            // Date used in this comment : 5th - Nov - 2009 (Thursday)
            // 
            // .NET    JQueryUI        Output      Comment
            // --------------------------------------------------------------
            // d       d               5           day of month(No leading zero)
            // dd      dd              05          day of month(two digit)
            // ddd     D               Thu         day short name
            // dddd    DD              Thursday    day long name
            // M       m               11          month of year(No leading zero)
            // MM      mm              11          month of year(two digit)
            // MMM     M               Nov         month name short
            // MMMM    MM              November    month name long.
            // yy      y               09          Year(two digit)
            // yyyy    yy              2009        Year(four digit)

            string currentFormat = format;

            //Convert the date
            currentFormat = currentFormat.Replace("dddd", "DD");
            currentFormat = currentFormat.Replace("ddd", "D");

            //Convert month
            if (currentFormat.Contains("MMMM"))
            {
                currentFormat = currentFormat.Replace("MMMM", "MM");
            }
            else if (currentFormat.Contains("MMM"))
            {
                currentFormat = currentFormat.Replace("MMM", "M");
            }
            else if (currentFormat.Contains("MM"))
            {
                currentFormat = currentFormat.Replace("MM", "mm");
            }
            else
            {
                currentFormat = currentFormat.Replace("M", "m");
            }

            //Convert year
            currentFormat = currentFormat.Contains("yyyy") ? currentFormat.Replace("yyyy", "yy") : currentFormat.Replace("yy", "y");

            return currentFormat;
        }

        /// <summary>
        /// Returns script to init jQuery DatePicker with localization
        /// </summary>
        public static IHtmlString InitDatePicker()
        {
            //Using CSS class 'datepicker' to identify DatePicker controls

            string dateformat = ConvertDateFormatToJQuery(Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern);

            string html = @"<script type=""text/javascript"">
                                jQuery(function ($) {
                                    if ($.isFunction($('input.datepicker').datepicker)) {
                                        $('input.datepicker').datepicker({
                                            duration: '',
                                            changeMonth: false,
                                            changeYear: false,
                                            yearRange: '2010:2020',
                                            showTime: false,
                                            time24h: true,
                                            dateFormat: '" + dateformat + @"'
                                        });
                              
                                        $.datepicker.setDefaults($.datepicker.regional['']);  //Restore the default for Localization Fallback
                                        $.datepicker.setDefaults($.datepicker.regional['" + Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName + @"']);
                                    }
                                });
                            </script>";

            return new HtmlString(html);
        }
    }
}