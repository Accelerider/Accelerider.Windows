using System;
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
                .From("https://accelerider-my.sharepoint.com/personal/cs02_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=b8a04e28-cbe7-46b6-a7e9-ff1dc364539e&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzNzcwMjIwOSIsImV4cCI6IjE1Mzc3MDU4MDkiLCJlbmRwb2ludHVybCI6ImZPVjloMFdhOFlLT3hNVVhOM0w4RDhySXBnVWVvYkt0ZTI1TVg2UUgrWkU9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6Ik1qQXlObUl4WlRJdE1URXlZaTAwTURFeUxUbGxORGd0TW1NM09UVmxZems0TnpJeSIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJaVGxpWXpsaVltSXROVFkyTWkwMFlqazNMVGd6TVdNdFl6ZzFNMkk1TkRobU0yTmkiLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDJAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM5QjEiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.L1BZOUNmREVFS0hTa0owU3JZbzU1WHZ1ZkJySjZETFZ4bVdtTjR6WERnWT0&ApiVersion=2.0")
                .From("https://accelerider-my.sharepoint.com/personal/cs02_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=b8a04e28-cbe7-46b6-a7e9-ff1dc364539e&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzNzc5NjA4OCIsImV4cCI6IjE1Mzc3OTk2ODgiLCJlbmRwb2ludHVybCI6ImZPVjloMFdhOFlLT3hNVVhOM0w4RDhySXBnVWVvYkt0ZTI1TVg2UUgrWkU9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6Ik1ETTJaVE0yWTJNdE9HWXlNaTAwWTJSaUxXSTJPVEV0T0RNMk1URmlPVFJsTjJGaSIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJaVGxpWXpsaVltSXROVFkyTWkwMFlqazNMVGd6TVdNdFl6ZzFNMkk1TkRobU0yTmkiLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDJAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM5QjEiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.WXNSYVFCT0ttSU9RZTBCMFhKUjZLU1NWamY5RzBKMWJNcUtzbjVhbmViMD0&ApiVersion=2.0")
                .From("https://accelerider-my.sharepoint.com/personal/cs02_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=b8a04e28-cbe7-46b6-a7e9-ff1dc364539e&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzNzgwMDI3MSIsImV4cCI6IjE1Mzc4MDM4NzEiLCJlbmRwb2ludHVybCI6ImZPVjloMFdhOFlLT3hNVVhOM0w4RDhySXBnVWVvYkt0ZTI1TVg2UUgrWkU9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6Ik1qa3hOR1ZsTkRBdFltWmxNQzAwT0RFeExUazBOVFl0TVRabFptUXlaVFEwTkRNdyIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJaVGxpWXpsaVltSXROVFkyTWkwMFlqazNMVGd6TVdNdFl6ZzFNMkk1TkRobU0yTmkiLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDJAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM5QjEiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.WXY3dHBZaGF6K3NMNVA3d2lUMFF3ZEgyckRCaDVzV2M1M0RyZkxhQytRQT0&ApiVersion=2.0")
                .To(@"C:\Users\Desktop\DownloadTest\新建文件夹\download-multi-thread.rmvb")
                .Build();

            var totalContext = downloader.Context;

            const long oneM = 1024 * 1024;
            var previousDateTimeOffset = DateTimeOffset.Now;
            var previousCompletedSize = 0L;
            var totalCompleted = 0L;

            var disposable1 = downloader
                .Select(item =>
                {
                    var TotalCompleted = totalCompleted += item.Bytes;
                    return (item.Id, /*item.Status,*/ TotalCompleted, item.Offset, item.TotalSize);
                })
                .Sample(TimeSpan.FromMilliseconds(500))
                .Timestamp()
                .Subscribe(timestampedContext =>
                {
                    var timestamp = timestampedContext.Timestamp;
                    var (Id, /*Status,*/ TotalCompleted, Offset, TotalSize) = timestampedContext.Value;

                    WriteLine($"{Id:B}: " +
                              $"{Offset:D10} --> {Offset + TotalSize:D10} " +
                              $"{1.0 * (TotalCompleted - previousCompletedSize) / (timestamp - previousDateTimeOffset).TotalSeconds / oneM:00.00} Mb/s " +
                              $"{100.0 * TotalCompleted / totalContext.TotalSize: 00.00}% " +
                              $"{TotalCompleted:D9}/{totalContext.TotalSize}");

                    previousCompletedSize = TotalCompleted;
                    previousDateTimeOffset = timestamp;
                }, () =>
                {
                    WriteLine($"======= Completed! Time: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss} =======");
                });

            var disposable2 = downloader
                .Sample(TimeSpan.FromMilliseconds(500))
                .Timestamp()
                .Subscribe(timestampedContext =>
                {
                    var timestamp = timestampedContext.Timestamp;
                    var context = timestampedContext.Value;

                    WriteLine($"{context.Id:B}: " +
                              $"{timestamp:T}");
                });

            WriteLine("enter ant key to Start downloader");
            ReadKey();
            var cancellationTokenSource = new CancellationTokenSource();
            await downloader.ActivateAsync(cancellationTokenSource.Token);

            await TimeSpan.FromMilliseconds(5000);
            WriteLine("downloader has been disposed. ");
            downloader.Dispose();

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
}
