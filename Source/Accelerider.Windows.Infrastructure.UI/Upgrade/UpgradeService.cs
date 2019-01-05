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
                    var upgradeInfos = await MockGetUpgradeInfos();

                    var tasks = _tasks.Values.ToArray();
                    foreach (var task in tasks)
                    {
                        var info = upgradeInfos.FirstOrDefault(
                            item => item.Name.Equals(task.Name, StringComparison.InvariantCultureIgnoreCase));

                        await task.ExecuteAsync(info);
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
        private static Task<HashSet<UpgradeInfo>> MockGetUpgradeInfos()
        {
            return Task.FromResult(new HashSet<UpgradeInfo>
            {
                new UpgradeInfo
                {
                    Name = "accelerider-shell-win",
                    Url = "https://file.mrs4s.me/file/3898c738090be65fc336577605014534",
                    Version = Version.Parse("0.0.1"),
                    ModuleType = "Accelerider Shell for Windows"
                },
                new UpgradeInfo
                {
                    Name = "apps-net-disk-win",
                    Url = "https://file.mrs4s.me/file/3898c738090be65fc336577605014534",
                    Version = Version.Parse("0.0.1"),
                    ModuleType = "Accelerider.Windows.Modules.NetDisk.NetDiskModule, Accelerider.Windows.Modules.NetDisk, Version=0.0.1.0, Culture=neutral, PublicKeyToken=null"
                }
            });
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
