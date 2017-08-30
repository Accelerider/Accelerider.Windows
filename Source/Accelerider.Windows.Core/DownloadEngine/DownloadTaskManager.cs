using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accelerider.Windows.Core.DownloadEngine.DownloadCore;
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


        public event Action<DownloadTaskItem, TransferStateEnum, TransferStateEnum> TaskStateChangeEvent;

        private List<DownloadTask> _handles;
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
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(500);
                    await ManagerTimer();
                }
            });
        }

        private async Task ManagerTimer()
        {
            var downloading = Tasks.Count(v => v.DownloadState == TransferStateEnum.Transfering);
            if (downloading < LocalConfigureInfo.Config.ParallelTaskNumber)
            {
                //移除所有下载完成的任务
                Tasks.RemoveAll(v => v.DownloadState == TransferStateEnum.Completed);
                var task = Tasks.FirstOrDefault(v => v.DownloadState == TransferStateEnum.Waiting);
                if (task == null)
                {
                    var item = Items.FirstOrDefault(v => !v.Completed && Tasks.All(t => t.DownloadPath != v.DownloadPath)); //在已创建任务中不存在
                    if (item == null) return;
                    var creator = AcceleriderUser.AccUser.GetTaskCreatorByUserid(item.FromUser);
                    IReadOnlyCollection<string> urls;
                    if (creator == null || (urls = await creator.GetDownloadUrls(item.FilePath)) == null)
                    {
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

        private void Task_DownloadStateChangedEvent(object sender, StateChangedArgs args)
        {
            var task = (HttpDownload)sender;
            switch (args.NewState)
            {
                case TransferStateEnum.Completed:
                    if (File.Exists(task.DownloadPath + ".downloading"))
                        File.Delete(task.DownloadPath + ".downloading");
                    Items.First(v => v.DownloadPath == task.DownloadPath).Completed = true;
                    Items.First(v => v.DownloadPath == task.DownloadPath).CompletedTime = DateTime.Now;
                    Save();
                    break;
                case TransferStateEnum.Faulted:
                    //TODO 刷新链接(还没做好)
                    break;
                case TransferStateEnum.Paused:
                    task.Info.Save(task.DownloadPath + ".downloading");
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
                Items.Add(task);
                Handles.Add(new DownloadTask(task));
            }
            Save();
            return Handles.FirstOrDefault(v => v.Item.DownloadPath == task.DownloadPath);
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
    }

    internal class DownloadTaskItem
    {
        public string FromUser { get; set; }

        public string FilePath { get; set; }

        public string DownloadPath { get; set; }

        public bool Completed { get; set; }

        public INetDiskFile NetDiskFile
        {
            get => _netDiskFile ?? (_netDiskFile = AcceleriderUser.AccUser
                       .GetTaskCreatorByUserid(FromUser)
                       .GetNetDiskFileByPath(FilePath));
            set => _netDiskFile = value;
        }


        [JsonIgnore]
        private INetDiskFile _netDiskFile;

        public DateTime CompletedTime { get; set; }
    }
}
