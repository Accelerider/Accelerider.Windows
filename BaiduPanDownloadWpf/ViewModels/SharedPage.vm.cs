using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace BaiduPanDownloadWpf.ViewModels
{
    internal class SharedPageViewModel : ViewModelBase
    {
        public SharedPageViewModel(IUnityContainer container) : base(container)
        {
        }
    }
}
