using MaterialDesignThemes.Wpf;
using System.Windows;

namespace Accelerider.Windows.Controls
{
    /// <summary>
    /// Interaction logic for SettingItem.xaml
    /// </summary>
    public partial class SettingItem
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(SettingItem), new PropertyMetadata(null));
        public static readonly DependencyProperty IconKindProperty = DependencyProperty.Register("IconKind", typeof(PackIconKind), typeof(SettingItem), new PropertyMetadata(default(PackIconKind)));


        public SettingItem()
        {
            InitializeComponent();
        }


        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public PackIconKind IconKind
        {
            get { return (PackIconKind)GetValue(IconKindProperty); }
            set { SetValue(IconKindProperty, value); }
        }


    }
}
