using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core.DownloadEngine
{
    internal class DownloadTask : ITransferTaskToken, ITransferedFile
    {
        public DownloadTaskItem Item { get; }

        private bool _cancel;

        internal DownloadTask(DownloadTaskItem item)
        {
            Item = item;
            DownloadTaskManager.Manager.TaskStateChangeEvent += Manager_TaskStateChangeEvent;
        }

        private void Manager_TaskStateChangeEvent(DownloadTaskItem arg1, TransferTaskStatusEnum arg2, TransferTaskStatusEnum arg3)
        {
            if (arg1 == Item && !_cancel)
            {
                TransferTaskStatusChanged?.Invoke(this, new TransferTaskStatusChangedEventArgs(this, arg2, arg3));
            }
        }

        public event EventHandler<TransferTaskStatusChangedEventArgs> TransferTaskStatusChanged;

        
        public TransferTaskStatusEnum TaskStatus => GetTaskStatus();

        public string OwnerName => (AcceleriderUser.AccUser.GetTaskCreatorByUserid(Item.FromUser) as INetDiskUser)?.Username ?? "Unknown";

        public IFileSummary FileSummary => Item.NetDiskFile;

        public DataSize Progress => new DataSize(
            DownloadTaskManager.Manager.GetTaskProcess(Item)?.Info.CompletedLength ?? 0L);

        //任务已取消 -> Canceled, 等待检查 -> Checking， 传输列表不存在 -> Created, 检查完成 -> Completed
        private TransferTaskStatusEnum GetTaskStatus()
        {
            if (CheckStatus != FileCheckStatusEnum.NotAvailable)
                return TransferTaskStatusEnum.Completed;
            if (_cancel)
                return TransferTaskStatusEnum.Canceled;
            if (Item.WaitingCheck)
                //return TransferTaskStatusEnum.Checking;
                return TransferTaskStatusEnum.Completed; // TODO
            return DownloadTaskManager.Manager.GetTaskProcess(Item)?.DownloadState ??
                   TransferTaskStatusEnum.Created;
        }

        public async Task<bool> PauseAsync()
        {
            return await Task.Run(() =>
            {
                var task = DownloadTaskManager.Manager.GetTaskProcess(Item);
                if (task == null) return false;
                task.StopAndSave();
                return true;
            });
        }

        public async Task<bool> StartAsync(bool force = false)
        {
            return await Task.Run(() =>
            {
                var task = DownloadTaskManager.Manager.GetTaskProcess(Item);
                if (task == null || task.DownloadState == TransferTaskStatusEnum.Transferring || task.DownloadState == TransferTaskStatusEnum.Faulted) return false;
                task.DownloadState = TransferTaskStatusEnum.Waiting;
                return true;
            });
        }

        public async Task<bool> CancelAsync()
        {
            _cancel = true;
            return await Task.Run(() =>
            {
                var task = DownloadTaskManager.Manager.GetTaskProcess(Item);
                if (task == null || task.DownloadState == TransferTaskStatusEnum.Canceled) return false;
                var temp = task.DownloadState;
                if (task.DownloadState == TransferTaskStatusEnum.Transferring)
                    task.StopAndSave();
                task.DownloadState = TransferTaskStatusEnum.Canceled;
                TransferTaskStatusChanged?.Invoke(this, new TransferTaskStatusChangedEventArgs(this, temp, TransferTaskStatusEnum.Canceled));
                return true;
            });
        }

        public bool Equals(ITransferTaskToken other) => FileSummary.FilePath == other?.FileSummary.FilePath;
        public FileTypeEnum FileType => FileSummary.FileType;
        public Task<bool> DeleteAsync()
        {
            throw new NotImplementedException();
        }

        public ITransferedFile GetTransferedFile()
        {
            return TaskStatus == TransferTaskStatusEnum.Completed ? this : null;
        }

        public FileLocation FilePath => Item.FilePath;
        public DataSize FileSize => FileSummary.FileSize;
        public DateTime CompletedTime => Item.CompletedTime;

        public event EventHandler<FileCheckStatusEnum> FileChekced;
        public FileCheckStatusEnum CheckStatus => Item.FileCheckStatus;

    }
}
