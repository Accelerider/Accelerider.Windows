using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure
{
    public abstract class DownloadTaskBuilderBase : TaskBuilderBase<IDownloadTask, Uri, FileLocation>
    {
        protected readonly ICollection<Uri> FromPaths = new Collection<Uri>();
        protected FileLocation ToPath;


        public override ITaskBuilder<IDownloadTask, Uri, FileLocation> From(Uri path)
        {
            FromPaths.Add(path);
            return this;
        }

        public override ITaskBuilder<IDownloadTask, Uri, FileLocation> To(FileLocation path)
        {
            ToPath = path;
            return this;
        }
    }
}
