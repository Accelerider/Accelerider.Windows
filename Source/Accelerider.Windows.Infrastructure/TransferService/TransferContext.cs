using System;
using System.Net;
using System.Threading;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public class TransferContext : ITransferContext
    {
        public CancellationToken CancellationToken { get; set; }

        public TransferStatus Status { get; set; }

        public object this[string key]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public long CompletedSize { get; set; }

        public long TotalSize { get; set; }

        public string LocalPath { get; set; }

        public string RemotePath { get; set; }

        public HttpWebResponse Response { get; set; }

        public ITransferContext Clone()
        {
            var result = new TransferContext();
            CancellationToken = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, result.CancellationToken).Token;
            result.Status = Status;
            result.RemotePath = RemotePath;
            result.LocalPath = LocalPath;
            result.Response = Response;
            return result;
        }
    }
}
