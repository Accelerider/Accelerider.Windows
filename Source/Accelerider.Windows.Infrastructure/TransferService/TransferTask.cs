using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public class TransferTask : ITransferTask
    {
        private readonly IObservable<ITransferContext> _observable;

        public TransferTask(IObservable<ITransferContext> observable)
        {
            _observable = observable;
        }

        public IObservable<ITransferContext> Activate()
        {
            return _observable;
        }

        public void Suspend()
        {
        }

        public IManagedTransferTask AsManaged(string queueName = null)
        {
            throw new NotImplementedException();
        }

        public static ITransferTask CreateLinkedTransferTask(IEnumerable<ITransferTask> tasks)
        {
            return new TransferTask(Observable.Create<ITransferContext>(o =>
            {
                var disposables = tasks.Select(item => item.Activate()).Select(item => item.Subscribe(o)).ToList();

                return () => disposables.ForEach(item => item.Dispose());
            }));
        }
    }
}
