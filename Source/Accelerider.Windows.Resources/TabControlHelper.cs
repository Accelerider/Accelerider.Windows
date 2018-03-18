using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Accelerider.Windows.Resources
{
    public class TabControlHelper
    {
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
            tabControl.SelectionChanged += OnSelectionChanged;
            tabControl.Loaded += OnLoaded;

            _initializedTabControlSet.Add(tabControl.GetHashCode());
        }

        private static void OnLoaded(object sender, RoutedEventArgs e)
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
                var resourceDictionary = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/Accelerider.Windows.Resources;component/Styles/Accelerider.Styles.TabControl.xaml")
                };
                content.Triggers.Add(resourceDictionary["LeftToRightMovedEventTrigger"] as TriggerBase);
                content.Triggers.Add(resourceDictionary["RightToLeftMovedEventTrigger"] as TriggerBase);
            }
            tabControl.Loaded -= OnLoaded;
        }

        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count == 0 || e.AddedItems.Count == 0) return;

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
    }
}
