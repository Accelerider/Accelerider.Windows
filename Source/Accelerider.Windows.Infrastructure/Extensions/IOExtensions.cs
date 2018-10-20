using System.Linq;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace System.IO
{
    public static class ExtensionMethods
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

        public static bool CreateHardLinkTo(this string @this, string targetPath)
        {
            if (File.Exists(targetPath) || Directory.Exists(targetPath)) throw new IOException("Cannot create an existing file or directory. ");

            if (File.Exists(@this))
            {
                return CreateHardLink(targetPath, @this, IntPtr.Zero);
            }

            if (Directory.Exists(@this))
            {
                var entries = Directory.GetFileSystemEntries(@this);
                Directory.CreateDirectory(targetPath);

                // Recursion
                return entries.All(item => item.CreateHardLinkTo(Path.Combine(targetPath, Path.GetFileName(item))));
            }

            throw new FileNotFoundException();
        }
    }
}
