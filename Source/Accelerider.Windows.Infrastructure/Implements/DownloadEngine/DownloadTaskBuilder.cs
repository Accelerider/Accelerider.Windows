using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure.Implements.DownloadEngine
{
    public class DownloadTaskBuilder : DownloadTaskBuilderBase
    {
        public override ITaskBuilder Clone()
        {
            var result = new DownloadTaskBuilder();
            //result.From(FromPaths.Select(v => v.AbsoluteUri)).To(ToPath.FullPath).Configure(Settings);
            return result;
        }

        public override IDownloadTask Build()
        {
            var result = new DownloadTask();
            result.Update(FromPaths, ToPath, Settings);
            return result;
        }

        public override IDownloadTask Update(IDownloadTask task)
        {
            if (task is DownloadTask downloadTask)
            {
                downloadTask.Update(FromPaths,ToPath,Settings);
                return downloadTask;
            }
            throw new ArgumentException("This task can not be update.");
        }
    }
}
