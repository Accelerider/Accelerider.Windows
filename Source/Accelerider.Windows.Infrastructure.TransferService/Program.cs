using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.FileTransferService;
using Polly;
using static System.Console;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    internal class Program
    {
        public static async Task Main()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var (taskObservable, totalContext) = await FileTransferService.DownloadAsync(
                new List<string>
                {
                    //"https://accelerider-my.sharepoint.com/personal/cs04_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=5c4a2c63-e0ea-44f5-89d1-eb48c00f09b8&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzNzY4NzExNyIsImV4cCI6IjE1Mzc2OTA3MTciLCJlbmRwb2ludHVybCI6IjcrVDJuM0tERDRvRXJSLzlvanRTRFREZ1Z4aXMrZjg2blVRQnV6bTgzT2s9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6IlpHVm1NRFprWkRFdE4yUXhNQzAwWmpaaUxUazRaR1l0T0dWaVptRTRPVFpqT0RRNSIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJZekF6WlRrM01EZ3RaamhrTXkwME1EWmxMV0l6TnpNdE9EUmlNVEV6TW1GbU1UTXciLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDRAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM4OTAiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.bmxxZVBMK3loaDNyVzk2VjI4RWo1UjEySWJBVFc1VGtaMjEzNXRjb1VZWT0&ApiVersion=2.0",
                    //"https://accelerider-my.sharepoint.com/personal/cs04_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=5c4a2c63-e0ea-44f5-89d1-eb48c00f09b8&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzNzY5NjU1NCIsImV4cCI6IjE1Mzc3MDAxNTQiLCJlbmRwb2ludHVybCI6IjcrVDJuM0tERDRvRXJSLzlvanRTRFREZ1Z4aXMrZjg2blVRQnV6bTgzT2s9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6Ik9USmxZbUkxT0dRdE56UTVOeTAwTTJGaUxXSXpOREl0WXpGa1ltRm1NbVU0WWpobSIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJZekF6WlRrM01EZ3RaamhrTXkwME1EWmxMV0l6TnpNdE9EUmlNVEV6TW1GbU1UTXciLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDRAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM4OTAiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.ckZiaFdYSnFUL0I1ancyUTZkNjJmK0VGUVVRSUhJNjVXQW9rN2dVMk5GTT0&ApiVersion=2.0",
                    "https://accelerider-my.sharepoint.com/personal/cs02_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=b8a04e28-cbe7-46b6-a7e9-ff1dc364539e&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzNzY5NzgxMiIsImV4cCI6IjE1Mzc3MDE0MTIiLCJlbmRwb2ludHVybCI6ImZPVjloMFdhOFlLT3hNVVhOM0w4RDhySXBnVWVvYkt0ZTI1TVg2UUgrWkU9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6Ill6TXpNV0ppTXpFdE9HVTVPUzAwTWpJeExUazROakF0T0dNNU1UVTBPR00xWmpreSIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJaVGxpWXpsaVltSXROVFkyTWkwMFlqazNMVGd6TVdNdFl6ZzFNMkk1TkRobU0yTmkiLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDJAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM5QjEiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.QlpzVXUrd1MybzFUd3JWa2pZOCtXZCtXYXJKZmpaREl2cFowR0owNzhxTT0&ApiVersion=2.0",
                    "https://accelerider-my.sharepoint.com/personal/cs02_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=b8a04e28-cbe7-46b6-a7e9-ff1dc364539e&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzNzcwMjIwOSIsImV4cCI6IjE1Mzc3MDU4MDkiLCJlbmRwb2ludHVybCI6ImZPVjloMFdhOFlLT3hNVVhOM0w4RDhySXBnVWVvYkt0ZTI1TVg2UUgrWkU9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6Ik1qQXlObUl4WlRJdE1URXlZaTAwTURFeUxUbGxORGd0TW1NM09UVmxZems0TnpJeSIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJaVGxpWXpsaVltSXROVFkyTWkwMFlqazNMVGd6TVdNdFl6ZzFNMkk1TkRobU0yTmkiLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDJAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM5QjEiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.L1BZOUNmREVFS0hTa0owU3JZbzU1WHZ1ZkJySjZETFZ4bVdtTjR6WERnWT0&ApiVersion=2.0",
                    "https://accelerider-my.sharepoint.com/personal/cs02_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=b8a04e28-cbe7-46b6-a7e9-ff1dc364539e&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzNzc5NjA4OCIsImV4cCI6IjE1Mzc3OTk2ODgiLCJlbmRwb2ludHVybCI6ImZPVjloMFdhOFlLT3hNVVhOM0w4RDhySXBnVWVvYkt0ZTI1TVg2UUgrWkU9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6Ik1ETTJaVE0yWTJNdE9HWXlNaTAwWTJSaUxXSTJPVEV0T0RNMk1URmlPVFJsTjJGaSIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJaVGxpWXpsaVltSXROVFkyTWkwMFlqazNMVGd6TVdNdFl6ZzFNMkk1TkRobU0yTmkiLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDJAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM5QjEiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.WXNSYVFCT0ttSU9RZTBCMFhKUjZLU1NWamY5RzBKMWJNcUtzbjVhbmViMD0&ApiVersion=2.0",
                    "https://accelerider-my.sharepoint.com/personal/cs02_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=b8a04e28-cbe7-46b6-a7e9-ff1dc364539e&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzNzgwMDI3MSIsImV4cCI6IjE1Mzc4MDM4NzEiLCJlbmRwb2ludHVybCI6ImZPVjloMFdhOFlLT3hNVVhOM0w4RDhySXBnVWVvYkt0ZTI1TVg2UUgrWkU9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6Ik1qa3hOR1ZsTkRBdFltWmxNQzAwT0RFeExUazBOVFl0TVRabFptUXlaVFEwTkRNdyIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJaVGxpWXpsaVltSXROVFkyTWkwMFlqazNMVGd6TVdNdFl6ZzFNMkk1TkRobU0yTmkiLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDJAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM5QjEiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.WXY3dHBZaGF6K3NMNVA3d2lUMFF3ZEgyckRCaDVzV2M1M0RyZkxhQytRQT0&ApiVersion=2.0",
                },
                @"C:\Users\Dingp\Desktop\DownloadTest\download-multi-thread.rmvb",
                cancellationTokenSource.Token,
                maxConcurrent: 16);

            const long OneM = 1024 * 1024;
            var previousDateTimeOffset = DateTimeOffset.Now;
            var previousCompletedSize = 0L;
            var totalCompleted = 0L;

            var disposable1 = taskObservable
                .Select(item =>
                {
                    var TotalCompleted = totalCompleted += item.Bytes;
                    return (item.Id, item.Status, TotalCompleted, item.Offset, item.TotalSize);
                })
                .Sample(TimeSpan.FromMilliseconds(500))
                .Timestamp()
                .Subscribe(timestampedContext =>
                {
                    var timestamp = timestampedContext.Timestamp;
                    var (Id, Status, TotalCompleted, Offset, TotalSize) = timestampedContext.Value;

                    WriteLine($"{Id:B}: " +
                              $"{Offset:D10} --> {Offset + TotalSize:D10} " +
                              $"{1.0 * (TotalCompleted - previousCompletedSize) / (timestamp - previousDateTimeOffset).TotalSeconds / OneM:00.00} Mb/s " +
                              $"{100.0 * TotalCompleted / totalContext.TotalSize: 00.00}% " +
                              $"{TotalCompleted:D9}/{totalContext.TotalSize}");

                    previousCompletedSize = TotalCompleted;
                    previousDateTimeOffset = timestamp;
                }, () =>
                {
                    WriteLine($"======= Completed! Time: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss} =======");
                });

            //var disposable2 = taskObservable
            //    .Sample(TimeSpan.FromMilliseconds(500))
            //    .Timestamp()
            //    .Subscribe(timestampedContext =>
            //    {
            //        var timestamp = timestampedContext.Timestamp;
            //        var context = timestampedContext.Value;

            //        WriteLine($"{context.Id:B}: " +
            //                  $"{timestamp:T}");
            //    });

            //await Task.Delay(TimeSpan.FromMilliseconds(10000));
            //disposable2.Dispose();


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
