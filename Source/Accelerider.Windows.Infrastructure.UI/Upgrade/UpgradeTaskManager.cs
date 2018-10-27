using Accelerider.Windows.Infrastructure.JsonObjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accelerider.Windows.TransferService;

namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public class UpgradeTaskManager
    {
        private const double UpgradeIntervalBasedMinute = 4 * 60;
        private const double RetryIntervalBasedMinute = 4;

        private readonly ConcurrentDictionary<string, IUpgradeTask> _tasks = new ConcurrentDictionary<string, IUpgradeTask>();

        private CancellationTokenSource _cancellationTokenSource;

        public void Add(IUpgradeTask task)
        {
            if (!_tasks.ContainsKey(task.Name)) _tasks[task.Name] = task;
        }

        public IUpgradeTask Find(string name) => _tasks.GetValueOrDefault(name);

        public Task<IDownloader> ExecuteAsync(IUpgradeTask task)
        {
            Add(task);
            return ExecuteAsync(task.Name);
        }

        public async Task<IDownloader> ExecuteAsync(string name)
        {
            return InternalExecuteAsync(_tasks.GetValueOrDefault(name), await GetAppMetadatasAsync());
        }

        public async void Run()
        {
            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
                _cancellationTokenSource = new CancellationTokenSource();

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                bool requiredRetry = false;
                try
                {
                    var appMetadatas = await GetAppMetadatasAsync();

                    foreach (var task in _tasks.Values)
                    {
                        await InternalExecuteAsync(task, appMetadatas);
                    }
                }
                catch (Exception)
                {
                    requiredRetry = true;
                }

                await Task.Delay(TimeSpan.FromMinutes(requiredRetry
                    ? RetryIntervalBasedMinute
                    : UpgradeIntervalBasedMinute),
                    _cancellationTokenSource.Token);
            }
        }

        public void Stop() => _cancellationTokenSource?.Cancel();

        private static IDownloader InternalExecuteAsync(IUpgradeTask task, IEnumerable<AppMetadata> appMetadatas)
        {
            var appMetadata = appMetadatas.FirstOrDefault(item => item.Name == task.Name);

            throw new NotImplementedException();
            //return appMetadata != null && task != null ? task.UpdateAsync(appMetadata) : null;
        }

        private Task<List<AppMetadata>> GetAppMetadatasAsync()
        {
            throw new NotImplementedException();
        }
    }
}
