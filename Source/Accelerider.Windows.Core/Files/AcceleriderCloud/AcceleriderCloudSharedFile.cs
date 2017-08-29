using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core.Files.AcceleriderCloud
{
    internal class AcceleriderCloudSharedFile : SharedFile
    {
        public new Task<bool> DeleteAsync()
        {
            throw new NotImplementedException();
        }
    }
}
