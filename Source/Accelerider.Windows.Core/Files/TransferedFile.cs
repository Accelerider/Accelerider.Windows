using System;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core.Files
{
    public class TransferedFile : DiskFileBase, ITransferedFile
    {
        public DateTime CompletedTime { get; }
    }
}
