using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;

namespace Accelerider.Windows.Controls
{
    [ContentProperty("Views")]
    public class SwitchableViewContainer : UserControl
    {
        public static readonly DependencyProperty ViewsProperty = DependencyProperty.Register("Views", typeof(ObservableCollection<FrameworkElement>), typeof(SwitchableViewContainer), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectorProperty = DependencyProperty.Register("Selector", typeof(Selector), typeof(SwitchableViewContainer), new PropertyMetadata(null, OnSelectorChanged));


        public ObservableCollection<FrameworkElement> Views
        {
            get { return (ObservableCollection<FrameworkElement>)GetValue(ViewsProperty); }
            set { SetValue(ViewsProperty, value); }
        }
        public Selector Selector
        {
            get { return (Selector)GetValue(SelectorProperty); }
            set { SetValue(SelectorProperty, value); }
        }


        public SwitchableViewContainer() : base()
        {
            Views = new ObservableCollection<FrameworkElement>();
        }


        private static void OnSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is SwitchableViewContainer control)) return;
            var style = new Style();
            for (int i = 0; i < control.Selector.Items.Count; i++)
            {
                var trigger = new DataTrigger
                {
                    Binding = new Binding
                    {
                        Path = new PropertyPath("SelectedIndex"),
                        Source = control.Selector
                    },
                    Value = i
                };
                trigger.Setters.Add(new Setter
                {
                    Property = ContentProperty,
                    Value = new Binding
                    {
                        Path = new PropertyPath($"Views[{i}]"),
                        Source = control
                    }
                });
                style.Triggers.Add(trigger);
            }
            control.Style = style;
        }
    }
}
