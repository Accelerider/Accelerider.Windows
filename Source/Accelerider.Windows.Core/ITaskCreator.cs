using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core
{
    internal interface ITaskCreator
    {

        IReadOnlyCollection<string> GetDownloadUrls(string file);

    }
}
