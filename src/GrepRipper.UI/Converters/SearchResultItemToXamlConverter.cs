using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using GrepRipper.UI.ViewModels;

namespace GrepRipper.UI.Converters;

[ValueConversion(typeof(SearchResultItem), typeof(TextBlock))]
public class SearchResultItemToXamlConverter : DependencyObject, IValueConverter
{
    public static readonly DependencyProperty ContextRunStyleProperty = DependencyProperty.Register(
        nameof(ContextRunStyle),
        typeof(Style),
        typeof(SearchResultItemToXamlConverter),
        new PropertyMetadata(),
        value => value == null || ((Style)value).TargetType == typeof(Run));

    public static readonly DependencyProperty MatchRunStyleProperty = DependencyProperty.Register(
        nameof(MatchRunStyle),
        typeof(Style),
        typeof(SearchResultItemToXamlConverter),
        new PropertyMetadata(),
        value => value == null || ((Style)value).TargetType == typeof(Run));

    public Style ContextRunStyle
    {
        get => (Style)this.GetValue(ContextRunStyleProperty);
        set => this.SetValue(ContextRunStyleProperty, value);
    }

    public Style MatchRunStyle
    {
        get => (Style)this.GetValue(MatchRunStyleProperty);
        set => this.SetValue(MatchRunStyleProperty, value);
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SearchResultItem item)
        {
            return null;
        }

        var textBlock = new TextBlock();

        // Workaround https://github.com/dotnet/wpf/issues/2681
        textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

        var match = item.Match;

        if (match.PreMatchLines is { Count: > 0 })
        {
            foreach (var preMatchLine in match.PreMatchLines)
            {
                var preMatchLineRun = new Run(preMatchLine)
                {
                    Style = this.ContextRunStyle
                };
                textBlock.Inlines.Add(preMatchLineRun);
                textBlock.Inlines.Add(new LineBreak());
            }
        }

        if (match.MatchIndex > 0)
        {
            var contextBeforeText = match.Line[..match.MatchIndex];
            var contextBeforeRun = new Run(contextBeforeText)
            {
                Style = this.ContextRunStyle
            };
            textBlock.Inlines.Add(contextBeforeRun);
        }

        var matchText = match.Line.Substring(match.MatchIndex, match.MatchLength);
        var matchRun = new Run(matchText)
        {
            Style = this.MatchRunStyle
        };
        textBlock.Inlines.Add(matchRun);

        var contextAfterLength = match.Line.Length - match.MatchIndex - match.MatchLength;
        if (contextAfterLength > 0)
        {
            var contextAfterText = match.Line.Substring(match.MatchIndex + match.MatchLength, contextAfterLength);
            var contextAfterRun = new Run(contextAfterText)
            {
                Style = this.ContextRunStyle
            };
            textBlock.Inlines.Add(contextAfterRun);
        }

        if (match.PostMatchLines is not { Count: > 0 })
        {
            return textBlock;
        }

        foreach (var postMatchLine in match.PostMatchLines)
        {
            textBlock.Inlines.Add(new LineBreak());
            var postMatchLineRun = new Run(postMatchLine)
            {
                Style = this.ContextRunStyle
            };
            textBlock.Inlines.Add(postMatchLineRun);
        }

        return textBlock;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
