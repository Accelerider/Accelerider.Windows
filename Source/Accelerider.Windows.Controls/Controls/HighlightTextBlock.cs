using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Accelerider.Windows.Controls
{
    public class HighlightTextBlock : TextBlock
    {
        public static readonly DependencyProperty SourceTextProperty = DependencyProperty.Register("SourceText", typeof(string), typeof(HighlightTextBlock), new PropertyMetadata(null, OnSourceTextChanged));
        public static readonly DependencyProperty QueriesTextProperty = DependencyProperty.Register("QueriesText", typeof(string), typeof(HighlightTextBlock), new PropertyMetadata(null, OnQueriesTextChanged));
        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register("HighlightBrush", typeof(Brush), typeof(HighlightTextBlock), new PropertyMetadata(Brushes.Yellow));

        private static void OnSourceTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((HighlightTextBlock)d).RefreshInlines();

        private static void OnQueriesTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((HighlightTextBlock)d).RefreshInlines();


        public string SourceText
        {
            get => (string)GetValue(SourceTextProperty);
            set => SetValue(SourceTextProperty, value);
        }

        public string QueriesText
        {
            get => (string)GetValue(QueriesTextProperty);
            set => SetValue(QueriesTextProperty, value);
        }

        public Brush HighlightBrush
        {
            get => (Brush)GetValue(HighlightBrushProperty);
            set => SetValue(HighlightBrushProperty, value);
        }


        private void RefreshInlines()
        {
            Inlines.Clear();

            if (string.IsNullOrEmpty(SourceText)) return;
            if (string.IsNullOrEmpty(QueriesText))
            {
                Inlines.Add(SourceText);
                return;
            }

            var sourceText = SourceText;
            var queries = QueriesText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var intervals = from query in queries.Distinct()
                            from interval in GetQueryIntervals(sourceText, query)
                            select interval;
            var mergedIntervals = MergeIntervals(intervals.ToList());
            var fragments = SplitTextByOrderedDisjointIntervals(sourceText, mergedIntervals);

            Inlines.AddRange(GenerateRunElement(fragments));
        }

        private IEnumerable GenerateRunElement(IEnumerable<(string fragment, bool isQuery)> fragments)
        {
            return from item in fragments
                   select item.isQuery
                   ? GetHighlightRun(item.fragment)
                   : new Run(item.fragment);
        }

        private Run GetHighlightRun(string highlightText)
        {
            var run = new Run(highlightText);
            run.SetBinding(TextElement.BackgroundProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(HighlightBrush)),
                Mode = BindingMode.OneWay
            });
            return run;
        }

        private static IEnumerable<(string fragment, bool isQuery)> SplitTextByOrderedDisjointIntervals(string sourceText, List<(int start, int end)> mergedIntervals)
        {
            if (string.IsNullOrEmpty(sourceText)) yield break;

            if (!mergedIntervals?.Any() ?? true)
            {
                yield return (sourceText, false);
                yield break;
            }

            (int start0, int end0) = mergedIntervals.First();

            if (start0 > 0) yield return (sourceText.Substring(0, start0), false);
            yield return (sourceText.Substring(start0, end0 - start0), true);
              
            int previousEnd = end0;
            foreach ((int start, int end) in mergedIntervals.Skip(1))
            {
                yield return (sourceText.Substring(previousEnd, start - previousEnd), false);
                yield return (sourceText.Substring(start, end - start), true);
                previousEnd = end;
            }

            if (previousEnd < sourceText.Length)
                yield return (sourceText.Substring(previousEnd), false);
        }

        private static List<(int start, int end)> MergeIntervals(List<(int start, int end)> intervals)
        {
            if (!intervals?.Any() ?? true) return new List<(int, int)>();

            intervals.Sort((x, y) => x.start != y.start ? x.start - y.start : x.end - y.end);

            (int startPointer, int endPointer) = intervals[0];

            var result = new List<(int, int)>();
            foreach ((int start, int end) in intervals.Skip(1))
            {
                if (start <= endPointer)
                {
                    if (endPointer < end)
                    {
                        endPointer = end;
                    }
                }
                else
                {
                    result.Add((startPointer, endPointer));
                    (startPointer, endPointer) = (start, end);
                }
            }
            result.Add((startPointer, endPointer));
            return result;
        }

        private static IEnumerable<(int start, int end)> GetQueryIntervals(string sourceText, string query)
        {
            if (string.IsNullOrEmpty(sourceText) || string.IsNullOrEmpty(query)) yield break;

            int nextStartIndex = 0;
            while (nextStartIndex < sourceText.Length)
            {
                int index = sourceText.IndexOf(query, nextStartIndex, StringComparison.CurrentCultureIgnoreCase);

                if (index == -1) yield break;

                nextStartIndex = index + query.Length;
                yield return (index, nextStartIndex);
            }
        }
    }
}
