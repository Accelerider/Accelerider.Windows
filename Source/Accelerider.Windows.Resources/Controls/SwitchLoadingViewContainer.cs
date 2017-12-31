namespace Accelerider.Windows.Resources.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    [ContentProperty("LoadedContent")]
    public class SwitchLoadingViewContainer : UserControl
    {
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(SwitchLoadingViewContainer), new PropertyMetadata(false));
        public static readonly DependencyProperty LoadingContentProperty = DependencyProperty.Register("LoadingContent", typeof(object), typeof(SwitchLoadingViewContainer), new PropertyMetadata(null));
        public static readonly DependencyProperty LoadedContentProperty = DependencyProperty.Register("LoadedContent", typeof(object), typeof(SwitchLoadingViewContainer), new PropertyMetadata(null));

        public bool IsLoading
        {
            get => (bool)this.GetValue(IsLoadingProperty);
            set => this.SetValue(IsLoadingProperty, value);
        }

        public object LoadingContent
        {
            get => this.GetValue(LoadingContentProperty);
            set => this.SetValue(LoadingContentProperty, value);
        }

        public object LoadedContent
        {
            get => this.GetValue(LoadedContentProperty);
            set => this.SetValue(LoadedContentProperty, value);
        }
    }
}
