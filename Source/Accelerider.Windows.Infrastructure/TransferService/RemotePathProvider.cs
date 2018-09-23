using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public interface IRemotePathProvider
    {
        void Vote(string remotePath, double score);

        string GetRemotePath();
    }

    internal class RemotePathProvider : IRemotePathProvider
    {
        private readonly ConcurrentDictionary<string, double> _remotePathDictionary;

        public RemotePathProvider(IList<string> remotePaths)
        {
            if(remotePaths == null) throw new ArgumentNullException(nameof(remotePaths));
            if(!remotePaths.Any()) throw new ArgumentException();

            _remotePathDictionary = new ConcurrentDictionary<string, double>(remotePaths.ToDictionary(item => item, item => 0.0));
        }


        public void Vote(string remotePath, double score)
        {
            if (_remotePathDictionary.ContainsKey(remotePath))
                _remotePathDictionary[remotePath] += score;
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
