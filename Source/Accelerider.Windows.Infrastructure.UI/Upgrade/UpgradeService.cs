using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public class UpgradeService : IUpgradeService
    {
        private const double UpgradeIntervalBaseMinute = 4 * 60;
        private const double RetryIntervalBaseMinute = 4;

        private readonly Func<Task<List<UpgradeInfo>>> _upgradeInfosGetter;
        private readonly ConcurrentDictionary<string, IUpgradeTask> _tasks = new ConcurrentDictionary<string, IUpgradeTask>();

        private CancellationTokenSource _cancellationTokenSource;

        public UpgradeService(Func<Task<List<UpgradeInfo>>> upgradeInfosGetter)
        {
            Guards.ThrowIfNull(upgradeInfosGetter);

            _upgradeInfosGetter = upgradeInfosGetter;
        }

        void IUpgradeService.Add(IUpgradeTask task) => _tasks.TryAdd(task.Name, task);

        IUpgradeTask IUpgradeService.Get(string name) => _tasks.TryGetValue(name, out var result) ? result : default;

        async void IUpgradeService.Run()
        {
            if (_cancellationTokenSource != null) return;

            _cancellationTokenSource = new CancellationTokenSource();
            await RunInternalAsync(_cancellationTokenSource.Token);
        }

        void IUpgradeService.Stop()
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
                        await task.LoadFromLocalAsync();
                    }

                    var upgradeInfos = await _upgradeInfosGetter();

                    foreach (var task in tasks)
                    {
                        var info = upgradeInfos.FirstOrDefault(
                            item => item.Name.Equals(task.Name, StringComparison.InvariantCultureIgnoreCase));

                        if (info == null) continue;

                        await task.LoadFromRemoteAsync(info);
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
    }

    public static class UpgradeServiceExtensions
    {
        public static void AddRange(this IUpgradeService @this, IEnumerable<IUpgradeTask> tasks)
        {
            tasks.ForEach(@this.Add);
        }
    }
}
