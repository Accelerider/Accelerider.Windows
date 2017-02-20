using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Core.DownloadEngine
{
    internal class DownloadTask : ModelBase
    {


        public DownloadTask(IUnityContainer container, INetDiskFile file, DownloadTaskToken token) : base(container)
        {

        }

        public void Start()
        {

        }
    }
}
