using System;
using System.Globalization;
using System.Windows.Data;
using FunkyGrep.UI.ViewModels;

namespace FunkyGrep.UI.Converters;

public class OpenFileInEditorParametersValueConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2 || values[0] is not EditorInfo editor || values[1] is not IFileItem fileItem)
        {
            return null;
        }

        return new OpenFileInEditorParameters(editor, fileItem);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
