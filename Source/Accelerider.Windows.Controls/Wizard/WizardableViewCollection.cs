using Accelerider.Windows.Infrastructure.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;

namespace Accelerider.Windows.Controls.Wizard
{
    public class WizardableViewCollection : ObservableCollection<FrameworkElement>
    {
        private readonly UserControl _owner;
        private FrameworkElement _currentView;

        public FrameworkElement CurrentView
        {
            get => _currentView ?? (_currentView = Items.FirstOrDefault());
            set => _currentView = value;
        }

        public IWizardable CurrentViewModel => GetViewModelFrom(CurrentView);

        public WizardableViewCollection(UserControl owner)
        {
            _owner = owner;
        }

        protected override void InsertItem(int index, FrameworkElement item)
        {
            base.InsertItem(index, item);
            InitializeItem(item);
        }

        private void InitializeItem(FrameworkElement item)
        {
            var viewModel = GetViewModelFrom(item);
            viewModel.BackCommand = new RelayCommand(
                BackInternal,
                () => Items.IndexOf(CurrentView) > 0);
            viewModel.NextCommand = new RelayCommandAsync(
                NextInternalAsync,
                () => Items.IndexOf(CurrentView) < Items.Count - 1 && viewModel.CanExecuteNextCommand());
        }

        private void BackInternal()
        {
            SetCurrentView(Items.IndexOf(CurrentView) - 1);
        }

        private async Task NextInternalAsync()
        {
            var parameter = await CurrentViewModel.OnLeavingAsync();

            if (ReferenceEquals(parameter, WizardOptions.Break) &&
                CurrentViewModel is DialogViewModel dialogViewModel)
            {
                dialogViewModel.CloseCommand.Invoke();
            }

            if (ReferenceEquals(parameter, WizardOptions.Cancel)) return;

            SetCurrentView(Items.IndexOf(CurrentView) + 1);
            CurrentViewModel.OnEntering(parameter);
        }

        private void SetCurrentView(int index)
        {
            if (index < 0 || index >= Items.Count) return;

            CurrentView = Items[index];
            _owner.Dispatcher.Invoke(new Action(() => _owner.Content = CurrentView));
        }

        private static IWizardable GetViewModelFrom(FrameworkElement view)
        {
            if (view == null) return null;

            if (view.DataContext is IWizardable wizardableViewModel)
                return wizardableViewModel;

            throw new Exception("The view model of the wizardable view must implements the IWizardable interface. ");
        }
    }
}
