using System;
using System.Linq;

namespace SmartlyDemo.RiotSPA.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        public static bool IsConvertibleToMonth(this string month)
        {
            if (String.IsNullOrEmpty(month)) return false;
            string[] longMonths = new string[] { "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" };

            return (longMonths.Contains(month.Trim().ToLower()));
        }
    }
}
