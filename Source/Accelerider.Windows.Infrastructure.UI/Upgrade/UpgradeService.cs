using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public class UpgradeService : IUpgradeService
    {
        private const double UpgradeIntervalBaseMinute = 4 * 60;
        private const double RetryIntervalBaseMinute = 4;

        private readonly ConcurrentDictionary<string, IUpgradeTask> _tasks = new ConcurrentDictionary<string, IUpgradeTask>();

        private CancellationTokenSource _cancellationTokenSource;

        public void Add(IUpgradeTask task)
        {
            _tasks.TryAdd(task.Name, task);
        }

        public IUpgradeTask Get(string name) => _tasks.TryGetValue(name, out var result) ? result : default;

        public async void Run()
        {
            if (_cancellationTokenSource != null) return;

            _cancellationTokenSource = new CancellationTokenSource();
            await RunInternalAsync(_cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
        }

        private async Task RunInternalAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                bool requiredRetry = false;
                try
                {
                    var tasks = _tasks.Values.ToArray();
                    foreach (var task in tasks)
                    {
                        await task.TryLoadAsync();
                    }

                    var upgradeInfos = await MockGetUpgradeInfos();

                    foreach (var task in tasks)
                    {
                        var info = upgradeInfos.FirstOrDefault(
                            item => item.Name.Equals(task.Name, StringComparison.InvariantCultureIgnoreCase));

                        await task.DownloadAsync(info);
                    }
                }
                catch (Exception)
                {
                    requiredRetry = true;
                }

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(requiredRetry
                            ? RetryIntervalBaseMinute
                            : UpgradeIntervalBaseMinute),
                        token);
                }
                catch (TaskCanceledException)
                {
                    // Ignore
                }
            }
        }

        #region Mock Data

        // TODO: Replace the Mock.
        private static async Task<List<UpgradeInfo>> MockGetUpgradeInfos()
        {
            var request = (HttpWebRequest)WebRequest.Create("http://localhost:8000/list.json");
            using (var responseStream = request.GetResponse().GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                var json = await reader.ReadToEndAsync();
                return json.ToObject<List<UpgradeInfo>>();
            }
        }

        #endregion
    }

    public static class UpgradeServiceExtensions
    {
        public static void AddRange(this IUpgradeService @this, IEnumerable<IUpgradeTask> tasks)
        {
            tasks.ForEach(@this.Add);
        }
    }
}
