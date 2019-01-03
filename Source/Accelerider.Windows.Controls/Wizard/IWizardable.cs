using System.Threading.Tasks;
using System.Windows.Input;

namespace Accelerider.Windows.Controls.Wizard
{
    public interface IWizardable
    {
        ICommand BackCommand { get; set; }

        ICommand NextCommand { get; set; }

        Task<object> OnLeavingAsync();

        void OnEntering(object parameter);

        bool CanExecuteNextCommand();
    }

    public interface IWizardable<in T> : IWizardable
    {
        void OnEntering(T parameter);
    }
}
