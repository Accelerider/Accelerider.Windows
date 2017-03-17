using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BaiduPanDownloadWpf.Controls
{
    /// <summary>
    /// Interaction logic for AsyncButton.xaml
    /// </summary>
    public partial class AsyncButton : UserControl
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(AsyncButton), new PropertyMetadata(null));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(AsyncButton), new PropertyMetadata(null));
        public static readonly DependencyProperty IsWorkingProperty = DependencyProperty.Register("IsWorking", typeof(bool), typeof(AsyncButton), new PropertyMetadata(false));
        public static readonly DependencyProperty ProgressBarStyleProperty = DependencyProperty.Register("ProgressBarStyle", typeof(Style), typeof(AsyncButton), new PropertyMetadata(null));
        public static readonly DependencyProperty ButtonTipProperty = DependencyProperty.Register("ButtonTip", typeof(string), typeof(AsyncButton), new PropertyMetadata(null));
        public static readonly DependencyProperty IconDataProperty = DependencyProperty.Register("IconData", typeof(Geometry), typeof(AsyncButton), new PropertyMetadata(null));
        public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register("IconWidth", typeof(double), typeof(AsyncButton), new PropertyMetadata(14.0));
        public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register("IconHeight", typeof(double), typeof(AsyncButton), new PropertyMetadata(14.0));
        public static readonly DependencyProperty ProgressRingWidthProperty = DependencyProperty.Register("ProgressRingWidth", typeof(double), typeof(AsyncButton), new PropertyMetadata(25.0));
        public static readonly DependencyProperty ProgressRingHeightProperty = DependencyProperty.Register("ProgressRingHeight", typeof(double), typeof(AsyncButton), new PropertyMetadata(25.0));
        public static readonly DependencyProperty IsAutoHideButtonProperty = DependencyProperty.Register("IsAutoHideButton", typeof(bool), typeof(AsyncButton), new PropertyMetadata(true));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }


        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
        public bool IsWorking
        {
            get { return (bool)GetValue(IsWorkingProperty); }
            set { SetValue(IsWorkingProperty, value); }
        }
        public Style ProgressBarStyle
        {
            get { return (Style)GetValue(ProgressBarStyleProperty); }
            set { SetValue(ProgressBarStyleProperty, value); }
        }
        public string ButtonTip
        {
            get { return (string)GetValue(ButtonTipProperty); }
            set { SetValue(ButtonTipProperty, value); }
        }
        public Geometry IconData
        {
            get { return (Geometry)GetValue(IconDataProperty); }
            set { SetValue(IconDataProperty, value); }
        }
        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }
        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }
        public double ProgressRingWidth
        {
            get { return (double)GetValue(ProgressRingWidthProperty); }
            set { SetValue(ProgressRingWidthProperty, value); }
        }
        public double ProgressRingHeight
        {
            get { return (double)GetValue(ProgressRingHeightProperty); }
            set { SetValue(ProgressRingHeightProperty, value); }
        }
        public bool IsAutoHideButton
        {
            get { return (bool)GetValue(IsAutoHideButtonProperty); }
            set { SetValue(IsAutoHideButtonProperty, value); }
        }

        public AsyncButton()
        {
            InitializeComponent();
        }
    }
}
