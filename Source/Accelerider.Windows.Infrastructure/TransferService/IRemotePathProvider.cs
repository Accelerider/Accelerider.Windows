﻿using System.Collections.Generic;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public interface IRemotePathProvider
    {
        IDictionary<string, double> RemotePaths { get; }

        void Vote(string remotePath, double score, bool isAccumulate = true);

        string GetRemotePath();
    }
}
