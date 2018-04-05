using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Accelerider.Windows.Views.AppStore
{
    /// <summary>
    /// Interaction logic for AppCard.xaml
    /// </summary>
    public partial class AppCard
    {
        public static readonly DependencyProperty AppNameProperty = DependencyProperty.Register("AppName", typeof(string), typeof(AppCard), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty AppImageSourceProperty = DependencyProperty.Register("AppImageSource", typeof(ImageSource), typeof(AppCard), new PropertyMetadata(default(ImageSource)));
        public static readonly DependencyProperty AppDescriptionProperty = DependencyProperty.Register("AppDescription", typeof(string), typeof(AppCard), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty AppDownloadCountProperty = DependencyProperty.Register("AppDownloadCount", typeof(long), typeof(AppCard), new PropertyMetadata(default(long)));
        public static readonly DependencyProperty AppAuthorsProperty = DependencyProperty.Register("AppAuthors", typeof(string), typeof(AppCard), new PropertyMetadata(default(string)));


        public string AppName
        {
            get => (string) GetValue(AppNameProperty);
            set => SetValue(AppNameProperty, value);
        }

        public ImageSource AppImageSource
        {
            get => (ImageSource) GetValue(AppImageSourceProperty);
            set => SetValue(AppImageSourceProperty, value);
        }

        public string AppDescription
        {
            get => (string) GetValue(AppDescriptionProperty);
            set => SetValue(AppDescriptionProperty, value);
        }

        public long AppDownloadCount
        {
            get => (long) GetValue(AppDownloadCountProperty);
            set => SetValue(AppDownloadCountProperty, value);
        }

        public string AppAuthors
        {
            get => (string) GetValue(AppAuthorsProperty);
            set => SetValue(AppAuthorsProperty, value);
        }


        public AppCard()
        {
            InitializeComponent();
        }
    }
}
