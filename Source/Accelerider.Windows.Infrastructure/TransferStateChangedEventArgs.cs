using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure
{
    public class TransferStateChangedEventArgs
    {
        public TransferStateChangedEventArgs(ITransferTaskToken token, TransferStateEnum oldState, TransferStateEnum newState)
        {
            Token = token;
            OldState = oldState;
            NewState = newState;
        }

        public ITransferTaskToken Token { get; }
        public TransferStateEnum OldState { get; }
        public TransferStateEnum NewState { get; }
    }
}
