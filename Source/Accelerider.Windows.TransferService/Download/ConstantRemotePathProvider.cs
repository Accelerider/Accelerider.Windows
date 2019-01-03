using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Accelerider.Windows.TransferService
{
    public class ConstantRemotePathProvider : IRemotePathProvider
    {
        [JsonProperty]
        protected ConcurrentDictionary<string, double> RemotePaths;

        [JsonConstructor]
        private ConstantRemotePathProvider() { }

        public ConstantRemotePathProvider(HashSet<string> remotePaths)
        {
            if (remotePaths == null) throw new ArgumentNullException(nameof(remotePaths));

            RemotePaths = new ConcurrentDictionary<string, double>(
                remotePaths.ToDictionary(item => item, item => 0D));
        }

        public void Rate(string remotePath, double score)
        {
            if (RemotePaths.ContainsKey(remotePath))
                RemotePaths[remotePath] = RemotePaths[remotePath] + score;
        }

        public virtual Task<string> GetAsync()
        {
            if (RemotePaths.Values.All(item => item < 0))
                throw new RemotePathExhaustedException(this);

            return Task.FromResult(
                RemotePaths.Aggregate((acc, item) => acc.Value < item.Value ? item : acc).Key);
        }
    }
}
