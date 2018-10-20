using Accelerider.Windows.TransferService;
using Accelerider.Windows.TransferService.WpfInteractions;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using static System.Console;
// ReSharper disable LocalizableElement

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
namespace Accelerider.Windows.Infrastructure.TransferService
{
    internal class Program
    {
        public static async Task Main()
        {
            var downloader = FileTransferService.GetDownloaderBuilder()
                .UseDefaultConfigure()
                .From("https://file.mrs4s.me/file/3898c738090be65fc336577605014534")
                //.From("https://accelerider-my.sharepoint.com/personal/cs02_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=b8a04e28-cbe7-46b6-a7e9-ff1dc364539e&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzOTg4NDEyMSIsImV4cCI6IjE1Mzk4ODc3MjEiLCJlbmRwb2ludHVybCI6ImZPVjloMFdhOFlLT3hNVVhOM0w4RDhySXBnVWVvYkt0ZTI1TVg2UUgrWkU9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6IlpEVmhOemxqT0RZdFpXSmxNUzAwWm1GaExUbGxNRGd0TTJVeE9EZGtaREExTVRNMiIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJaVGxpWXpsaVltSXROVFkyTWkwMFlqazNMVGd6TVdNdFl6ZzFNMkk1TkRobU0yTmkiLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDJAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM5QjEiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.SDl1TTZwMzFPalRwRHBJMVRMUEJnNkhxZDg0a3kzcENSTG90TDUxbFZpST0&ApiVersion=2.0")
                .To(@"C:\Users\Dingp\Desktop\DownloadTest\download-multi-thread.rmvb")
                .Build();

            WriteLine("Enter ant key to Start downloader: ");
            ReadKey(true);
            WriteLine("Try to Run... ");
            Subscribes(downloader.ToBindable());
            downloader.Run();

            await TimeSpan.FromSeconds(30);
            WriteLine("Try to Stop... ");
            downloader.Stop();

            var json = downloader.ToJson();

            var downloaderFromJson = FileTransferService.GetDownloaderBuilder()
                .UseDefaultConfigure()
                .Build(json);

            WriteLine("Enter ant key to Start downloader: ");
            ReadKey(true);
            WriteLine("Try to Run... ");
            Subscribes(downloaderFromJson.ToBindable());
            downloaderFromJson.Run();

            ReadKey();
        }

        public static void Subscribes(BindableDownloader bindableDownloader)
        {
            bindableDownloader.PropertyChanged += (sender, args) =>
            {
                var downloader = (BindableDownloader)sender;
                Console.WriteLine($"[{args.PropertyName}] : {downloader.Status}");
            };

            bindableDownloader.Progress.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != nameof(BindableProgress.CompletedSize)) return;

                var progress = (BindableProgress)sender;
                Console.WriteLine($"{progress.Speed}/s | " +
                                  $"{progress.RemainingTime:hh\\:mm\\:ss} | " +
                                  $"{progress.CompletedSize}/{progress.TotalSize}");
            };
        }
    }

    public static class Extensions
    {
        public static IDisposable SubscribeReport(this IDownloader @this)
        {
            const long oneM = 1024 * 1024;
            const long blockSize = 1024 * 1024 * 20;

            var previousDateTimeOffset = DateTimeOffset.Now;
            var previousCompletedSize = 0L;

            @this
                .Distinct(item => item.Status)
                .Timestamp()
                .Subscribe(item => WriteLine($"{item.Timestamp:HH:mm:ss}: Status is {item.Value.Status}"), error =>
                {
                    WriteLine($"ERROR: {error}");
                });

            return @this
                .Where(item => item.Status == TransferStatus.Transferring)
                .Sample(TimeSpan.FromMilliseconds(1000))
                .Timestamp()
                .Subscribe(item =>
                {
                    var timestamp = item.Timestamp;
                    var notification = item.Value;
                    var completedSize = @this.GetCompletedSize();

                    var message = $"{notification.CurrentBlockId:B}: " +
                                  $"{notification.Status} " +
                                  (notification.CurrentBlockId == Guid.Empty
                                      ? string.Empty
                                      : $"{@this.BlockContexts[notification.CurrentBlockId].Offset / blockSize:D3} --> {@this.BlockContexts[notification.CurrentBlockId].TotalSize / blockSize} "
                                  ) +
                                  $"{1.0 * (completedSize - previousCompletedSize) / (timestamp - previousDateTimeOffset).TotalSeconds / oneM:00.00} MB/s " +
                                  $"{100.0 * completedSize / @this.Context.TotalSize: 00.00}% " +
                                  $"{completedSize:D9}/{@this.Context.TotalSize}";

                    WriteLine(message);

                    previousCompletedSize = completedSize;
                    previousDateTimeOffset = timestamp;
                }, error =>
                {
                    WriteLine($"ERROR: {error}");
                }, () =>
                {
                    WriteLine($"======= Completed! Time: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss} =======");
                });
        }
    }
}
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
