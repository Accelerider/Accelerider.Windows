﻿using System;
using System.Linq;
using System.Windows;
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

        public IViewModelResolver IfInheritsFrom<TViewModel>(Action<FrameworkElement, TViewModel> configuration)
        {
            return IfInheritsFrom<FrameworkElement, TViewModel>(configuration);
        }

        public IViewModelResolver IfInheritsFrom<TView, TViewModel>(Action<TView, TViewModel> configuration)
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

        public IViewModelResolver IfInheritsFrom(Type genericInterfaceType, Action<IGenericInterface, FrameworkElement, object> configuration)
        {
            var previousAction = _configureViewAndViewModel;
            _configureViewAndViewModel = (view, viewModel) =>
            {
                previousAction?.Invoke(view, viewModel);
                var interfaceInstance = viewModel.AsGenericInterface(typeof(IAwareViewLoadedAndUnloaded<>));
                if (interfaceInstance != null)
                {
                    configuration?.Invoke(interfaceInstance, view, viewModel);
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
                    view.Loaded += (sender, e) => viewModel.OnLoaded();
                    view.Unloaded += (sender, e) => viewModel.OnUnloaded();
                })
                .IfInheritsFrom(typeof(IAwareViewLoadedAndUnloaded<>), (interfaceInstance, view, viewModel) =>
                {
                    var viewType = view.GetType();
                    if (interfaceInstance.GenericArguments.Single() != viewType)
                    {
                        throw new InvalidOperationException();
                    }

                    var onLoadedMethod = interfaceInstance.GetMethod<Action<object>>("OnLoaded", viewType);
                    var onUnloadedMethod = interfaceInstance.GetMethod<Action<object>>("OnUnloaded", viewType);

                    view.Loaded += (sender, args) => onLoadedMethod(sender);
                    view.Unloaded += (sender, args) => onUnloadedMethod(sender);
                })
                .IfInheritsFrom<ILocalizable>((view, viewModel) =>
                {
                    viewModel.I18nManager = I18nManager.Instance;
                    view.Loaded += (sender, args) => I18nManager.Instance.CurrentUICultureChanged += viewModel.OnCurrentUICultureChanged;
                    view.Unloaded += (sender, args) => I18nManager.Instance.CurrentUICultureChanged -= viewModel.OnCurrentUICultureChanged;
                });
                //.IfInheritsFrom<IAwareTabItemSelectionChanged>((view, viewModel) =>
                //{
                //    TabControlHelper.SetAwareSelectionChanged(view, true);
                //});

            return @this;
        }
    }
}
