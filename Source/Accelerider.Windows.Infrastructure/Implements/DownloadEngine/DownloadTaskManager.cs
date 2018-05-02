using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Implements.DownloadEngine
{
    public class DownloadTaskManager
    {
        public static DownloadTaskManager Manager { get; } = new DownloadTaskManager();

        private DownloadTaskManager()
        {
        }
    }
}
