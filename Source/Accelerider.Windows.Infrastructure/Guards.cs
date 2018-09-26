using System;
using System.Linq;

namespace Accelerider.Windows.Infrastructure
{
    internal static class Guards
    {
        public static void ThrowIfNullReference(params object[] parameters)
        {
            if (parameters.Any(item => item == null))
                throw new ArgumentNullException();
        }
    }
}
