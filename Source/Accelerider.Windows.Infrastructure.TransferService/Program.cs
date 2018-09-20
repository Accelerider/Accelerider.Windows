using System;
using System.IO.Pipes;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
using static Accelerider.Windows.Infrastructure.TransferService.FileTransferService;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    internal class Program
    {
        public static async Task Main()
        {
            await FileTransferService2.BlockDownloadAsync(
                //"https://accelerider-my.sharepoint.com/personal/cs04_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=e8ccde48-10a9-4e74-b2ed-b4e52cb8f68c&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzNzM2NjAwMiIsImV4cCI6IjE1MzczNjk2MDIiLCJlbmRwb2ludHVybCI6IjRMWTNhWjhzMnBJYlM5NUhpNEIxKzRTUTZVNHVydVBic1J1NG5tS0ErbUU9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6IlkyWTFaakk1T0RRdE9XVTBNaTAwTVRSakxXSTVZVFF0WVdJMFpETTRaRFptWW1WbCIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJZekF6WlRrM01EZ3RaamhrTXkwME1EWmxMV0l6TnpNdE9EUmlNVEV6TW1GbU1UTXciLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDRAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM4OTAiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.RG41aGd2dnhHZk5HYVNuWEhId0RBb0luL3FoVGtNMGxXT2tJNzBlekt0OD0&ApiVersion=2.0",
                "https://d.pcs.baidu.com/file/345bc5a471843bc537073e2c27808a42?fid=2872528644-250528-409815114683188&dstime=1537372912&rt=sh&sign=FDtAERVY-DCb740ccc5511e5e8fedcff06b081203-twq81yp%2BxTv7YirDuJ4E95imkGc%3D&expires=8h&chkv=1&chkbd=0&chkpc=et&dp-logid=311942304396355259&dp-callid=0&shareid=2073107565&r=556813142",
                @"C:\Users\Dingp\Desktop\DownloadTest\download-multi-thread.test",
                new CancellationTokenSource().Token);

            //DownloadSingleThread();

            ReadKey();
        }

        private static void DownloadSingleThread()
        {
            var getResponseStream = new Func<string, ITransferContext, HttpWebRequest>(CreateRequest)
                .Then(ConfigureRequestDefault)
                .Then(GetResponse)
                .Then(GetStream);

            var downloader = GetDownloader(getResponseStream, GetStream, new TransferContext());

            var downloadTask = downloader((
                //"https://accelerider-my.sharepoint.com/personal/cs04_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=e8ccde48-10a9-4e74-b2ed-b4e52cb8f68c&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzNzI4NjUzMyIsImV4cCI6IjE1MzcyOTAxMzMiLCJlbmRwb2ludHVybCI6IjRMWTNhWjhzMnBJYlM5NUhpNEIxKzRTUTZVNHVydVBic1J1NG5tS0ErbUU9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6IllURm1PVGd5WTJVdFpqa3lOQzAwTURKa0xXRTJPRFV0WXpBeU0yWXhPV1JqWkRKaiIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJZekF6WlRrM01EZ3RaamhrTXkwME1EWmxMV0l6TnpNdE9EUmlNVEV6TW1GbU1UTXciLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDRAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM4OTAiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.K3B2dWx6dkhQSm16TXBleGhyN3Zabi8rdnVSN2tvZ3NMMU5udGRESHl3Yz0&ApiVersion=2.0",
                "http://demo.jianguoyun.com/static/exe/installer/dotnetbrowser-1.16.zip",
                @"C:\Users\Dingp\Desktop\DownloadTest\123.test"));

            long previousCompletedSize = 0;
            DateTimeOffset previousDateTime = DateTimeOffset.Now;
            const double period = 1000;

            downloadTask
                .Activate()
                //.ObserveOn(NewThreadScheduler.Default)
                //.SubscribeOn(Scheduler.CurrentThread)
                .Sample(TimeSpan.FromMilliseconds(period))
                .Timestamp()
                .Subscribe(
                    timestampedContext =>
                    {
                        var timestamp = timestampedContext.Timestamp;
                        var context = timestampedContext.Value;

                        WriteLine($"Id: {Thread.CurrentThread.ManagedThreadId:00}, " +
                                  $"{((context.CompletedSize - previousCompletedSize) / (timestamp - previousDateTime).TotalSeconds) / (1024 * 1024): 00.000} Mb/s, " +
                                  $"{100.0 * context.CompletedSize / context.TotalSize: 00.00}%, " +
                                  $"Delta Time: {(timestamp - previousDateTime).TotalSeconds}");

                        previousCompletedSize = context.CompletedSize;
                        previousDateTime = timestamp;
                    },
                    () =>
                    {
                        WriteLine($"Id: {Thread.CurrentThread.ManagedThreadId:00}, " +
                                  $"======= Completed! Time: {DateTime.Now:U} =======");
                    }
                );
        }
    }
}
