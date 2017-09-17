using System;
using System.Windows;

namespace Accelerider.Windows.Controls
{
    /// <summary>
    /// Interaction logic for MessageCard.xaml
    /// </summary>
    public partial class MessageCard
    {
        public static readonly DependencyProperty HeadUriProperty = DependencyProperty.Register("HeadUri", typeof(Uri), typeof(MessageCard), new PropertyMetadata(default(Uri)));
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(MessageCard), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty AuthorProperty = DependencyProperty.Register("Author", typeof(string), typeof(MessageCard), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty DateProperty = DependencyProperty.Register("Date", typeof(DateTime), typeof(MessageCard), new PropertyMetadata(default(DateTime)));


        public MessageCard()
        {
            InitializeComponent();
        }


        public Uri HeadUri
        {
            get => (Uri) GetValue(HeadUriProperty);
            set => SetValue(HeadUriProperty, value);
        }

        public string Message
        {
            get => (string) GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public string Author
        {
            get => (string) GetValue(AuthorProperty);
            set => SetValue(AuthorProperty, value);
        }

        public DateTime Date
        {
            get => (DateTime) GetValue(DateProperty);
            set => SetValue(DateProperty, value);
        }
    }
}
