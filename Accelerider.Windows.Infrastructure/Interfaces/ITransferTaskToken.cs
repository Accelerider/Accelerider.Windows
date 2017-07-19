using System;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITransferTaskToken : IEquatable<ITransferTaskToken>
    {
        /// <summary>
        /// 
        /// </summary>
        TransferStateEnum TransferState { get; }

        /// <summary>
        /// 
        /// </summary>
        IDiskFile FileInfo { get; }

        /// <summary>
        /// 
        /// </summary>
        DataSize Progress { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<bool> PauseAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<bool> RestartAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<bool> CancelAsync();
    }
}
