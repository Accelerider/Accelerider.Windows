using System;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Reactive.Concurrency;
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
            var cancellationTokenSource = new CancellationTokenSource();

            var taskObservable = await FileTransferService2.CreateDownloadTaskAsync(
                //"https://accelerider-my.sharepoint.com/personal/cs04_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=5c4a2c63-e0ea-44f5-89d1-eb48c00f09b8&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzNzY2NzUxMiIsImV4cCI6IjE1Mzc2NzExMTIiLCJlbmRwb2ludHVybCI6IjcrVDJuM0tERDRvRXJSLzlvanRTRFREZ1Z4aXMrZjg2blVRQnV6bTgzT2s9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6Ill6Um1ObVV5TnpRdFlUTXpNQzAwTm1NM0xXSTVPVE10T0dabU1EUm1aRGsyTlRJeiIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJZekF6WlRrM01EZ3RaamhrTXkwME1EWmxMV0l6TnpNdE9EUmlNVEV6TW1GbU1UTXciLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDRAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM4OTAiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.MnoxU1U2ekM0RWdVNXdTUUlUNEtsSk5GTHpROE10SG9oSTFDaGZQeHhZZz0&ApiVersion=2.0",
                "https://accelerider-my.sharepoint.com/personal/cs02_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=b8a04e28-cbe7-46b6-a7e9-ff1dc364539e&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzNzY3MTY1OCIsImV4cCI6IjE1Mzc2NzUyNTgiLCJlbmRwb2ludHVybCI6ImZPVjloMFdhOFlLT3hNVVhOM0w4RDhySXBnVWVvYkt0ZTI1TVg2UUgrWkU9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6Ik1tSTFNRFk0T0RndE9UTTBNeTAwT0RJeUxXSTRNRGN0TXpkaU9UY3lZekZrWm1WbSIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJaVGxpWXpsaVltSXROVFkyTWkwMFlqazNMVGd6TVdNdFl6ZzFNMkk1TkRobU0yTmkiLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDJAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM5QjEiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.LzA3T1RQN0lwMS9UR2ZjQ0Rra0hIaTZYK0hqbG94c2hlNGNvdkdONURtdz0&ApiVersion=2.0",
                @"C:\Users\Dingp\Desktop\DownloadTest\download-multi-thread.rmvb",
                cancellationTokenSource.Token);

            const long OneM = 1024 * 1024;
            var previousDateTimeOffset = DateTimeOffset.Now;
            var previousCompletedSize = 0;

            var disposable1 = taskObservable
                .Sample(TimeSpan.FromMilliseconds(500))
                .Timestamp()
                .Subscribe(timestampedContext =>
                {
                    var timestamp = timestampedContext.Timestamp;
                    var context = timestampedContext.Value;
                    WriteLine($"{context.Id:B}: " +
                              $"{context.Status} " +
                              $"{1.0 * (context.CompletedSize - previousCompletedSize) / (timestamp - previousDateTimeOffset).TotalSeconds / OneM:00.00} Mb/s " +
                              $"{100.0 * context.CompletedSize / context.TotalSize: 00.00}% " +
                              $"{context.CompletedSize / OneM:00.000}M/{context.TotalSize / OneM:00.000}M ");
                }, () =>
                {
                    WriteLine($"======= Completed! Time: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss} =======");
                });

            var disposable2 = taskObservable
                .Sample(TimeSpan.FromMilliseconds(1000))
                .Timestamp()
                .Subscribe(timestampedContext =>
                {
                    var timestamp = timestampedContext.Timestamp;
                    var context = timestampedContext.Value;

                    WriteLine($"{context.Id:B}: " +
                              $"{timestamp:T}");
                });

            await Task.Delay(TimeSpan.FromMilliseconds(10000));
            disposable2.Dispose();


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
