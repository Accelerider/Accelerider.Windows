using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Accelerider.Windows.Infrastructure.TransferService.WpfInteractions;
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
                .Build();
            //var disposable1 = downloader.SubscribeReport();

            downloader
                .From("https://file.mrs4s.me/file/3898c738090be65fc336577605014534")
                .To(@"C:\Users\Dingp\Desktop\DownloadTest\download-multi-thread.rmvb");

            WriteLine("Enter ant key to Start downloader: ");
            ReadKey(true);
            WriteLine("Try to ActivateAsync... ");

            Subscribe(downloader.ToBindable());

            downloader.Run();



            ReadKey();
        }

        public static void Subscribe(BindableDownloader bindableDownloader)
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
