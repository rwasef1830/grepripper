using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using GrepRipper.UI.ViewModels;

namespace GrepRipper.UI.Converters;

[ValueConversion(typeof(SearchResultItem), typeof(string))]
public class SearchResultItemToTextConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not SearchResultItem item)
        {
            return null;
        }

        var requiredCapacity = item.Match.PreMatchLines.Sum(x => x.Length)
                               + Environment.NewLine.Length * item.Match.PreMatchLines.Count
                               + item.Match.Line.Length
                               + Environment.NewLine.Length
                               + item.Match.PostMatchLines.Sum(x => x.Length)
                               + Environment.NewLine.Length * item.Match.PostMatchLines.Count;

        var builder = new StringBuilder(requiredCapacity);
        var match = item.Match;

        if (match.PreMatchLines is { Count: > 0 })
        {
            foreach (var preMatchLine in match.PreMatchLines)
            {
                builder.AppendLine(preMatchLine);
            }
        }

        builder.AppendLine(match.Line);

        if (match.PostMatchLines is { Count: > 0 })
        {
            foreach (var postMatchLine in match.PostMatchLines)
            {
                builder.AppendLine(postMatchLine);
            }
        }

        builder.Length -= Environment.NewLine.Length;
        return builder.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
