using System;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core.Files
{
    public class TransferedFile : DiskFileBase, ITransferedFile
    {
        public DateTime CompletedTime { get; set; }

        public FileCheckStatusEnum CheckStatus => throw new NotImplementedException();

        public override async Task<bool> DeleteAsync()
        {
            await Task.Delay(1);
            return true;
        }
    }
}
