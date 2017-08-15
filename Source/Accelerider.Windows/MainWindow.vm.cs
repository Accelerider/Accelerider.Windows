using System.Threading.Tasks;
using Accelerider.Windows.ViewModels;
using Microsoft.Practices.Unity;
using Accelerider.Windows.Assets;
using Accelerider.Windows.Events;

namespace Accelerider.Windows
{
    public class MainWindowViewModel : ViewModelBase
    {
        private int _transferTaskCount;


        public MainWindowViewModel(IUnityContainer container) : base(container)
        {
            GlobalMessageQueue.Enqueue(UiStrings.Message_Welcome);
            EventAggregator.GetEvent<DownloadTaskCreatedEvent>().Subscribe(e => TransferTaskCount += e.Count);
        }


        public int TransferTaskCount
        {
            get => _transferTaskCount;
            set => SetProperty(ref _transferTaskCount, value);
        }
    }
}
