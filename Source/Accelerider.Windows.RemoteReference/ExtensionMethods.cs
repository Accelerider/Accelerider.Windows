using System;
using System.IO;
using System.Linq;

namespace Accelerider.Windows.RemoteReference
{
    public static class ExtensionMethods
    {
        public static bool CreateHardLinkTo(this string @this, string targetPath)
        {
            if (File.Exists(targetPath) || Directory.Exists(targetPath)) throw new IOException("Cannot create an existing file or directory. ");

            if (File.Exists(@this))
            {
                return NativeMethods.CreateHardLink(targetPath, @this, IntPtr.Zero);
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
