using System;
using System.Collections.Generic;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure
{
    public abstract class DownloadTaskBuilderBase : TaskBuilderBase
    {
        protected readonly List<Uri> FromPaths = new List<Uri>();
        protected FileLocation ToPath;


        public override ITaskBuilder From(string path)
        {
            FromPaths.Add(new Uri(path));
            return this;
        }

        public override ITaskBuilder To(string path)
        {
            ToPath = path;
            return this;
        }
    }
}
