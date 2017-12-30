using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Input;

namespace Accelerider.Windows.Resources.Controls
{
    /// <summary>
    /// Interaction logic for SettingItem.xaml
    /// </summary>
    public partial class SettingItem : ICommandSource
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(SettingItem), new PropertyMetadata(null));
        public static readonly DependencyProperty IconKindProperty = DependencyProperty.Register("IconKind", typeof(PackIconKind), typeof(SettingItem), new PropertyMetadata(default(PackIconKind)));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(SettingItem), new PropertyMetadata(default(ICommand)));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(SettingItem), new PropertyMetadata(default(object)));
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(SettingItem), new PropertyMetadata(default(IInputElement)));


        public SettingItem()
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

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => (object)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public IInputElement CommandTarget
        {
            get => (IInputElement)GetValue(CommandTargetProperty);
            set => SetValue(CommandTargetProperty, value);
        }

        private void SettingItem_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Command?.CanExecute(CommandParameter) ?? false) Command.Execute(CommandParameter);
        }
    }
}
