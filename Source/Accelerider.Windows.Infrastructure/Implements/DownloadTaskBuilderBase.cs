using System;
using System.Collections.Generic;
using System.Linq;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure
{
    public abstract class DownloadTaskBuilderBase : TaskBuilderBase, IDownloadTaskBuilder
    {
        protected readonly List<Uri> FromPaths = new List<Uri>();
        protected FileLocation ToPath;


        public override ITaskBuilder From(string path)
        {
            FromPaths.Add(new Uri(path));
            return this;
        }

        public override ITaskBuilder From(IEnumerable<string> paths)
        {
            FromPaths.AddRange(paths.Select(item => new Uri(item)));
            return this;
        }

        public override ITaskBuilder To(string path)
        {
            ToPath = path;
            return this;
        }

        public override ITaskBuilder To(IEnumerable<string> paths)
        {
            ToPath = paths.Last();
            return this;
        }

        public abstract IDownloadTask Build();

        public abstract IDownloadTask Update(IDownloadTask task);
    }
}
