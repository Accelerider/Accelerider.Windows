using System;
using System.IO;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Models;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels.Entering
{
    public class EnteringWindowViewModel : ViewModelBase
    {
        private bool _isLoading;


        public EnteringWindowViewModel(IUnityContainer container) : base(container)
        {
            EventAggregator.GetEvent<MainWindowLoadingEvent>().Subscribe(e => IsLoading = e);
        }


        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public override async void OnLoaded(object view)
        {
            var publickeyPath = Path.Combine(Environment.CurrentDirectory, "publickey.xml");
            if (!File.Exists(publickeyPath))
            {
                var nonAuthApi = Container.Resolve<INonAuthenticationApi>();
                var publickey = await nonAuthApi.GetPublicKeyAsync();
                File.WriteAllText(publickeyPath, publickey);
            }
        }
    }
}
