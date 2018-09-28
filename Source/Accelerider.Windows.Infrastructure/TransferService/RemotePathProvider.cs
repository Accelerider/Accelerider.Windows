using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public interface IRemotePathProvider
    {
        IDictionary<string, double> RemotePaths { get; }

        void Vote(string remotePath, double score, bool isAccumulate = true);

        string GetRemotePath();
    }

    internal class RemotePathProvider : IRemotePathProvider
    {
        [JsonProperty]
        public IDictionary<string, double> RemotePaths { get; private set; }

        [JsonConstructor]
        private RemotePathProvider() { }

        public RemotePathProvider(ICollection<string> remotePaths)
        {
            if (remotePaths == null) throw new ArgumentNullException(nameof(remotePaths));
            if (!remotePaths.Any()) throw new ArgumentException();

            RemotePaths = new ConcurrentDictionary<string, double>(remotePaths.ToDictionary(item => item, item => 0.0));
        }

        public void Vote(string remotePath, double score, bool isAccumulate = true)
        {
            if (RemotePaths.ContainsKey(remotePath))
                RemotePaths[remotePath] = isAccumulate ? RemotePaths[remotePath] + score : score;
        }

        public string GetRemotePath()
        {
            var theBestItem = RemotePaths.FirstOrDefault();
            foreach (var item in RemotePaths)
            {
                if (item.Value > theBestItem.Value) theBestItem = item;
            }

            return theBestItem.Key;
        }
    }
}
