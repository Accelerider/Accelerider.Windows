using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure
{
    public abstract class UploadTaskBuilderBase : TaskBuilderBase<IUploadTask, FileLocation, Uri>
    {
        protected readonly ICollection<FileLocation> FromPaths = new Collection<FileLocation>();
        protected Uri ToPath;


        public override ITaskBuilder<IUploadTask, FileLocation, Uri> From(FileLocation path)
        {
            FromPaths.Add(path);
            return this;
        }

        public override ITaskBuilder<IUploadTask, FileLocation, Uri> To(Uri path)
        {
            ToPath = path;
            return this;
        }
    }
}
