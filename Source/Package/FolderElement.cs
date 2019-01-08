using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Package
{
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

        public static FolderElement Create(string xmlText)
        {
            var root = XElement.Parse(xmlText);

            return FolderElementPipe(new FolderElement(), root, null);
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

}
