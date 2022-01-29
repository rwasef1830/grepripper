using System;
using System.Globalization;
using System.Windows.Data;

namespace FunkyGrep.UI.Converters;

// https://stackoverflow.com/a/30731090/111830
public class RelationalValueConverter : IMultiValueConverter
{
    public enum RelationType
    {
        Gt, Lt, Gte, Lte, Eq, Neq
    }

    public RelationType Relation { get; set; }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2)
        {
            throw new ArgumentException(@"Must have two parameters", nameof(values));
        }

        if (values[0] is not IComparable v0 || values[1] is not IComparable v1)
        {
            throw new ArgumentException(@"Must arguments must be IComparible", nameof(values));
        }

        var r = v0.CompareTo(v1);

        return this.Relation switch
        {
            RelationType.Gt => r > 0,
            RelationType.Lt => r < 0,
            RelationType.Gte => r >= 0,
            RelationType.Lte => r <= 0,
            RelationType.Eq => r == 0,
            RelationType.Neq => r != 0,
            _ => throw new InvalidOperationException()
        };
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
