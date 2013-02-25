using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace yamm
{
    public static class StringExtensions
    {
        public static string Join(this IEnumerable<string> values, string separator)
        {
            return string.Join( separator,values);
        }

        public static string[] Deconstruct(this string value)
        {
            var matches = Regex.Matches(value, "^([a-z])[^A-Z]*|([A-Z][^A-Z]*)");
            return matches.Cast<Match>().Select(x => x.Value).ToArray();
        }
    }
}
