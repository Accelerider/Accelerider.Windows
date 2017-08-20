using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Accelerider.Windows.Controls
{
    [ContentProperty("LoadedContent")]
    public class SwitchLoadingViewContainer : UserControl
    {
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(SwitchLoadingViewContainer), new PropertyMetadata(false));
        public static readonly DependencyProperty LoadedContentProperty = DependencyProperty.Register("LoadedContent", typeof(object), typeof(SwitchLoadingViewContainer), new PropertyMetadata(null));

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public object LoadedContent
        {
            get => GetValue(LoadedContentProperty);
            set => SetValue(LoadedContentProperty, value);
        }
    }
}
