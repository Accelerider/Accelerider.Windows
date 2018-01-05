using System.Windows;
using System.Windows.Input;

namespace Accelerider.Windows.Resources.Controls
{
    /// <summary>
    /// Interaction logic for SearchBar.xaml
    /// </summary>
    public partial class SearchBar
    {
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register("SearchText", typeof(string), typeof(SearchBar), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty SearchCommandProperty = DependencyProperty.Register("SearchCommand", typeof(ICommand), typeof(SearchBar), new PropertyMetadata(default(ICommand)));


        public string SearchText
        {
            get => (string) GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        public ICommand SearchCommand
        {
            get => (ICommand) GetValue(SearchCommandProperty);
            set => SetValue(SearchCommandProperty, value);
        }


        public SearchBar()
        {
            InitializeComponent();
        }
    }
}
