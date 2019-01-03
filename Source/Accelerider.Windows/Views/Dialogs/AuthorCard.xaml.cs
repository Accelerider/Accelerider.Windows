using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Accelerider.Windows.Infrastructure;

namespace Accelerider.Windows.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for AuthorCard.xaml
    /// </summary>
    public partial class AuthorCard
    {
        public static readonly DependencyProperty AvatarProperty = DependencyProperty.Register(
       "Avatar", typeof(ImageSource), typeof(AuthorCard), new PropertyMetadata(default(ImageSource)));
        public static readonly DependencyProperty AuthorNameProperty = DependencyProperty.Register(
            "AuthorName", typeof(string), typeof(AuthorCard), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description", typeof(string), typeof(AuthorCard), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty EmailProperty = DependencyProperty.Register(
            "Email", typeof(string), typeof(AuthorCard), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty GithubHomeProperty = DependencyProperty.Register(
            "GithubHome", typeof(string), typeof(AuthorCard), new PropertyMetadata(default(string)));


        public AuthorCard()
        {
            OpenGithubHomeCommand = GetCommand(() => GithubHome);
            OpenEmailCommand = GetCommand(() => Email);

            InitializeComponent();
        }

        private RelayCommand GetCommand(Func<string> urlGetter)
        {
            return new RelayCommand(() =>
            {
                var url = urlGetter();
                if (!string.IsNullOrWhiteSpace(url)) Process.Start(url);
            });
        }

        public ImageSource Avatar
        {
            get => (ImageSource)GetValue(AvatarProperty);
            set => SetValue(AvatarProperty, value);
        }

        public string AuthorName
        {
            get => (string)GetValue(AuthorNameProperty);
            set => SetValue(AuthorNameProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public string Email
        {
            get => (string)GetValue(EmailProperty);
            set => SetValue(EmailProperty, value);
        }

        public string GithubHome
        {
            get => (string)GetValue(GithubHomeProperty);
            set => SetValue(GithubHomeProperty, value);
        }

        public ICommand OpenGithubHomeCommand { get; }

        public ICommand OpenEmailCommand { get; }
    }
}
