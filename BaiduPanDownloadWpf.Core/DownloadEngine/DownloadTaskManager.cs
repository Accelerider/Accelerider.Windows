using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using BaiduPanDownloadWpf.Infrastructure.Events;
using BaiduPanDownloadWpf.Infrastructure;

namespace BaiduPanDownloadWpf.Core.DownloadEngine
{
    internal class DownloadTaskManager : ModelBase
    {
        public DownloadTaskManager(IUnityContainer container) : base(container)
        {
            EventAggregator.GetEvent<DownloadStateChangedEvent>().Subscribe(
                OnDownloadStateChanged, 
                Prism.Events.ThreadOption.BackgroundThread, 
                keepSubscriberReferenceAlive: false, 
                filter: e => e.NewState == (e.NewState & (DownloadStateEnum.Completed | DownloadStateEnum.Canceled | DownloadStateEnum.Faulted | DownloadStateEnum.Paused)));
        }

        public void Add(DownloadTask task)
        {
            task.Start();
            
        }

        public void AddRange(IEnumerable<DownloadTask> task)
        {

        }

        private void OnDownloadStateChanged(DownloadStateChangedEventArgs e)
        {

        }
    }
}
