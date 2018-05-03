using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure.Implements.DownloadEngine
{
    public class DownloadTaskBuilder : DownloadTaskBuilderBase
    {
        

        public override ITaskBuilder Clone()
        {
            throw new NotImplementedException();
        }

        public override ITransportTask Build()
        {
            var result=new DownloadTask();
            return result;
        }

        public override ITransportTask Update(ITransportTask task)
        {
            throw new NotImplementedException();
        }
    }
}
