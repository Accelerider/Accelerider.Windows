using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Package
{
    [DebuggerDisplay("{" + nameof(Target) + "}")]
    public class FileElement
    {
        protected const string FileTag = "File";
        protected const string FolderTag = "Folder";

        public FolderElement Parent { get; set; }

        public string Target { get; set; }

        public virtual string Source { get; set; }

        protected FileElement() { }

        public static FileElement Create(XElement node, FolderElement parent)
        {
            return FileElementPipe(new FileElement(), node, parent);
        }

        protected static T FileElementPipe<T>(T element, XElement node, FolderElement parent) where T : FileElement
        {
            element.Parent = parent;
            element.Target = node.Attribute(nameof(Target))?.Value;
            element.Source = node.Attribute(nameof(Source))?.Value;

            if (string.IsNullOrWhiteSpace(element.Source))
            {
                element.Source = Path.Combine(parent.Source, element.Target ?? throw new InvalidOperationException());
            }

            element.Source = element.Source
                .ReplaceConfigurationPlaceholder()
                .ReplaceVersionPlaceholder()
                .GetFullPath(parent?.Source);

            element.Target = element.Target
                .ReplaceConfigurationPlaceholder()
                .ReplaceVersionPlaceholder(element.Source)
                .GetFullPath(parent?.Target);

            return element;
        }
    }

    public class FolderElement : FileElement
    {
        private string _sourcePath;

        public override string Source
        {
            get => _sourcePath ?? (_sourcePath = Parent?.Source);
            set => _sourcePath = value;
        }

        public List<FolderElement> Folders { get; set; }

        public List<FileElement> Files { get; set; }

        public new static FolderElement Create(XElement node, FolderElement parent)
        {
            return FolderElementPipe(new FolderElement(), node, parent);
        }

        protected static T FolderElementPipe<T>(T element, XElement node, FolderElement parent) where T : FolderElement
        {
            element = FileElementPipe(element, node, parent);

            if (!node.HasElements)
            {
                element.Source = Path.Combine(
                    parent.Source, 
                    Path.GetFileName(element.Target ?? throw new InvalidOperationException()));
            }

            element.Files = node
                .Elements(FileTag)
                .Select(item => FileElement.Create(item, element))
                .ToList();

            element.Folders = node
                .Elements(FolderTag)
                .Select(item => Create(item, element))
                .ToList();

            return element;
        }
    }

    public class PackageInfo : FolderElement
    {
        public static PackageInfo Parse(string xmlText)
        {
            var package = XElement.Parse(xmlText);

            return FolderElementPipe(new PackageInfo(), package, null);
        }
    }

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
