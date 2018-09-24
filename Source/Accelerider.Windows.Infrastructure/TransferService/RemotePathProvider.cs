using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public interface IRemotePathProvider
    {
        IReadOnlyList<string> RemotePaths { get; }

        void Vote(string remotePath, double score, bool isAccumulate = true);

        string GetRemotePath();
    }

    internal class RemotePathProvider : IRemotePathProvider
    {
        private readonly ConcurrentDictionary<string, double> _remotePathDictionary;

        public RemotePathProvider(List<string> remotePaths)
        {
            if (remotePaths == null) throw new ArgumentNullException(nameof(remotePaths));
            if (!remotePaths.Any()) throw new ArgumentException();

            RemotePaths = remotePaths.AsReadOnly();
            _remotePathDictionary = new ConcurrentDictionary<string, double>(remotePaths.ToDictionary(item => item, item => 0.0));
        }


        public IReadOnlyList<string> RemotePaths { get; }


        public void Vote(string remotePath, double score, bool isAccumulate = true)
        {
            if (_remotePathDictionary.ContainsKey(remotePath))
                _remotePathDictionary[remotePath] = isAccumulate ? _remotePathDictionary[remotePath] + score : score;
        }

        public string GetRemotePath()
        {
            var theBestItem = _remotePathDictionary.FirstOrDefault();
            foreach (var item in _remotePathDictionary)
            {
                if (item.Value > theBestItem.Value) theBestItem = item;
            }

            return theBestItem.Key;
        }
    }
}
