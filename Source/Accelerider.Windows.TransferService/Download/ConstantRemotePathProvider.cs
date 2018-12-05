using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Accelerider.Windows.TransferService
{
    internal class ConstantRemotePathProvider : IRemotePathProvider
    {
        [JsonProperty("remotePaths")]
        private readonly IDictionary<string, double> _remotePaths;

        [JsonConstructor]
        private ConstantRemotePathProvider() { }

        public ConstantRemotePathProvider(HashSet<string> remotePaths)
        {
            if (remotePaths == null) throw new ArgumentNullException(nameof(remotePaths));
            if (!remotePaths.Any()) throw new ArgumentException();

            _remotePaths = new ConcurrentDictionary<string, double>(remotePaths.ToDictionary(item => item, item => 0.0));
        }

        public void Score(string remotePath, double score)
        {
            if (_remotePaths.ContainsKey(remotePath))
                _remotePaths[remotePath] = _remotePaths[remotePath] + score;
        }

        public Task<string> GetRemotePathAsync()
        {
            if (_remotePaths.Values.All(item => item < 0))
                throw new RemotePathExhaustedException(this);

            var theBestItem = _remotePaths.FirstOrDefault();
            foreach (var item in _remotePaths)
            {
                if (item.Value > theBestItem.Value) theBestItem = item;
            }

            return Task.FromResult(theBestItem.Key);
        }
    }
}
