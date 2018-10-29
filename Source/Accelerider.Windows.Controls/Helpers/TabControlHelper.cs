using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Accelerider.Windows.Infrastructure.Mvvm;

namespace Accelerider.Windows.Controls
{
    public class TabControlHelper
    {
        #region Switch Aware

        public static readonly DependencyProperty AwareSelectionChangedProperty = DependencyProperty.RegisterAttached(
            "AwareSelectionChanged", typeof(bool), typeof(TabControlHelper), new PropertyMetadata(default(bool), OnAwareSelectionChangedChanged));

        public static void SetAwareSelectionChanged(DependencyObject element, bool value)
        {
            element.SetValue(AwareSelectionChangedProperty, value);
        }

        public static bool GetAwareSelectionChanged(DependencyObject element)
        {
            return (bool)element.GetValue(AwareSelectionChangedProperty);
        }

        private static void OnAwareSelectionChangedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)d;

            if ((bool)e.NewValue)
                frameworkElement.Loaded += OnLoaded;
            else
                frameworkElement.Loaded -= OnLoaded;
        }

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            var contentControl = (ContentControl)sender;
            var tab = contentControl.TryFindParent<TabControl>();

            if (tab == null) return;

            tab.SelectionChanged -= OnSelectionChanged;
            tab.SelectionChanged += OnSelectionChanged;
        }

        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource != sender) return;

            var toTabItem = e.AddedItems.Cast<FrameworkElement>().SingleOrDefault();
            var fromTabItem = e.RemovedItems.Cast<FrameworkElement>().SingleOrDefault();

            if (toTabItem == null || fromTabItem == null) return;

            if (GetAwareSelectionChanged(fromTabItem))
                (fromTabItem.DataContext as IAwareTabItemSelectionChanged)?.OnUnselected();

            if (GetAwareSelectionChanged(toTabItem))
                (toTabItem.DataContext as IAwareTabItemSelectionChanged)?.OnSelected();
        }

        #endregion

        #region Switch Animation

        private const string LeftToRightMovedEventTriggerResourceKey = "LeftToRightMovedEventTrigger";
        private const string RightToLeftMovedEventTriggerResourceKey = "RightToLeftMovedEventTrigger";

        private static readonly Uri TabControlResourceDictionaryUri = new Uri("pack://application:,,,/Accelerider.Windows.Controls;component/Styles/Accelerider.Styles.TabControl.xaml");

        public static readonly DependencyProperty LeftToRightAnimationProperty = DependencyProperty.RegisterAttached("LeftToRightAnimation", typeof(Storyboard), typeof(TabControlHelper), new PropertyMetadata(null, OnLeftToRightAnimationChanged));
        public static Storyboard GetLeftToRightAnimation(DependencyObject obj)
        {
            return (Storyboard)obj.GetValue(LeftToRightAnimationProperty);
        }
        public static void SetLeftToRightAnimation(DependencyObject obj, Storyboard value)
        {
            obj.SetValue(LeftToRightAnimationProperty, value);
        }

        public static readonly DependencyProperty RightToLeftAnimationProperty = DependencyProperty.RegisterAttached("RightToLeftAnimation", typeof(Storyboard), typeof(TabControlHelper), new PropertyMetadata(null, OnRightToLeftAnimationChanged));
        public static Storyboard GetRightToLeftAnimation(DependencyObject obj)
        {
            return (Storyboard)obj.GetValue(RightToLeftAnimationProperty);
        }
        public static void SetRightToLeftAnimation(DependencyObject obj, Storyboard value)
        {
            obj.SetValue(RightToLeftAnimationProperty, value);
        }


        public static readonly RoutedEvent LeftToRightMovedEvent = EventManager.RegisterRoutedEvent("LeftToRightMoved", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(TabControlHelper));
        public static void AddLeftToRightMovedHandler(DependencyObject obj, RoutedEventHandler value)
        {
            if (obj is UIElement element)
            {
                element.AddHandler(LeftToRightMovedEvent, value);
            }
        }
        public static void RemoveLeftToRightMovedHandler(DependencyObject obj, RoutedEventHandler value)
        {
            if (obj is UIElement element)
            {
                element.RemoveHandler(LeftToRightMovedEvent, value);
            }
        }

        public static readonly RoutedEvent RightToLeftMovedEvent = EventManager.RegisterRoutedEvent("RightToLeftMoved", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(TabControlHelper));
        public static void AddRightToLeftMovedHandler(DependencyObject obj, RoutedEventHandler value)
        {
            if (obj is UIElement element)
            {
                element.AddHandler(RightToLeftMovedEvent, value);
            }
        }
        public static void RemoveRightToLeftMovedHandler(DependencyObject obj, RoutedEventHandler value)
        {
            if (obj is UIElement element)
            {
                element.RemoveHandler(RightToLeftMovedEvent, value);
            }
        }


        private static readonly HashSet<int> _initializedTabControlSet = new HashSet<int>();

        private static void OnLeftToRightAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabControl tabControl &&
                !_initializedTabControlSet.Contains(tabControl.GetHashCode()))
            {
                ModifyTabControl(tabControl);
            }
        }

        private static void OnRightToLeftAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabControl tabControl &&
                !_initializedTabControlSet.Contains(tabControl.GetHashCode()))
            {
                ModifyTabControl(tabControl);
            }
        }

        private static void ModifyTabControl(Selector tabControl)
        {
            tabControl.SelectionChanged += OnSelectionChangedForSwitchAnimation;
            tabControl.Loaded += OnLoadedForSwitchAnimation;

            _initializedTabControlSet.Add(tabControl.GetHashCode());
        }

        private static void OnLoadedForSwitchAnimation(object sender, RoutedEventArgs e)
        {
            if (!(sender is TabControl tabControl)) return;

            foreach (TabItem tabItem in tabControl.Items)
            {
                if (!(tabItem.Content is FrameworkElement content)) break;

                // 1. Sets TranslateTransform instance to RenderTransform Property if it does not exist.
                switch (content.RenderTransform)
                {
                    case TranslateTransform _:
                        break;
                    case TransformGroup group:
                        if (group.Children.FirstOrDefault(item => item is TranslateTransform) == null)
                            group.Children.Add(new TranslateTransform());
                        break;
                    case MatrixTransform _:
                        content.RenderTransform = new TranslateTransform();
                        break;
                    default:
                        throw new Exception("the RenderTransform property already has a value, and the value is not TranslateTransform type.");
                }

                // 2. Add animation to EventTrigger property.
                var resourceDictionary = new ResourceDictionary { Source = TabControlResourceDictionaryUri };
                content.Triggers.Add(resourceDictionary[LeftToRightMovedEventTriggerResourceKey] as TriggerBase);
                content.Triggers.Add(resourceDictionary[RightToLeftMovedEventTriggerResourceKey] as TriggerBase);
            }
            tabControl.Loaded -= OnLoaded;
        }

        private static void OnSelectionChangedForSwitchAnimation(object sender, SelectionChangedEventArgs e)
        {
            if (sender != e.OriginalSource || e.RemovedItems.Count == 0 || e.AddedItems.Count == 0) return;

            var fromTabItem = e.RemovedItems.Cast<TabItem>().Single();
            var toTabItem = e.AddedItems.Cast<TabItem>().Single();
            if (sender is TabControl tabControl &&
                toTabItem.Content is UIElement content)
            {
                content.RaiseEvent(
                    tabControl.Items.IndexOf(fromTabItem) > tabControl.Items.IndexOf(toTabItem)
                    ? new RoutedEventArgs(RightToLeftMovedEvent, content)
                    : new RoutedEventArgs(LeftToRightMovedEvent, content));
            }
        }

        #endregion
    }
}
