using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Core.DownloadEngine.DownloadCore;
using Accelerider.Windows.Core.Tools;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json;

namespace Accelerider.Windows.Core.DownloadEngine
{
    internal class DownloadTaskManager
    {
        public static DownloadTaskManager Manager { get; } = new DownloadTaskManager();

        public List<DownloadTaskItem> Items;
        public List<DownloadTask> Handles => _handles ?? (_handles = Items.Select(v => new DownloadTask(v)).ToList());
        public List<HttpDownload> Tasks;

        public event Action<DownloadTaskItem, TransferTaskStatusEnum, TransferTaskStatusEnum> TaskStateChangeEvent;

        private bool _stop = false;

        private List<DownloadTask> _handles;
        private List<string> _deleteList = new List<string>();

        private DownloadTaskManager()
        {
            var taskListFile = Path.Combine(Directory.GetCurrentDirectory(), "DownloadList.json");
            Items = File.Exists(taskListFile)
                ? JsonConvert.DeserializeObject<List<DownloadTaskItem>>(File.ReadAllText(taskListFile))
                : new List<DownloadTaskItem>();
            Tasks = Items.Where(v => File.Exists(v.DownloadPath + ".downloading"))
                .Select(v => HttpDownload.GetTaskByInfo(
                    JsonConvert.DeserializeObject<DownloadInfo>(File.ReadAllText(v.DownloadPath + ".downloading"))))
                .ToList();
            Tasks.ForEach(v => v.DownloadStateChangedEvent += Task_DownloadStateChangedEvent);
            //任务管理
            Task.Run(async () =>
            {
                while (!_stop)
                {
                    await Task.Delay(500);
                    if (!_stop)
                        await ManagerTimer();
                }
            });
            //效验管理
            Task.Run(async () =>
            {
                while (!_stop)
                {
                    await Task.Delay(500);
                    if (!_stop)
                        await CheckFile();
                }
            });
            //删除循环
            Task.Run(async () =>
            {
                while (!_stop)
                {
                    await Task.Delay(500);
                    foreach (var path in _deleteList.ToArray())
                    {
                        if (!File.Exists(path)) _deleteList.Remove(path);
                        try
                        {
                            File.Delete(path);
                            _deleteList.Remove(path);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 下载任务调度
        /// </summary>
        /// <returns></returns>
        private async Task ManagerTimer()
        {
            var downloading = Tasks.Count(v => v.DownloadState == TransferTaskStatusEnum.Transferring);
            if (downloading < LocalConfigureInfo.Config.ParallelTaskNumber)
            {
                var task = Tasks.FirstOrDefault(v => v.DownloadState == TransferTaskStatusEnum.Waiting);
                if (task == null)
                {
                    var item = Items.FirstOrDefault(v => !v.Completed && Tasks.All(t => t.DownloadPath != v.DownloadPath)); //在已创建任务中不存在
                    if (item == null) return;
                    var creator = AcceleriderUser.AccUser.GetTaskCreatorByUserid(item.FromUser);
                    IReadOnlyCollection<string> urls;
                    if (creator == null || (urls = await creator.GetDownloadUrls(item.FilePath)) == null)
                    {
                        Console.WriteLine("Get download link fail");
                        //TODO 在这里需要做任务失败的操作
                        return;
                    }
                    var blockSize = 1024 * 1024 * 10L;
                    if (!(creator is IBaiduCloudUser))
                        blockSize = 100 * 1024 * 1024;
                    var info = HttpDownload.CreateTaskInfo(urls.ToArray(), item.DownloadPath, 16,
                        new Dictionary<string, string>(), null, blockSize);
                    info.Save(item.DownloadPath + ".downloading");
                    task = HttpDownload.GetTaskByInfo(info);
                    Tasks.Add(task);
                }
                task.DownloadStateChangedEvent += Task_DownloadStateChangedEvent;
                task.Start();
            }
        }

        /// <summary>
        /// 文件效验任务调度
        /// </summary>
        /// <returns></returns>
        private async Task CheckFile()
        {
            var item = Items.FirstOrDefault(v => v.WaitingCheck);
            if (item == null)
                return;
            if (AcceleriderUser.AccUser.GetTaskCreatorByUserid(item.FromUser) is IBaiduCloudUser)
            {
                try
                {
                    if (File.Exists(item.DownloadPath))
                    {
                        var md5 = await FileTools.GetMd5HashFromFile(item.DownloadPath);
                        if (!string.IsNullOrEmpty(md5))
                        {
                            //TODO 需要修改服务端 暂时啥都不做
                            item.WaitingCheck = false;
                            item.FileCheckStatus = FileCheckStatusEnum.Normal;
                            //TaskStateChangeEvent?.Invoke(item, TransferTaskStatusEnum.Checking,/* TODO: Please confirm this change. */ TransferTaskStatusEnum.Completed);
                            Save();
                            return;
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            item.WaitingCheck = false;
            item.FileCheckStatus = FileCheckStatusEnum.Warning;
            //TaskStateChangeEvent?.Invoke(item,TransferTaskStatusEnum.Checking,/* TODO: Please confirm this change. */ TransferTaskStatusEnum.Completed);
            Save();
        }

        private async void Task_DownloadStateChangedEvent(object sender, StateChangedArgs args)
        {
            var task = (HttpDownload)sender;
            var item = Items.FirstOrDefault(v => v.DownloadPath == task.DownloadPath);
            switch (args.NewState)
            {
                case TransferTaskStatusEnum.Completed: // TODO: Please confirm this change.
                    if (File.Exists(task.DownloadPath + ".downloading"))
                        File.Delete(task.DownloadPath + ".downloading");
                    item.Completed = true;
                    item.CompletedTime = DateTime.Now;
                    item.WaitingCheck = true;
                    Tasks.Remove(task);
                    Save();
                    break;
                case TransferTaskStatusEnum.Faulted:
                    if (task.Data.ContainsKey("LastRefreshTime") && DateTime.Now - DateTime.Parse(task.Data["LastRefreshTime"]) < new TimeSpan(1, 0, 0)) //如果最后刷新时间距离现在小于一小时
                        break;
                    var creator = AcceleriderUser.AccUser.GetTaskCreatorByUserid(item.FromUser);
                    IReadOnlyCollection<string> urls;
                    if (creator == null || (urls = await creator.GetDownloadUrls(item.FilePath)) == null)
                    {
                        //TODO 在这里需要做任务失败的操作
                        break;
                    }
                    task.Info.DownloadUrl = urls.ToArray();
                    task.Info.DownloadCount.Clear();
                    if (!task.Data.ContainsKey("LastRefreshTime"))
                        task.Data.Add("LastRefreshTime", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    task.Data["LastRefreshTime"] = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                    task.Info.Save(item.DownloadPath + ".downloading");
                    task.DownloadState = TransferTaskStatusEnum.Waiting;
                    return;
                case TransferTaskStatusEnum.Paused:
                    task.Info.Save(task.DownloadPath + ".downloading");
                    break;
                case TransferTaskStatusEnum.Canceled:
                    _deleteList.Add(task.DownloadPath);
                    _deleteList.Add(task.DownloadPath + ".downloading");
                    Delete(item);
                    Tasks.Remove(task);
                    Save();
                    return;
                case TransferTaskStatusEnum.Waiting:
                    if (args.OldState == TransferTaskStatusEnum.Faulted)
                        return;
                    break;
            }
            TaskStateChangeEvent?.Invoke(Items.First(v => v.DownloadPath == task.DownloadPath), args.OldState, args.NewState);
        }

        public void Enqueue(DownloadTaskItem task)
        {

        }

        public DownloadTask Add(DownloadTaskItem task)
        {
            if (Items.All(v => v.DownloadPath != task.DownloadPath))
            {
                task.DownloadPath=task.DownloadPath.Replace("?", " ");
                Items.Add(task);
                var downloadTask = new DownloadTask(task);
                Handles.Add(downloadTask);
                Save();
                return downloadTask;
            }
            return null;
        }

        public void Delete(DownloadTaskItem task)
        {
            Items.Remove(task);
            Handles.RemoveAll(v => v.Item == task);
            Save();
        }

        public HttpDownload GetTaskProcess(DownloadTaskItem task)
        {
            return Tasks.FirstOrDefault(v => v.DownloadPath == task.DownloadPath);
        }

        public void Remove(DownloadTaskItem task)
        {
            Save();
        }

        public void Save()
        {
            var taskListFile = Path.Combine(Directory.GetCurrentDirectory(), "DownloadList.json");
            File.WriteAllText(taskListFile, JsonConvert.SerializeObject(Items.ToArray(), Formatting.Indented));
        }

        public void Stop()
        {
            _stop = true;
            Tasks.ForEach(v => v.StopAndSave());
        }
    }

    internal class DownloadTaskItem
    {
        public string FromUser { get; set; }

        public string FilePath { get; set; }

        public string DownloadPath { get; set; }

        public bool Completed { get; set; }

        public DownloadTaskFile NetDiskFile { get; set; }

        public FileCheckStatusEnum FileCheckStatus { get; set; } = FileCheckStatusEnum.NotAvailable;

        public bool WaitingCheck { get; set; } = false;

        public DateTime CompletedTime { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is DownloadTaskItem))
                return false;
            var b = (DownloadTaskItem)obj;
            return this == b;
        }


        public override int GetHashCode()
        {
            return DownloadPath.GetHashCode();
        }

        public static bool operator ==(DownloadTaskItem left, DownloadTaskItem right)
        {
            return left?.DownloadPath == right?.DownloadPath;
        }

        public static bool operator !=(DownloadTaskItem left, DownloadTaskItem right)
        {
            return !(left == right);
        }
    }

    internal class DownloadTaskFile : IFileSummary
    {
        public FileTypeEnum FileType { get; set; }

        [JsonIgnore]
        public FileLocation FilePath
        {
            get => new FileLocation(_path);
            set => _path = value;
        }

        [JsonIgnore]
        public DataSize FileSize
        {
            get => new DataSize(_fileSize);
            set => _fileSize = value.BaseBValue;
        }


        [JsonProperty("FilePath")]
        private string _path;

        [JsonProperty("FileSize")]
        private long _fileSize;
    }
}
