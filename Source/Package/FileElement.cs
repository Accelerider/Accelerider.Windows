using System;
using System.Diagnostics;
using System.IO;
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
}
