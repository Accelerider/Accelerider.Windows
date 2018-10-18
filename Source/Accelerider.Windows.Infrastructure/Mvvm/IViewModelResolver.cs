using System;
using System.Windows;

namespace Accelerider.Windows.Infrastructure.Mvvm
{
    public interface IViewModelResolver
    {
        object Resolve(object view, Type viewModelType);

        IViewModelResolver IfInheritsFrom<TViewModel>(Action<FrameworkElement, TViewModel> configuration);

        IViewModelResolver IfInheritsFrom<TView, TViewModel>(Action<TView, TViewModel> configuration) where TView : FrameworkElement;

        IViewModelResolver IfInheritsFrom(Type genericInterfaceType, Action<IGenericInterface, FrameworkElement, object> configuration);
    }
}
