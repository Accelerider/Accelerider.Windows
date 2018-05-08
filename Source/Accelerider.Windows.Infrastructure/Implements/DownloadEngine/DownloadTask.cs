using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Extensions;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json;

namespace Accelerider.Windows.Infrastructure.Implements.DownloadEngine
{
    public class DownloadTask : IDownloadTask
    {
        private TransportStatus _status = TransportStatus.Ready;

        public IEnumerable<Uri> Uris { get; private set; } = new Uri[0];

        public TransportSettings Setting { get; private set; }

        public List<DownloadBlock> DownloadBlocks { get; private set; } = new List<DownloadBlock>();

        public List<DownloadThread> DownloadThreads { get; } = new List<DownloadThread>();

        public event StatusChangedEventHandler StatusChanged;

        public bool IsDisposed { get; private set; }

        public TransportStatus Status
        {
            get => _status;
            private set
            {
                if (value == _status) return;

                var oldStatus = _status;
                _status = value;
                StatusChanged?.Invoke(this, new StatusChangedEventArgs(oldStatus, _status));
            }
        }

        public DataSize CompletedSize => Status == TransportStatus.Completed
            ? TotalSize.BaseBValue
            : DownloadBlocks?.Sum(v => v.DownloadedSize) ?? 0;

        public Dictionary<Uri, int> UriUseCount = new Dictionary<Uri, int>();

        public DataSize TotalSize { get; private set; }

        public FileLocation LocalPath { get; private set; }


        public void Update(IEnumerable<Uri> uris, FileLocation file, TransportSettings setting)
        {
            Uris = uris;
            LocalPath = file;
            Setting = setting;
        }

        public async Task StartAsync()
        {
            await Task.Run(() =>
            {
                if (!Initialize())
                    throw new IOException("Init blocks failed.");
                if (Status == TransportStatus.Transporting)
                    return;
                Status = TransportStatus.Transporting;
                if (DownloadBlocks.All(v => v.Completed))
                {
                    Status = TransportStatus.Completed;
                    return;
                }
                var response = GetResponse();
                if (response == null)
                {
                    Status = TransportStatus.Faulted;
                    return;
                }

                if (!File.Exists(LocalPath))
                {
                    using (var stream = new FileStream(LocalPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite, 1024 * 1024 * 5))
                        stream.SetLength(response.ContentLength);
                }

                TotalSize = response.ContentLength;
                DownloadThreads.Clear();
                var num = 0;
                for (var i = 0; i < Setting.ThreadCount; i++, num++)
                {
                    var block = DownloadBlocks.FirstOrDefault(v => !v.Completed && !v.Downloading);
                    if (block == null) return;
                    if (num == Uris.Count()) num = 0;
                    var thread = new DownloadThread(Uris.ToArray()[num], LocalPath, block, Setting);
                    thread.DownloadCompletedEvent += DownloadCompletedEvent;
                    thread.DownloadFailedEvent += DownloadFailedEvent;
                    thread.StartDownload();
                    DownloadThreads.Add(thread);
                }

            });


        }

        private void DownloadCompletedEvent(DownloadThread thread)
        {
            lock (this)
            {
                if (DownloadThreads.Count == 0) return;
                DownloadThreads.Remove(thread);
                if (DownloadBlocks.All(v => v.Completed))
                {
                    Status = TransportStatus.Completed;
                    return;
                }

                var nextBlock = DownloadBlocks.FirstOrDefault(v => !v.Completed && !v.Downloading);
                if (nextBlock == null) return;
                var newThread = new DownloadThread(Uris.First(), LocalPath, nextBlock, Setting);
                newThread.DownloadCompletedEvent += DownloadCompletedEvent;
                newThread.DownloadFailedEvent += DownloadFailedEvent;
                newThread.StartDownload();
                DownloadThreads.Add(newThread);
            }
        }

        private void DownloadFailedEvent(DownloadThread thread)
        {
            if (Status != TransportStatus.Faulted)
            {
                DownloadThreads?.ForEach(v => v.Stop());
                SaveBlock();
                if (Status != TransportStatus.Faulted)
                    Status = TransportStatus.Faulted;

            }
        }

        public async Task SuspendAsync()
        {
            await Task.Run(() =>
            {
                if (DownloadThreads != null && DownloadThreads.Count != 0)
                {
                    DownloadThreads.ForEach(v=>v.Stop());
                    SaveBlock();
                    Status = TransportStatus.Suspended;
                }
            });
        }

        public Task DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public bool Equals(ITransportTask other)
        {
            if (!(other is DownloadTask)) return false;
            return other.LocalPath.Equals(LocalPath) && other.Status == Status && other.TotalSize == TotalSize;
        }

