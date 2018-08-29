using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autofac;

namespace Accelerider.Windows.Infrastructure.ViewModels
{
    public class ViewModelResolver
    {
        private Action<FrameworkElement, object> _configureViewAndViewModel;

        private readonly IContainer _container;

        public ViewModelResolver(IContainer container)
        {
            _container = container;
        }

        public object Resolve(object view, Type viewModelType)
        {
            var viewModel = _container.Resolve(viewModelType);
            if (view is FrameworkElement frameworkElement)
            {
                _configureViewAndViewModel?.Invoke(frameworkElement, viewModel);
            }
            return viewModel;
        }

        public ViewModelResolver IfInheritsFrom<TViewModel>(Action<FrameworkElement, TViewModel> configuration)
        {
            return IfInheritsFrom<FrameworkElement, TViewModel>(configuration);
        }

        public ViewModelResolver IfInheritsFrom<TView, TViewModel>(Action<TView, TViewModel> configuration)
            where TView : FrameworkElement
        {
            var previousAction = _configureViewAndViewModel;
            _configureViewAndViewModel = (view, viewModel) =>
            {
                previousAction?.Invoke(view, viewModel);
                if (view is TView tView && viewModel is TViewModel tViewModel)
                {
                    configuration?.Invoke(tView, tViewModel);
                }
            };
            return this;
        }
    }
}
