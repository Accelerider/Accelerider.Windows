using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Accelerider.Windows.Infrastructure.Extensions;
using Newtonsoft.Json;

namespace Accelerider.Windows.Infrastructure.FileTransferService.Impls
{
    internal class DownloaderImpl : TransporterBaseImpl, IDownloader
    {
        private class DownloadThread
        {
            private readonly Uri _downloadUri;

            private readonly FileLocator _localFilePath;

            private readonly DownloadBlock _block;

            private readonly TransporterSettings _settings;

            private int _errorCount;

            private bool _stoped;

            public DownloadThread(Uri uri, FileLocator toFile, DownloadBlock block, TransporterSettings settings)
            {
                _downloadUri = uri;
                _localFilePath = toFile;
                _block = block;
                _settings = settings;
            }

            public event Action<DownloadThread> Completed;
            public event Action<DownloadThread> Failed;

            public void StartDownload()
            {
                _block.Downloading = true;
                new Thread(Start) { IsBackground = true }.Start();
            }

            private void Start()
            {
                HttpWebRequest request = null;
                try
                {
                    if (_stoped) return;
                    if (!File.Exists(_localFilePath)) return;
                    if (_block.Completed)
                    {
                        _block.Downloading = false;
                        Completed?.Invoke(this);
                        return;
                    }

                    //Check range
                    if (_block.BeginOffset > _block.EndOffset)
                    {
                        _block.Completed = true;
                        _block.Downloading = false;
                        Completed?.Invoke(this);
                        return;
                    }

                    request = WebRequest.CreateHttp(_downloadUri);
                    request.Headers = _settings.Headers.ToWebHeaderCollection();
                    request.Method = "GET";
                    request.Timeout = _settings.ConnectTimeout;
                    request.ReadWriteTimeout = _settings.ReadWriteTimeout;
                    request.AddRange(_block.BeginOffset, _block.EndOffset);
                    var response = (HttpWebResponse)request.GetResponse();
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var fileStream = new FileStream(_localFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, DataSize.OneMB))
                        {
                            fileStream.Seek(_block.BeginOffset, SeekOrigin.Begin);
                            var array = new byte[4 * DataSize.OneKB]; //4KB

                            Debug.Assert(responseStream != null, nameof(responseStream) + " != null");
                            var length = responseStream.Read(array, 0, array.Length);
                            while (true)
                            {
                                if (_stoped)
                                {
                                    fileStream.Flush();
                                    _block.Downloading = false;
                                    return;
                                }

                                //Miss bytes
                                if (length <= 0 || _block.BeginOffset - 1 != _block.EndOffset)
                                {
                                    new Thread(Start) { IsBackground = true }.Start();
                                    return;
                                }

                                //Block download completed
                                if (length <= 0 || _block.BeginOffset > _block.EndOffset) break;

                                fileStream.Write(array, 0, length);
                                _block.BeginOffset += length;
                                _block.DownloadedSize += length;
                                length = responseStream.Read(array, 0, array.Length);
                            }

                            fileStream.Flush();
                        }
                    }

                    _block.Completed = true;
                    _block.Downloading = false;
                    Completed?.Invoke(this);

                }
                catch (WebException ex)
                {
                    if (ex.Message.Contains("404"))
                    {
                        //TODO: Download failed
                        return;
                    }

                    //Last block
                    if (_block.BeginOffset == _block.EndOffset)
                    {
                        _block.Completed = true;
                        _block.Downloading = false;
                        Completed?.Invoke(this);
                        return;
                    }

                    _errorCount++;
                    if (_errorCount > _settings.MaxErrorCount)
                    {
                        Failed?.Invoke(this);
                        return;
                    }
                    new Thread(Start) { IsBackground = true }.Start();
                }
                catch (IOException)
                {
                    //TODO: Check disk space and try again
                }
                catch (Exception)
                {
                    _errorCount++;
                    if (_errorCount > _settings.MaxErrorCount)
                    {
                        Failed?.Invoke(this);
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
                        // ignored
                    }
                }
            }

