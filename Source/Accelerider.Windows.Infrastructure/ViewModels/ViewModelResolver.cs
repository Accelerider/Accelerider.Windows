using System;
using System.Windows;
using Accelerider.Windows.Infrastructure.I18n;
using Autofac;

namespace Accelerider.Windows.Infrastructure.ViewModels
{
    public class ViewModelResolver : IViewModelResolver
    {
        private Action<FrameworkElement, object> _configureViewAndViewModel;

        public IContainer Container { get; }

        public ViewModelResolver(IContainer container)
        {
            Container = container;
        }

        public object Resolve(object view, Type viewModelType)
        {
            var viewModel = Container.Resolve(viewModelType);
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

    public static class ViewModelResolverExtensions
    {
        public static IViewModelResolver ApplyDefaultConfigure(this IViewModelResolver @this)
        {
            @this
                .IfInheritsFrom<ViewModelBase>((view, viewModel) =>
                {
                    viewModel.Dispatcher = view.Dispatcher;
                })
                .IfInheritsFrom<IAwareViewLoadedAndUnloaded>((view, viewModel) =>
                {
                    view.Loaded += (sender, e) => viewModel.OnLoaded(sender);
                    view.Unloaded += (sender, e) => viewModel.OnUnloaded(sender);
                })
                .IfInheritsFrom<ILocalizable>((view, viewModel) =>
                {
                    view.Loaded += (sender, args) => LanguageManager.Instance.CurrentUICultureChanged += viewModel.OnCurrentUICultureChanged;
                    view.Unloaded += (sender, args) => LanguageManager.Instance.CurrentUICultureChanged -= viewModel.OnCurrentUICultureChanged;
                });

            return @this;
        }
    }
}
