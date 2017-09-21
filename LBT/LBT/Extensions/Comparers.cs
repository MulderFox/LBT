using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace LBT.Extensions
{
    internal sealed class NameComparer : IComparer<string>
    {
        private static readonly NameComparer DefaultInstance = new NameComparer();

        static NameComparer() { }
        private NameComparer() { }

        public static NameComparer CzechCaseInsensitive
        {
            get { return DefaultInstance; }
        }

        public int Compare(string value1, string value2)
        {
            CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
            var caseInsensitiveComparer = new CaseInsensitiveComparer(currentUICulture);

            int result = caseInsensitiveComparer.Compare(value1, value2);
            return result;
        }
    }
}