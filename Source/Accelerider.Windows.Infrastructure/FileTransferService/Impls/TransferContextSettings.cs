using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.FileTransferService.Impls
{
    internal class TransferContextSettings
    {
        public int MaxParallelTranspoterCount { get; set; } = 4;
    }
}
