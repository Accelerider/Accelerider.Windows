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

        internal DownloadTask(DownloadTaskItem item)
        {
            Item = item;
            DownloadTaskManager.Manager.TaskStateChangeEvent += Manager_TaskStateChangeEvent;
        }

        private void Manager_TaskStateChangeEvent(DownloadTaskItem arg1, TransferTaskStatusEnum arg2, TransferTaskStatusEnum arg3)
        {
            if (arg1 == Item)
            {
                TransferTaskStatusChanged?.Invoke(this, new TransferTaskStatusChangedEventArgs(this, arg2, arg3));
            }
        }

        public event EventHandler<TransferTaskStatusChangedEventArgs> TransferTaskStatusChanged;

        public TransferTaskStatusEnum TaskStatus => DownloadTaskManager.Manager.GetTaskProcess(Item)?.DownloadState ??
                                                  TransferTaskStatusEnum.Canceled;

        public string OwnerName => (AcceleriderUser.AccUser.GetTaskCreatorByUserid(Item.FromUser) as INetDiskUser)?.Username ?? "Unknown";

        public IFileSummary FileSummary => Item.NetDiskFile;

        public DataSize Progress => new DataSize(
            DownloadTaskManager.Manager.GetTaskProcess(Item)?.Info.CompletedLength ?? 0L);

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
                if (task == null || task.DownloadState == TransferTaskStatusEnum.Transfering || task.DownloadState == TransferTaskStatusEnum.Faulted) return false;
                task.DownloadState = TransferTaskStatusEnum.Waiting;
                return true;
            });
        }

        public Task<bool> CancelAsync()
        {
            throw new NotImplementedException();
        }

        public bool Equals(ITransferTaskToken other) => false;
        public FileTypeEnum FileType => FileSummary.FileType;
        public Task<bool> DeleteAsync()
        {
            throw new NotImplementedException();
        }

        public ITransferedFile GetTransferedFile()
        {
            return TaskStatus == TransferTaskStatusEnum.Checking ? this : null;
        }

        public FileLocation FilePath => Item.FilePath;
        public DataSize FileSize => FileSummary.FileSize;
        public DateTime CompletedTime => Item.CompletedTime;

        public FileCheckStatusEnum CheckStatus => Item.FileCheckStatus;

    }
}
