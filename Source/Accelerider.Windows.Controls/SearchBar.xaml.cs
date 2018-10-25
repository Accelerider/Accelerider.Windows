﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Accelerider.Windows.Controls
{
    /// <summary>
    /// Interaction logic for SearchBar.xaml
    /// </summary>
    public partial class SearchBar
    {
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(nameof(SearchText), typeof(string), typeof(SearchBar), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty SearchResultsProperty = DependencyProperty.Register(nameof(SearchResults), typeof(IEnumerable<object>), typeof(SearchBar), new PropertyMetadata(null));
        public static readonly DependencyProperty SearchCommandProperty = DependencyProperty.Register(nameof(SearchCommand), typeof(ICommand), typeof(SearchBar), new PropertyMetadata(default(ICommand)));
        public static readonly DependencyProperty IsRealTimeModeProperty = DependencyProperty.Register(nameof(IsRealTimeMode), typeof(bool), typeof(SearchBar), new PropertyMetadata(false));
        public static readonly DependencyProperty SelectedSearchResultProperty = DependencyProperty.Register(nameof(SelectedSearchResult), typeof(object), typeof(SearchBar), new PropertyMetadata(null));
        public static readonly DependencyProperty SearchResultItemTemplateProperty = DependencyProperty.Register("SearchResultItemTemplate", typeof(DataTemplate), typeof(SearchBar), new PropertyMetadata(null));


        public SearchBar()
        {
            InitializeComponent();
            PART_SearchBox.IsKeyboardFocusedChanged += OnSearchBoxIsKeyboardFocusedChanged;
            PART_SearchResultsPopup.Closed += (sender, e) => SetValue(SelectedSearchResultProperty, null);
        }


        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        public IEnumerable<object> SearchResults
        {
            get => (IEnumerable<object>)GetValue(SearchResultsProperty);
            set => SetValue(SearchResultsProperty, value);
        }

        public ICommand SearchCommand
        {
            get => (ICommand)GetValue(SearchCommandProperty);
            set => SetValue(SearchCommandProperty, value);
        }

        public bool IsRealTimeMode
        {
            get => (bool)GetValue(IsRealTimeModeProperty);
            set => SetValue(IsRealTimeModeProperty, value);
        }

        public object SelectedSearchResult
        {
            get => GetValue(SelectedSearchResultProperty);
            set => SetValue(SelectedSearchResultProperty, value);
        }

        public DataTemplate SearchResultItemTemplate
        {
            get => (DataTemplate)GetValue(SearchResultItemTemplateProperty);
            set => SetValue(SearchResultItemTemplateProperty, value);
        }


        private void PART_SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = PART_SearchBox.Text;
            if (IsRealTimeMode &&
               !string.IsNullOrEmpty(text) &&
               SearchCommand.CanExecute(text))
            {
                SearchCommand.Execute(text);
            }
        }

        private void OnSearchBoxIsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && PART_SearchBox.IsMouseOver)
            {
                PART_SearchResultsPopup.IsOpen = true;
                PART_SearchResultsPopup.StaysOpen = true;
            }
            else
            {
                if (!PART_SearchResultsPopup.IsMouseOver)
                    PART_SearchBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));

                 PART_SearchResultsPopup.StaysOpen = false;
            }
        }
    }
}
