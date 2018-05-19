using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.FileTransferService.Impls
{
    internal class TransferCommandImpl : ITransferCommand
    {


        public TransferCommandImpl(TransporterBaseImpl transporter)
        {

        }

        public void Restart()
        {
            
        }

        public void Suspend()
        {
            throw new NotImplementedException();
        }

        public void AsNext()
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }
    }
}
