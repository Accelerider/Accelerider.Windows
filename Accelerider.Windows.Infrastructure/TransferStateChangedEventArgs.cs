using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure
{
    public class TransferStateChangedEventArgs
    {
        public TransferStateChangedEventArgs(TransferStateEnum oldState, TransferStateEnum newState)
        {
            OldState = oldState;
            NewState = newState;
        }

        public TransferStateEnum OldState { get; }
        public TransferStateEnum NewState { get; }
    }
}
