using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Accelerider.Windows.Infrastructure.TransferService
{
    public interface IDownloader : ITransporter
    {
        DownloadContext Context { get; }

        /// <summary>
        /// Sets a uri sequence that represents the file source, it can be url or local disk path.
        /// </summary>
        /// <param name="paths">The uri sequence that represents the file source. </param>
        /// <returns>Returns the current instance. </returns>
        IDownloader From(IEnumerable<string> paths);
    }
}
