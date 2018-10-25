using System.Runtime.InteropServices;
using System.Security;

namespace System
{
    [SuppressUnmanagedCodeSecurity]
    public static class SecureStringExtensions
    {
        public static string AsString(this SecureString @this)
        {
            if (@this == null || @this.Length == 0)
                return string.Empty;

            IntPtr bstr = IntPtr.Zero;
            try
            {
                bstr = Marshal.SecureStringToBSTR(@this);
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                if (bstr != IntPtr.Zero)
                    Marshal.ZeroFreeBSTR(bstr);
            }
        }

        public static SecureString AsSecureString(this string @this)
        {
            if (string.IsNullOrEmpty(@this))
                return new SecureString();

            var secureString = new SecureString();
            Array.ForEach(@this.ToCharArray(), secureString.AppendChar);

            return secureString;
        }
    }
}
