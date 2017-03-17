using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using BaiduPanDownloadWpf.Core.Download.DwonloadCore;
using BaiduPanDownloadWpf.Infrastructure;
using BaiduPanDownloadWpf.Infrastructure.Events;
using Microsoft.Practices.Unity;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;

namespace BaiduPanDownloadWpf.Core.Download
{
    public class TaskManager : ModelBase
    {
        private static readonly Dictionary<string, TaskManager> Manager = new Dictionary<string, TaskManager>();

        public static TaskManager GetTaskManagerByLocalDiskUser(IUnityContainer container, MountUser user)
        {
            if (!Manager.ContainsKey(user.Username))
            {
                //Manager.Add(user.Name, new TaskManager(user));
                Manager.Add(user.Username, new TaskManager(container, user));
            }
            return Manager[user.Username];
        }

        private readonly MountUser _user;
        private readonly TaskDatabase _database;
        private readonly string _dataFolder;
        private readonly List<HttpDownload> _downloadingTasks = new List<HttpDownload>();
        private bool _runing;

        public string TaskListFile => Path.Combine(_dataFolder, "TaskList.json");

        public TaskManager(IUnityContainer container, MountUser user) : base(container)
        {
            _user = user;
            _dataFolder = Path.Combine(Common.UserDataSavePath, user.Username);
            if (!Directory.Exists(_dataFolder))
                Directory.CreateDirectory(_dataFolder);
            _database = TaskDatabase.GetDatabaseByUser(Container, user);
            _runing = true;
            new Thread(async () =>
            {
                while (_runing)
                {
                    Thread.Sleep(1000);
                    if (_downloadingTasks.Count(v => v.DownloadState == DownloadStateEnum.Downloading) < Container.Resolve<ILocalConfigInfo>().ParallelTaskNumber)
                    {
                        //如果正在下载的文件数量与已经请求的文件数量相同
                        if (_database.GetDownloadingTask().Length == _downloadingTasks.Count)
                        {
                            var result = await _database.Next();
                            if (result == null)
                            {
                                continue;
                            }
                            if (result.ErrorCode != 0)
                            {
                                if (result.ErrorCode == 209)
                                {
                                    //没有新的任务了
                                }
                                //出现错误时的处理
                                continue;

                            }
                            //Created -> Waiting
                            EventAggregator.GetEvent<DownloadStateChangedEvent>().Publish(new DownloadStateChangedEventArgs(_database.GetFileIdByPath(result.Info.DownloadPath), DownloadStateEnum.Created, DownloadStateEnum.Waiting));
                            AddDownloadingTask(result.Info);
                            continue;
                        }
                        var data =
                            _database.GetDownloadingTask()
                                .FirstOrDefault(v => _downloadingTasks.All(v2 => v.DownloadPath != v2.DownloadPath));
                        if (data != null)
                            AddDownloadingTask(data.Info);
                    }
                }
            })
            { IsBackground = true }.Start();
        }

        private void AddDownloadingTask(DownloadInfo info)
        {
            var download = HttpDownload.GetTaskByInfo(info);
            download.DownloadStateChangedEvent += Download_DownloadStateChangedEvent;
            download.DownloadPercentageChangedEvent += Download_DownloadPercentageChangedEvent;
            download.Start();
            _downloadingTasks.Add(download);
        }

        private void Download_DownloadPercentageChangedEvent(object sender, PercentageChangedEventArgs args)
        {
            var info = (HttpDownload)sender;

            Debug.WriteLine($"PercentageChanged: Task: {info.DownloadPath} Speed: {args.Speed} Progress: {args.Progress}");

            //TaskPercentageChangedEvent?.Invoke(this, new DownloadProgressChangedEventArgs(_database.GetFileIdByPath(info.DownloadPath), new DataSize(args.Progress), new DataSize(args.Speed)));
            EventAggregator.GetEvent<DownloadProgressChangedEvent>().Publish(new DownloadProgressChangedEventArgs(_database.GetFileIdByPath(info.DownloadPath), new DataSize(args.Progress), new DataSize(args.Speed)));
        }

