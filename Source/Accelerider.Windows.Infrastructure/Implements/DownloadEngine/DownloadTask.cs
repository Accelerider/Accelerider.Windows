using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Extensions;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json;

namespace Accelerider.Windows.Infrastructure.Implements.DownloadEngine
{
    public class DownloadTask : ITransportTask
    {
        public bool Equals(ITransportTask other)
        {
            if (!(other is DownloadTask)) return false;
            return other.LocalPath.Equals(LocalPath) && other.Status == Status && other.TotalSize == TotalSize;
        }




        public IEnumerable<Uri> Uris { get; private set; } = new Uri[0];
        public FileLocation ToFile { get; private set; }
        public TransportSettings Setting { get; private set; }
        public List<DownloadBlock> DownloadBlocks { get; private set; } = new List<DownloadBlock>();

        public void Update(IEnumerable<Uri> uris, FileLocation file, TransportSettings setting)
        {
            Uris = uris;
            ToFile = file;
            Setting = setting;
        }


        public event StatusChangedEventHandler StatusChanged;
        public bool IsCanceled { get; private set; }
        public TransportStatus Status { get; private set; } = TransportStatus.Ready;
        public DataSize CompletedSize { get; private set; }
        public DataSize TotalSize { get; private set; }
        public FileLocation LocalPath { get; private set; }

        public async Task StartAsync()
        {
            await Task.Run(() =>
            {
                if (!Init())
                    throw new IOException("Init blocks failed.");

            });


        }

        public Task SuspendAsync()
        {
            throw new NotImplementedException();
        }

        public Task RestartAsync()
        {
            throw new NotImplementedException();
        }

        public Task CancelAsync()
        {
            throw new NotImplementedException();
        }

        //Init blocks information
        private bool Init()
        {
            if (DownloadBlocks.Count > 0) return true;
            var blockFile = ToFile.FullPath + ".block";
            if (File.Exists(blockFile))
            {
                DownloadBlocks = JsonConvert.DeserializeObject<List<DownloadBlock>>(File.ReadAllText(blockFile));
                return true;
            }
            var response = GetResponse();
            if (response == null)
                return false;
            var contentLength = response.ContentLength;
            var temp = 0L;
            while (temp+Setting.BlockSize < contentLength)
            {
                DownloadBlocks.Add(new DownloadBlock()
                {
                    BeginOffset = temp,
                    EndOffset = temp + Setting.BlockSize - 1
                });
                temp += Setting.BlockSize;
            }
            DownloadBlocks.Add(new DownloadBlock()
            {
                BeginOffset = temp,
                EndOffset = contentLength
            });
            return true;
        }

        private HttpWebResponse GetResponse()
        {
            foreach (var uri in Uris)
            {
                try
                {
                    var request = WebRequest.CreateHttp(uri);
                    request.Headers = Setting.Headers.ToWebHeaderCollection();
                    request.Method = "GET";
                    return (HttpWebResponse) request.GetResponse();
                }
                catch
                {
                    //TODO: Error handle
                }
            }

            return null;
        }

    }

    public class DownloadBlock
    {
        public long BeginOffset { get; set; }

        public long EndOffset { get; set; }

        public long DownloadedSize { get; set; }

        public bool Completed { get; set; }

        [JsonIgnore]
        public bool Downloading { get; set; }

    }

    public class DownloadThread
    {
        public DownloadThread(string url, FileLocation toFile, DownloadBlock block, TransportSettings settings)
        {

        }
    }
}
