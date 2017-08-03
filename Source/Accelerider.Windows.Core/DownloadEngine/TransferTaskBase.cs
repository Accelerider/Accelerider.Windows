using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;

namespace Accelerider.Windows.Core.DownloadEngine
{
    internal abstract class TransferTaskBase
    {
        public event EventHandler<TransferStateChangedEventArgs> TransferStateChanged;

        public abstract Task<bool> TryCancelAsync();
        public abstract Task<bool> TryPauseAsync();
        public abstract Task<bool> TryRestartAsync();

        protected virtual void OnTransferStateChanged(TransferStateChangedEventArgs e)
        {
            TransferStateChanged?.Invoke(this, e);
        }
    }
}
