namespace Accelerider.Windows.Infrastructure.FileTransferService
{
    public interface ITransferCommand
    {
        void Restart();

        void Suspend();

        void AsNext();

        void Cancel();
    }
}
