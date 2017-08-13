using System;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core.Files
{
    public class TransferedFile : DiskFileBase, ITransferedFile
    {
        public DateTime CompletedTime { get; }
        public override Task<bool> DeleteAsync()
        {
            throw new NotImplementedException();
        }
    }
}
