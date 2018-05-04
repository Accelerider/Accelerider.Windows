using System;
using System.Collections.Generic;
using System.Linq;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure
{
    public abstract class UploadTaskBuilderBase : TaskBuilderBase
    {
        protected FileLocation FromPath;
        protected readonly List<Uri> ToPaths = new List<Uri>();


        public override ITaskBuilder From(string path)
        {
            FromPath = path;
            return this;
        }

        public override ITaskBuilder From(IEnumerable<string> paths)
        {
            FromPath = paths.Last();
            return this;
        }

        public override ITaskBuilder To(string path)
        {
            ToPaths.Add(new Uri(path));
            return this;
        }

        public override ITaskBuilder To(IEnumerable<string> paths)
        {
            ToPaths.AddRange(paths.Select(item => new Uri(item)));
            return this;
        }
    }
}
