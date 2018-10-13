using System;
using System.Collections.Generic;
using System.IO;
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
            if (strings.Any(string.IsNullOrEmpty))
                throw new ArgumentNullException();
        }

        public static void ThrowIfNullOrEmpty(IEnumerable<string> strings)
        {
            ThrowIfNull(strings);
            ThrowIfNullOrEmpty(strings.ToArray());
        }

        public static void ThrowIfFileNotFound(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Can not found the specified file path. ", path);
        }

        public static void ThrowIfFolderNotFount(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Can not found the specified path {path}. ");
        }

        public static void ThrowIfInvalidPath(string path)
        {
            if (!File.Exists(path) && !Directory.Exists(path))
                throw new DirectoryNotFoundException($"The specified path is not a valid file or directory.  ({path})");
        }
    }
}
