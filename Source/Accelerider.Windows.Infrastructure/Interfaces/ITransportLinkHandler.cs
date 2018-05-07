using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Implements.DownloadEngine;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface ITransportLinkHandler
    {
        IEnumerable<string> GetLink(string info);

        void ErrorHandle(ITransportTask task, DownloadTaskManager manager);

    }

    [AttributeUsage(AttributeTargets.Class)]
    public class LinkHandlerAttribute : Attribute
    {
        public LinkHandlerAttribute(string type)
        {
            Type = type;
        }

        public string Type { get; }
    }
}
