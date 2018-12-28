using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Accelerider.Windows.TransferService
{
    public interface IPersistable<out T>
    {
        IPersister<T> GetPersister();
    }

    public interface IPersister<out T>
    {
        T Restore();
    }

    public class ConstantRemotePathProvider : IRemotePathProvider
    {
        protected ConcurrentDictionary<string, double> RemotePaths;

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

        public virtual IPersister<IRemotePathProvider> GetPersister()
        {
            return new Persister(this);
        }

        private class Persister : IPersister<IRemotePathProvider>
        {
            [JsonProperty]
            public ConcurrentDictionary<string, double> Data { get; private set; }

            [JsonConstructor]
            public Persister() { }

            public Persister(ConstantRemotePathProvider remotePathProvider)
            {
                Data = (ConcurrentDictionary<string, double>)remotePathProvider.RemotePaths;
            }

            IRemotePathProvider IPersister<IRemotePathProvider>.Restore()
            {
                return new ConstantRemotePathProvider { RemotePaths = Data };
            }
        }
    }
}
