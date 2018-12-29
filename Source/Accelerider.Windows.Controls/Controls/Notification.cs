using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Accelerider.Windows.Controls
{
    public partial class Notification : Control, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(ImageSource), typeof(Notification), new PropertyMetadata(default(ImageSource)));
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof(string), typeof(Notification), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description", typeof(string), typeof(Notification), new PropertyMetadata(default(string)));

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }


        private ICommand _closeCommand;

        public ICommand CloseCommand
        {
            get => _closeCommand;
            set => SetProperty(ref _closeCommand, value);
        }

        static Notification()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Notification), new FrameworkPropertyMetadata(typeof(Notification)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }
    }
}
