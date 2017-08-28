using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Core.Tools
{
    internal static class StringTools
    {
        public static string UrlEncode(this string value)
        {
            const int limit = 32760;
            var sb = new StringBuilder();
            var loops = value.Length / limit;
            for (var i = 0; i <= loops; i++)
            {
                sb.Append(i < loops
                    ? Uri.EscapeDataString(value.Substring(limit * i, limit))
                    : Uri.EscapeDataString(value.Substring(limit * i)));
            }
            return sb.ToString();
        }
    }
}
