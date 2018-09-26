using System;
using System.Collections.Generic;
using Polly;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public class TransferSettings
    {
        public IAsyncPolicy<IEnumerable<IObservable<BlockTransferContext>>> BuildPolicy { get; set; }

        public int MaxConcurrent { get; set; }
    }
}
