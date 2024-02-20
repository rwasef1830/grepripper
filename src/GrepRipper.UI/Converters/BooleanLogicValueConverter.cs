using System;
using System.Globalization;
using System.Windows.Data;

namespace GrepRipper.UI.Converters;

public class BooleanLogicValueConverter : IMultiValueConverter
{
    public bool IsBooleanAnd { get; set; }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var current = true;
        foreach (var value in values)
        {
            if (value is bool booleanValue)
            {
                current = this.IsBooleanAnd ? current && booleanValue : current || booleanValue;
            }
            else
            {
                current = false;
                break;
            }
        }

        return current;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
