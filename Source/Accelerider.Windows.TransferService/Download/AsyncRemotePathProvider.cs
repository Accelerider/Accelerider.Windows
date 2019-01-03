using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerider.Windows.TransferService
{
    public class AsyncRemotePathProvider : ConstantRemotePathProvider
    {
        private readonly Func<Task<string>> _remotePathGetter;

        public AsyncRemotePathProvider(Func<Task<string>> remotePathGetter)
            : base(new HashSet<string>(Enumerable.Empty<string>()))
        {
            _remotePathGetter = remotePathGetter;
        }

        public override async Task<string> GetAsync()
        {
            var path = await _remotePathGetter();
            if (path == null) return await base.GetAsync();

            RemotePaths.TryAdd(path, 0);

            return path;
        }
    }
}
