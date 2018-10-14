using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;


namespace Accelerider.Windows.Infrastructure.TransferService
{
    internal class FileDownloaderManager : IDownloaderManager
    {
        public const string AcceleriderDownloadFileExtension = ".ard";

        private readonly ConcurrentList<IDownloader> _executingList = new ConcurrentList<IDownloader>();
        private readonly ConcurrentList<IDownloader> _pendingList = new ConcurrentList<IDownloader>();
        private readonly ConcurrentList<IDownloader> _suspendedList = new ConcurrentList<IDownloader>();
        private readonly ConcurrentList<IDownloader> _completedList = new ConcurrentList<IDownloader>();
        private readonly ConcurrentList<IDownloader>[] _allLists;
        private readonly AsyncLocker _advanceLocker = new AsyncLocker();
        private readonly ConcurrentDictionary<Guid, IDisposable> _observers = new ConcurrentDictionary<Guid, IDisposable>();

        private int _maxConcurrent;

        public int MaxConcurrent
        {
            get => _maxConcurrent;
            set
            {
                if (_maxConcurrent == value) return;
                _maxConcurrent = value;
                Advance();
            }
        }

        public IEnumerable<IDownloader> Downloaders => _allLists.SelectMany(item => item);

        public FileDownloaderManager()
        {
            _allLists = new[] { _executingList, _pendingList, _suspendedList, _completedList };
        }

        public bool Add(IDownloader downloader)
        {
            switch (downloader.Status)
            {
                case TransferStatus.Ready:
                    _pendingList.Add(downloader);
                    Advance();
                    break;
                case TransferStatus.Transferring:
                    if (MaxConcurrent > _executingList.Count)
                    {
                        _executingList.Add(downloader);
                    }
                    else
                    {
                        downloader.Suspend();
                        _pendingList.Add(downloader);
                    }
                    break;
                case TransferStatus.Suspended:
                case TransferStatus.Faulted:
                    _suspendedList.Add(downloader);
                    break;
                case TransferStatus.Completed:
                    _completedList.Add(downloader);
                    break;
                default:
                    return false;
            }

            return true;
        }

        public void AsNext(Guid id)
        {
            var downloader = DequeueById(id);

            if (CanAsNext(downloader))
            {
                _pendingList.Insert(0, downloader);
                Advance();
            }
        }

        public void Ready(Guid id)
        {
            MoveToTail(_pendingList, id, _suspendedList);
            Advance();
        }

        public void StartAll()
        {
            while (_suspendedList.Any())
            {
                _pendingList.Add(_suspendedList.Dequeue());
            }
            Advance();
        }

        public void SuspendAll()
        {
            _observers.Values.ForEach(item => item.Dispose());
            _observers.Clear();
            _executingList.ForEach(item => item.Suspend());
        }

        public string ToJson()
        {
            if (_executingList.Any()) throw new InvalidOperationException();

            return _allLists.SelectMany(item => item)
                .Where(item => !string.IsNullOrEmpty(item.Context?.LocalPath))
                .Select(item =>
                {
                    var path = item.Context.LocalPath + AcceleriderDownloadFileExtension;
                    File.WriteAllText(path, item.ToJson());
                    return path;
                })
                .ToList()
                .ToJson(Formatting.Indented);
        }

        public IDownloaderManager FromJson(string json)
        {
            json.ToObject<List<string>>()?
                .Where(item => File.Exists(item))
                .Select(item => File.ReadAllText(item))
                .Select(item =>
                {
                    var configureTag = item.GetJsonValue(nameof(IDownloader.Tag));
                    return !string.IsNullOrEmpty(configureTag)
                        ? FileTransferService.GetFileDownloaderBuilder().UseConfigure(configureTag).Build().FromJson(item)
                        : null;
                })
                .Where(item => item != null)
                .ForEach(item => Add(item));

            return this;
        }

        private bool CanAsNext(IDownloader item)
        {
            return item != null &&
                   item.Status != TransferStatus.Transferring &&
                   item.Status != TransferStatus.Completed &&
                   item.Status != TransferStatus.Disposed;
        }

        private void Advance() => _advanceLocker.Await(async () =>
        {
            if (MaxConcurrent > _executingList.Count) await IncreaseTasksAsync();
            if (MaxConcurrent < _executingList.Count) DecreaseTasks();
        }, executeAfterUnlocked: false);

        private async Task IncreaseTasksAsync()
        {
            while (MaxConcurrent > _executingList.Count && _pendingList.Any())
            {
                var downloader = _pendingList.Dequeue();
                if (downloader == null) break;

                await downloader.ActivateAsync();

                ObserveDownloader(downloader);
                _executingList.Insert(0, downloader);
            }
        }

        private void DecreaseTasks()
        {
            while (MaxConcurrent < _executingList.Count && _executingList.Any())
            {
                var downloader = _executingList.Pop();
                if (downloader == null) break;

                downloader.Suspend();

                _pendingList.Insert(0, downloader);
            }
        }

        private IDisposable ObserveDownloader(IDownloader downloader)
        {
            var id = downloader.Id;

            var disposable = downloader
                .Distinct(item => item.Status)
                .Subscribe(item =>
                {
                    switch (item.Status)
                    {
                        case TransferStatus.Ready:
                            MoveToTail(_pendingList, id, _executingList, _suspendedList);
                            break;
                        case TransferStatus.Transferring:
                            MoveToTop(_executingList, id, _pendingList, _suspendedList);
                            break;
                        case TransferStatus.Suspended:
                            MoveToTail(_suspendedList, id, _executingList);
                            break;
                        case TransferStatus.Disposed:
                            DequeueById(id);
                            break;
                    }
                    Advance();
                }, error =>
                {
                    MoveToTail(_suspendedList, id, _executingList, _pendingList);
                    Advance();
                }, () =>
                {
                    MoveToTop(_completedList, id, _executingList, _pendingList, _suspendedList);
                    if (_observers.TryRemove(id, out var token))
                    {
                        token.Dispose();
                    }
                    Advance();
                });

            _observers.TryAdd(id, disposable);

            return disposable;
        }

        private bool MoveToTail(ConcurrentList<IDownloader> target, Guid id, params ConcurrentList<IDownloader>[] sources)
        {
            return DequeueAndOperateById(id, target.Add, sources);
        }

        private bool MoveToTop(ConcurrentList<IDownloader> target, Guid id, params ConcurrentList<IDownloader>[] sources)
        {
            return DequeueAndOperateById(id, item => target.Insert(0, item), sources);
        }

        private bool DequeueAndOperateById(Guid id, Action<IDownloader> operation, params ConcurrentList<IDownloader>[] sources)
        {
            var temp = DequeueById(id, sources);

            var result = temp != null;
            if (result) operation?.Invoke(temp);

            return result;
        }

        private IDownloader DequeueById(Guid id, params ConcurrentList<IDownloader>[] sources)
        {
            var set = sources.Any() ? sources : _allLists;

            foreach (var list in set)
            {
                var result = list.Dequeue(item => item.Id == id);
                if (result != null) return result;
            }

            return null;
        }
    }
}
