using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Accelerider.Windows.Resources.Controls
{
    public class SwitchableViewContainer : UserControl
    {
        public static readonly DependencyProperty SelectorProperty = DependencyProperty.Register("Selector", typeof(Selector), typeof(SwitchableViewContainer), new PropertyMetadata(null, OnSelectorChanged));

        public SwitchableViewContainer()
        {
            Loaded += (sender, e) =>
            {
                if (Selector.SelectedIndex != -1)
                    Content = Resources[Selector.SelectedIndex.ToString()];
            };
        }

        public Selector Selector
        {
            get => (Selector)GetValue(SelectorProperty);
            set => SetValue(SelectorProperty, value);
        }

        private static void OnSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SwitchableViewContainer)d;
            control.Selector.SelectionChanged += (sender, eventArgs) =>
            {
                if (control.Selector.Items.Count == 0) return;
                control.Content = control.Resources[control.Selector.SelectedIndex.ToString()];
            };
        }
    }
}
