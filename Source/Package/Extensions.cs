using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Package
{
    internal static class Extensions
    {
        private const string ConfigurationPlaceholder = "{Configuration}";
#if RELEASE
        private const string ConfigurationName = "Release";
#elif DEBUG
        private const string ConfigurationName = "Debug";
#endif
        private static readonly Regex VersionPlaceholderRegex = new Regex(@"\{Version:(.+?)\}", RegexOptions.Compiled);


        public static string ReplaceConfigurationPlaceholder(this string path)
        {
            return path.Replace(ConfigurationPlaceholder, ConfigurationName);
        }

        public static string ReplaceVersionPlaceholder(this string path, string sourcePath = null)
        {
            var match = VersionPlaceholderRegex.Match(path);

            return match.Success
                ? path.Replace(match.Value, AssemblyName
                    .GetAssemblyName(sourcePath == null
                        ? match.Groups[1].Value
                        : Path.Combine(sourcePath, match.Groups[1].Value))
                    .Version
                    .ToString(3))
                : path;
        }

        public static string GetFullPath(this string relativePath, string baseDirectory = null)
        {
            baseDirectory = baseDirectory ?? Environment.CurrentDirectory;
            return Path.GetFullPath(Path.Combine(baseDirectory, relativePath));
        }

        public static IEnumerable<FileElement> Flatten(this FolderElement @this)
        {
            yield return @this;

            foreach (var file in @this.Files)
            {
                yield return file;
            }

            foreach (var folder in @this.Folders)
            {
                foreach (var element in folder.Flatten())
                {
                    yield return element;
                }
            }
        }
    }
}
