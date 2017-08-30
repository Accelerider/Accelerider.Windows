using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accelerider.Windows.Core.DownloadEngine.DownloadCore;
using Accelerider.Windows.Infrastructure;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;

namespace Accelerider.Windows.Core.DownloadEngine
{
    internal class DownloadTaskManager
    {
        public static DownloadTaskManager Manager { get; } = new DownloadTaskManager();

        public List<DownloadTaskItem> Items;
        public List<HttpDownload> Tasks;


        private readonly Timer _managerTimer;
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
            _managerTimer = new Timer()
            {
                Interval = 500
            };
            _managerTimer.Elapsed += ManagerTimerElapsed;
            _managerTimer.Start();
        }

        private void ManagerTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
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
                    IReadOnlyCollection<string> urls = new List<string>();
                    if (creator == null || (urls = creator.GetDownloadUrls(item.FilePath)) == null)
                    {
                        //TODO 在这里需要做任务失败的操作
                        return;
                    }
                    var info = HttpDownload.CreateTaskInfo(urls.ToArray(), item.DownloadPath, 16,
                        new Dictionary<string, string>());
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
            var task = (HttpDownload) sender;
            switch (args.NewState)
            {
                case TransferStateEnum.Completed:
                    if(File.Exists(task.DownloadPath+".downloading"))
                        File.Delete(task.DownloadPath+".downloading");
                    Items.First(v => v.DownloadPath == task.DownloadPath).Completed = true;
                    Save();
                    break;
                case TransferStateEnum.Faulted:
                    //TODO 刷新链接(还没做好)
                    break;
                case TransferStateEnum.Paused:
                    task.Info.Save(task.DownloadPath + ".downloading");
                    break;
            }
        }

        public void Enqueue(DownloadTaskItem task)
        {
            if (Items.All(v => v.DownloadPath != task.DownloadPath))
                Items.Add(task);
            Save();
        }

        public void Remove(DownloadTaskItem task)
        {
            Save();
        }

        public void Save()
        {
            var taskListFile = Path.Combine(Directory.GetCurrentDirectory(), "DownloadList.json");
            File.WriteAllText(taskListFile, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }

    internal class DownloadTaskItem
    {
        public string FromUser { get; set; }

        public string FilePath { get; set; }

        public string DownloadPath { get; set; }

        public bool Completed { get; set; }
    }
}
