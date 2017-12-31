using System.Windows;
using MaterialDesignThemes.Wpf;

namespace Accelerider.Windows.Resources.Controls
{
    /// <summary>
    /// Interaction logic for ToolBarButton.xaml
    /// </summary>
    public partial class ToolBarButton
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(ToolBarButton), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty IconKindProperty = DependencyProperty.Register("IconKind", typeof(PackIconKind), typeof(ToolBarButton), new PropertyMetadata(default(PackIconKind)));

        public ToolBarButton()
        {
            InitializeComponent();
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public PackIconKind IconKind
        {
            get => (PackIconKind)GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }
    }
}
