using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public class DownloaderManager
    {
        private readonly ConcurrentDictionary<Guid, IDownloader> _downloadingQueue = new ConcurrentDictionary<Guid, IDownloader>();
        private readonly ConcurrentDictionary<Guid, IDownloader> _pendingQueue = new ConcurrentDictionary<Guid, IDownloader>();
        private readonly ConcurrentDictionary<Guid, IDownloader> _suspendedQueue = new ConcurrentDictionary<Guid, IDownloader>();
        private readonly ConcurrentDictionary<Guid, IDownloader> _completedQueue = new ConcurrentDictionary<Guid, IDownloader>();



        public void Add(IDownloader downloader)
        {
            if (_pendingQueue.TryAdd(downloader.Context.Id, downloader))
            {
                Advance();
            }
        }

        public void Remove(IDownloader downloader)
        {
            
        }

        public void Remove(Guid id)
        {

        }

        public void AsNext(IDownloader downloader)
        {

        }

        public void AsNext(Guid downloader)
        {

        }

        private void Advance()
        {

        }
    }
}
