using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure.Implements.DownloadEngine
{
    public class DownloadTaskManager
    {
        public static DownloadTaskManager Manager { get; } = new DownloadTaskManager();

        private readonly List<ITransportLinkHandler> _handlers = new List<ITransportLinkHandler>();

        public List<DownloadTask> TaskList { get; } = new List<DownloadTask>();


        private DownloadTaskManager()
        {
            //Manager thread
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(1000);
                    if (TaskList.Count(v => v.Status == TransportStatus.Transporting) < 5) //TODO: Use config option.
                    {
                        var task = TaskList.FirstOrDefault(v => v.Status == TransportStatus.Ready);
                        if (task != null)
                        {
                            
                        }
                    }
                }
            }).Start();
        }

        public void RegisterLinkHandler(ITransportLinkHandler handler)
        {
            if (handler.GetType().GetCustomAttributes(typeof(LinkHandlerAttribute)).FirstOrDefault() == null)
                throw new ArgumentException("Attribute 'LinkHandler' not found.");
            _handlers.Add(handler);
        }

        public void AddTask(DownloadTask task)
        {
            TaskList.Add(task);
        }





    }
}
