using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public class FileDownloader
    {
        public Task<IObservable<TransferContext>> DownloadAsync(string remotePath, string localPath, CancellationToken cancellationToken)
        {
            return DownloadAsync(new List<string> { remotePath }, localPath, cancellationToken);
        }

        public Task<IObservable<TransferContext>> DownloadAsync(List<string> remotePaths, string localPath, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
