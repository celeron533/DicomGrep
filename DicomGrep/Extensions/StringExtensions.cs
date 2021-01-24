using System;
using System.Collections.Generic;
using System.Text;

namespace DicomGrep.Extensions
{
    public static class StringExtensions
    {
        public static bool CaseInsensitiveContains(this string text, string value,
                            StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return text.IndexOf(value, stringComparison) >= 0;
        }
    }
}
