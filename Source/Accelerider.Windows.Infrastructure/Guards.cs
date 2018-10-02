using System;
using System.Collections.Generic;
using System.Linq;

namespace Accelerider.Windows.Infrastructure
{
    internal static class Guards
    {
        public static void ThrowIfNull(params object[] parameters)
        {
            if (parameters.Any(item => item == null))
                throw new ArgumentNullException();
        }

        public static void ThrowIfNullOrEmpty(params string[] strings)
        {
            if(strings.Any(string.IsNullOrEmpty))
                throw new ArgumentNullException();
        }

        public static void ThrowIfNullOrEmpty(IEnumerable<string> strings)
        {
            ThrowIfNull(strings);
            ThrowIfNullOrEmpty(strings.ToArray());
        }
    }
}