        //Init blocks information
        private bool Initialize()
        {
            if (DownloadBlocks.Count > 0) return true;
            var blockFile = LocalPath + ".block";
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
            while (temp + Setting.BlockSize < contentLength)
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

        public void SaveBlock() => File.WriteAllText(LocalPath + ".block", JsonConvert.SerializeObject(DownloadBlocks));


        private HttpWebResponse GetResponse()
        {
            foreach (var uri in Uris)
            {
                try
                {
                    var request = WebRequest.CreateHttp(uri);
                    request.Headers = Setting.Headers.ToWebHeaderCollection();
                    request.Method = "GET";
                    return (HttpWebResponse)request.GetResponse();
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
        public Uri DownloadUri { get; set; }

        public FileLocation LocalFile { get; }

        public DownloadBlock Block { get; }

        public TransportSettings Settings { get; }

        public int ErrorCount { get; private set; }

        private bool _stoped;

        public DownloadThread(Uri uri, FileLocation toFile, DownloadBlock block, TransportSettings settings)
        {
            DownloadUri = uri;
            LocalFile = toFile;
            Block = block;
            Settings = settings;
        }

        public event Action<DownloadThread> DownloadCompletedEvent;
        public event Action<DownloadThread> DownloadFailedEvent;

        public void StartDownload()
        {
            Block.Downloading = true;
            new Thread(Start) { IsBackground = true }.Start();
        }

        private void Start()
        {
            HttpWebRequest request = null;
            try
            {
                if (_stoped) return;
                if (!File.Exists(LocalFile)) return;
                if (Block.Completed)
                {
                    Block.Downloading = false;
                    DownloadCompletedEvent?.Invoke(this);
                    return;
                }

                //Check range
                if (Block.BeginOffset > Block.EndOffset)
                {
                    Block.Completed = true;
                    Block.Downloading = false;
                    DownloadCompletedEvent?.Invoke(this);
                    return;
                }

                request = WebRequest.CreateHttp(DownloadUri);
                request.Headers = Settings.Headers.ToWebHeaderCollection();
                request.Method = "GET";
                request.Timeout = Settings.ConnectTimeout;
                request.ReadWriteTimeout = Settings.ReadWriteTimeout;
                request.AddRange(Block.BeginOffset, Block.EndOffset);
                var response = (HttpWebResponse)request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    using (var fileStream = new FileStream(LocalFile, FileMode.Open, FileAccess.ReadWrite,
                        FileShare.ReadWrite, 1024 * 1024))
                    {
                        fileStream.Seek(Block.BeginOffset, SeekOrigin.Begin);
                        var array = new byte[4096]; //4KB
                        var length = responseStream.Read(array, 0, array.Length);
                        while (true)
                        {
                            if (_stoped)
                            {
                                fileStream.Flush();
                                Block.Downloading = false;
                                return;
                            }

                            //Miss bytes
                            if (length <= 0 || Block.BeginOffset - 1 != Block.EndOffset)
                            {
                                new Thread(Start) { IsBackground = true }.Start();
                                return;
                            }

                            //Block download completed
                            if (length <= 0 || Block.BeginOffset > Block.EndOffset) break;

                            fileStream.Write(array, 0, length);
                            Block.BeginOffset += length;
                            Block.DownloadedSize += length;
                            length = responseStream.Read(array, 0, array.Length);
                        }

                        fileStream.Flush();
                    }
                }

                Block.Completed = true;
                Block.Downloading = false;
                DownloadCompletedEvent?.Invoke(this);

            }
            catch (WebException ex)
            {
                if (ex.Message.Contains("404"))
                {
                    //TODO: Download failed
                    return;
                }

                //Last block
                if (Block.BeginOffset == Block.EndOffset)
                {
                    Block.Completed = true;
                    Block.Downloading = false;
                    DownloadCompletedEvent?.Invoke(this);
                    return;
                }

                ErrorCount++;
                if (ErrorCount > Settings.MaxErrorCount)
                {
                    DownloadFailedEvent?.Invoke(this);
                    return;
                }
                new Thread(Start) { IsBackground = true }.Start();
            }
            catch (IOException ex)
            {
                //TODO: Check disk space and try again
            }
            catch (Exception)
            {
                ErrorCount++;
                if (ErrorCount > Settings.MaxErrorCount)
                {
                    DownloadFailedEvent?.Invoke(this);
                    return;
                }
                new Thread(Start) { IsBackground = true }.Start();
            }
            finally
            {
                try
                {
                    request?.Abort();
                }
                catch
                {
                }

            }
        }

        public void Stop()
        {
            if (Block.Completed)
                return;
            _stoped = true;
        }
    }
}
