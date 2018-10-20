using System;

namespace Accelerider.Windows.TransferService
{
    public class RemotePathExhaustedException : Exception
    {
        public IRemotePathProvider Provider { get; }

        public RemotePathExhaustedException(IRemotePathProvider provider)
        {
            Provider = provider;
        }
    }
}
