using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    internal class Program
    {
        public static async Task Main()
        {
            var downloader = FileTransferService.GetFileDownloaderBuilder()
                .UseDefaultConfigure()
                .Build();
            var disposable1 = downloader.SubscribeReport();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            downloader
                .From("https://accelerider-my.sharepoint.com/personal/cs02_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=b8a04e28-cbe7-46b6-a7e9-ff1dc364539e&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzODQzNTIzNiIsImV4cCI6IjE1Mzg0Mzg4MzYiLCJlbmRwb2ludHVybCI6ImZPVjloMFdhOFlLT3hNVVhOM0w4RDhySXBnVWVvYkt0ZTI1TVg2UUgrWkU9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6Ik9HRmpNVFF5TVRndFpqVXpOQzAwTVRRMkxXSTFOakl0WmpaallqaGhOalkyT1dSaiIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJaVGxpWXpsaVltSXROVFkyTWkwMFlqazNMVGd6TVdNdFl6ZzFNMkk1TkRobU0yTmkiLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDJAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM5QjEiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.OXlreXNmUG1neGdUUkp5RE9IbWx5SzJnMHErUUsxZmh5QTI3RE5qN1d6OD0&ApiVersion=2.0")
                .From("https://accelerider-my.sharepoint.com/personal/cs02_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=b8a04e28-cbe7-46b6-a7e9-ff1dc364539e&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzODQzNTI3MCIsImV4cCI6IjE1Mzg0Mzg4NzAiLCJlbmRwb2ludHVybCI6ImZPVjloMFdhOFlLT3hNVVhOM0w4RDhySXBnVWVvYkt0ZTI1TVg2UUgrWkU9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6Ik1tRXlNVGN4TkdZdFpqZ3dNQzAwTkdNNUxUaGhPRGt0TXpWaFl6TTNNak0yTlRZMCIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJaVGxpWXpsaVltSXROVFkyTWkwMFlqazNMVGd6TVdNdFl6ZzFNMkk1TkRobU0yTmkiLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDJAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM5QjEiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.aFgyRGFFVE5zRlNXUHNGblJpcWZqcW5nS0hTd09HOEpSSTNvK1RHODN5ST0&ApiVersion=2.0")
                .From("https://accelerider-my.sharepoint.com/personal/cs02_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=b8a04e28-cbe7-46b6-a7e9-ff1dc364539e&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzODQ0MjQ5NiIsImV4cCI6IjE1Mzg0NDYwOTYiLCJlbmRwb2ludHVybCI6ImZPVjloMFdhOFlLT3hNVVhOM0w4RDhySXBnVWVvYkt0ZTI1TVg2UUgrWkU9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6IlpqVXlNRFJrTm1JdE9ERm1OeTAwTURVd0xUZzFOalV0WXpFMllUSXdOVEkxT1dabSIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJaVGxpWXpsaVltSXROVFkyTWkwMFlqazNMVGd6TVdNdFl6ZzFNMkk1TkRobU0yTmkiLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDJAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM5QjEiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.MnBKcW95K09pVHN3aXZINEpoNk9SbWRJMVpIOXZBSlBQYUc0eHFiSXRQOD0&ApiVersion=2.0")
                .To(@"C:\Users\Dingp\Desktop\DownloadTest\download-multi-thread.rmvb");
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            WriteLine("Enter ant key to Start downloader: ");
            ReadKey(true);
            var cancellationTokenSource = new CancellationTokenSource();
            WriteLine("Try to ActivateAsync... ");
            await downloader.ActivateAsync(cancellationTokenSource.Token);

            await TimeSpan.FromSeconds(10);
            WriteLine("Try to Suspend... ");
            downloader.Suspend();
            var json = downloader.ToJson();
            WriteLine("Try to Dispose... ");
            downloader.Dispose();

            await TimeSpan.FromSeconds(5);

            var downloader2 = FileTransferService.GetFileDownloaderBuilder()
                .UseDefaultConfigure()
                .Build();

            downloader2.SubscribeReport();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            downloader2.FromJson(json);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            WriteLine("Try to ActivateAsync... ");
            await downloader2.ActivateAsync(cancellationTokenSource.Token);

            //FileTransferService
            //    .GetFileDownloaderBuilder()
            //    .UseDefaultConfigure()
            //    .Build()
            //    .FromJson(json);

            //await TimeSpan.FromMilliseconds(5000);
            //WriteLine("downloader has been disposed. ");
            //downloader.Dispose();

            ReadKey();
        }

        private static IObservable<(string flag, long number)> GenerateObservable(int flagNumber)
        {
            return Observable.Create<(string flag, long number)>(o =>
            {
                var source = new CancellationTokenSource();
                var token = source.Token;
                token.Register(() => WriteLine("Do something on cancelled. "));


                var flag = $"FLAG - {flagNumber}";
                Task.Run(async () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(1000));
                        if (token.IsCancellationRequested) break;
                        o.OnNext((flag, i));
                    }

                    o.OnCompleted();
                }, token);
                return () => source.Cancel();
            });
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
                .Where(item => item.Status != TransferStatus.Transferring)
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