            public void Stop()
            {
                if (_block.Completed)
                    return;
                _stoped = true;
            }
        }

        private class DownloadBlock
        {
            public long BeginOffset { get; set; }

            public long EndOffset { get; set; }

            public long DownloadedSize { get; set; }

            public bool Completed { get; set; }

            [JsonIgnore]
            public bool Downloading { get; set; }
        }


        private readonly List<DownloadThread> _downloadThreads = new List<DownloadThread>();

        private List<DownloadBlock> _downloadBlocks = new List<DownloadBlock>();

        public IEnumerable<Uri> Uris { get; private set; }

        public TransporterSettings Setting { get; private set; }

        public override DataSize CompletedSize => Status == TransferStatus.Completed
            ? TotalSize.BaseBValue
            : _downloadBlocks?.Sum(v => v.DownloadedSize) ?? 0;


        public void Update(IEnumerable<Uri> uris, FileLocator file, TransporterSettings setting)
        {
            Uris = uris;
            LocalPath = file;
            Setting = setting;
        }

        public override void Start()
        {
            if (!Initialize())
                throw new IOException("Init blocks failed.");
            if (Status == TransferStatus.Transferring)
                return;
            Status = TransferStatus.Transferring;
            if (_downloadBlocks.All(v => v.Completed))
            {
                Status = TransferStatus.Completed;
                return;
            }
            var response = GetResponse();
            if (response == null)
            {
                Status = TransferStatus.Faulted;
                return;
            }

            if (!File.Exists(LocalPath))
            {
                using (var stream = new FileStream(LocalPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite, 5 * DataSize.OneMB))
                    stream.SetLength(response.ContentLength);
            }

            TotalSize = response.ContentLength;
            _downloadThreads.Clear();
            var num = 0;
            for (var i = 0; i < Setting.ThreadCount; i++, num++)
            {
                var block = _downloadBlocks.FirstOrDefault(v => !v.Completed && !v.Downloading);
                if (block == null) return;
                if (num == Uris.Count()) num = 0;
                var thread = new DownloadThread(Uris.ToArray()[num], LocalPath, block, Setting);
                thread.Completed += OnDownloadCompleted;
                thread.Failed += OnDownloadFailed;
                thread.StartDownload();
                _downloadThreads.Add(thread);
            }
        }

        public override void Suspend()
        {
            if (_downloadThreads != null && _downloadThreads.Count != 0)
            {
                _downloadThreads.ForEach(v => v.Stop());
                SaveBlock();
                Status = TransferStatus.Suspended;
            }
        }

        public void SaveBlock() => File.WriteAllText(LocalPath + ".block", JsonConvert.SerializeObject(_downloadBlocks));


        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Free any other managed objects here.

            }

            // Free any unmanaged objects here.


            _disposed = true;
        }


        //Init blocks information
        private bool Initialize()
        {
            if (_downloadBlocks.Count > 0) return true;
            var blockFile = LocalPath + ".block";
            if (File.Exists(blockFile))
            {
                _downloadBlocks = JsonConvert.DeserializeObject<List<DownloadBlock>>(File.ReadAllText(blockFile));
                return true;
            }
            var response = GetResponse();
            if (response == null)
                return false;
            var contentLength = response.ContentLength;
            var temp = 0L;
            while (temp + Setting.BlockSize < contentLength)
            {
                _downloadBlocks.Add(new DownloadBlock()
                {
                    BeginOffset = temp,
                    EndOffset = temp + Setting.BlockSize - 1
                });
                temp += Setting.BlockSize;
            }
            _downloadBlocks.Add(new DownloadBlock()
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
                    request.Headers = new WebHeaderCollection(); //Setting.Headers.ToWebHeaderCollection();
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

        private void OnDownloadCompleted(DownloadThread thread)
        {
            lock (this)
            {
                if (_downloadThreads.Count == 0) return;
                _downloadThreads.Remove(thread);
                if (_downloadBlocks.All(v => v.Completed))
                {
                    Status = TransferStatus.Completed;
                    return;
                }

                var nextBlock = _downloadBlocks.FirstOrDefault(v => !v.Completed && !v.Downloading);
                if (nextBlock == null) return;
                var newThread = new DownloadThread(Uris.First(), LocalPath, nextBlock, Setting);
                newThread.Completed += OnDownloadCompleted;
                newThread.Failed += OnDownloadFailed;
                newThread.StartDownload();
                _downloadThreads.Add(newThread);
            }
        }

        private void OnDownloadFailed(DownloadThread thread)
        {
            if (Status != TransferStatus.Faulted)
            {
                _downloadThreads?.ForEach(v => v.Stop());
                SaveBlock();
                if (Status != TransferStatus.Faulted)
                    Status = TransferStatus.Faulted;

            }
        }

    }

}
