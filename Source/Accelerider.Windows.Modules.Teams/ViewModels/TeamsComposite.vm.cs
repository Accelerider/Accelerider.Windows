using Accelerider.Windows.Infrastructure;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.Teams.ViewModels
{
    public class TeamsCompositeViewModel : ViewModelBase
    {
        public TeamsCompositeViewModel(IUnityContainer container) : base(container)
        {
        }
    }
}