        private void Download_DownloadStateChangedEvent(object sender, StateChangedArgs args)
        {
            var info = (HttpDownload)sender;

            Debug.WriteLine($"PercentageChanged: Task: {info.DownloadPath} State: {args.OldState} -> {args.NewState}");

            var fileId = _database.GetFileIdByPath(info.DownloadPath);
            switch (args.NewState)
            {
                case DownloadStateEnum.Completed:
                    _database.SetCompleted(info.DownloadPath);
                    _downloadingTasks.Remove(info);
                    break;
                case DownloadStateEnum.Waiting:
                    _downloadingTasks.Remove(info);
                    //暂停所做操作
                    break;
            }
            //TaskStateChangedEvent?.Invoke(this, new DownloadStateChangedEventArgs(_database.GetFileIdByPath(info.DownloadPath), args.OldState, args.NewState));
            EventAggregator.GetEvent<DownloadStateChangedEvent>().Publish(new DownloadStateChangedEventArgs(fileId, args.OldState, args.NewState));
        }

        public LocalDiskFile[] GetUncompletedList()
        {
            return _database.GetUncompletedList().Select(v => new LocalDiskFile(v.Id, v.DownloadPath, v.DownloadFileInfo.FileType, v.DownloadFileInfo.FileSize, DateTime.Now)).ToArray();
        }

        public LocalDiskFile[] GetCompletedList()
        {
            return _database.GetCompletedList().Select(v => new LocalDiskFile(v.Id, v.DownloadPath, v.FileInfo.FileType, v.FileInfo.FileSize, v.CompletedTime)).ToArray();
        }

        public void StopAndSave()
        {
            _runing = false;
            _downloadingTasks.ForEach(v =>
            {
                var info = v.StopAndSave();
                _database.UpdateTask(info);
            });
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="id"></param>
        public void PauseTask(long id)
        {
            if (!_database.Contains(id))
                return;
            if (_downloadingTasks.Any(v => v.DownloadPath == _database.GetFilePathById(id)))
            {
                _downloadingTasks.ForEach(v =>
                {
                    if (v.DownloadPath == _database.GetFilePathById(id))
                    {
                        var info = v.StopAndSave();
                        if (info != null)
                            _database.UpdateTask(info);
                    }
                });
            }
        }

        /// <summary>
        /// 继续任务
        /// </summary>
        /// <param name="id"></param>
        public void ContinueTask(long id)
        {
            if (!_database.Contains(id))
                return;
            if (_downloadingTasks.Any(v => v.DownloadPath == _database.GetFilePathById(id) && v.DownloadState != DownloadStateEnum.Downloading))
            {
                _downloadingTasks.ForEach(v =>
                {
                    if (v.DownloadPath == _database.GetFilePathById(id) && v.DownloadState != DownloadStateEnum.Downloading)
                    {
                        v.Start();
                    }
                });
            }
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="id"></param>
        public async Task RemoveTask(long id)
        {
            await Task.Run(() =>
            {
                if (_database.GetFilePathById(id) != string.Empty)
                {
                    var path = _database.GetFilePathById(id);
                    if (_downloadingTasks.Any(v => v.DownloadPath == path))
                    {
                        var task = _downloadingTasks.FirstOrDefault(v => v.DownloadPath == path);
                        task.StopAndSave();
                        _downloadingTasks.Remove(task);
                    }
                }
                _database.RemoveTask(id);
            });
            EventAggregator.GetEvent<DownloadStateChangedEvent>().Publish(new DownloadStateChangedEventArgs(id, DownloadStateEnum.Waiting, DownloadStateEnum.Canceled));
        }

        /// <summary>
        /// 新建下载任务
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool CreateTask(NetDiskFile file, string path)
        {
            if (_database.Contains(path))
            {
                return false;
            }
            _database.Add(file, path);
            return true;
        }

    }
}
