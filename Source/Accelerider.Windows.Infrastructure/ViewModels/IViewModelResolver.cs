using System;
using System.Windows;

namespace Accelerider.Windows.Infrastructure.ViewModels
{
    public interface IViewModelResolver
    {
        object Resolve(object view, Type viewModelType);

        ViewModelResolver IfInheritsFrom<TViewModel>(Action<FrameworkElement, TViewModel> configuration);

        ViewModelResolver IfInheritsFrom<TView, TViewModel>(Action<TView, TViewModel> configuration) where TView : FrameworkElement;
    }
}
