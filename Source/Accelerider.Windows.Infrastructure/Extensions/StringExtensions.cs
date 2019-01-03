using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Accelerider.Windows.Infrastructure;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class StringExtensions
    {
        public static string RandomString(int length)
        {
            var b = new byte[4];
            new RNGCryptoServiceProvider().GetBytes(b);
            var r = new Random(BitConverter.ToInt32(b, 0));
            var ret = string.Empty;
            const string str = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (var i = 0; i < length; i++)
                ret += str.Substring(r.Next(0, str.Length - 1), 1);
            return ret;
        }

        public static string LogId => RandomString(48);

        public static string GetMatch(this string text, string p1, string p2)
        {
            var rg = new Regex("(?<=(" + p1 + "))[.\\s\\S]*?(?=(" + p2 + "))",
                RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(text).Value;
        }

        public static bool IsEmailAddress(this string @this)
        {
            return !string.IsNullOrEmpty(@this) && !string.IsNullOrWhiteSpace(@this); // TODO
        }

        public static string TrimMiddle(this string @this, int limitLength, string omitPlaceholder = null)
        {
            const string defaultOmitPlaceholder = "...";

            if (omitPlaceholder == null) omitPlaceholder = defaultOmitPlaceholder;
            Guards.ThrowIfNot(limitLength >= omitPlaceholder.Length);

            if (string.IsNullOrEmpty(@this) || @this.Length <= limitLength) return @this;

            if (limitLength == omitPlaceholder.Length) return omitPlaceholder;

            var halfLength = (limitLength - omitPlaceholder.Length) / 2;
            var bias = (limitLength - omitPlaceholder.Length) % 2;

            var firstHalfString = @this.Substring(0, halfLength + bias);
            var secondHalfString = @this.Substring(@this.Length - halfLength);

            return $"{firstHalfString}{omitPlaceholder}{secondHalfString}";
        }
    }
}
