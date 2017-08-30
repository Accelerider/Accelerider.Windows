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

        public static string GetSuperPath(this string path)
        {
            var list = path.Split('/');
            var tmp = new List<string>();
            for (var i = 0; i < list.Length - 1; i++)
                tmp.Add(list[i]);
            var superPath = string.Join("/", tmp);
            return string.IsNullOrEmpty(superPath) ? "/" : superPath;
        }

        public static string GetFileSizeString(this long size)
        {
            if (size < 1024) return size + "B";
            if (size < 1024 * 1024) return (size / 1024D).ToString("f2") + "KB";
            if (size < 1024 * 1024 * 1024) return (size / 1024D / 1024D).ToString("f2") + "MB";
            return (size / 1024D / 1024D / 1024D).ToString("f2") + "GB";
        }
    }
}
