using System;


namespace Accelerider.Windows.Infrastructure.TransferService
{
    public class BlockTransferException : Exception
    {
        public BlockTransferContext Context { get; }

        public BlockTransferException(BlockTransferContext context, Exception innerException)
            : base(string.Empty, innerException)
        {
            Context = context;
        }
    }
}
