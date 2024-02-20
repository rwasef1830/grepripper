using System;
using System.Globalization;
using System.Windows.Data;
using GrepRipper.UI.ViewModels;

namespace GrepRipper.UI.Converters;

public class OpenFileInEditorParametersValueConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is not [EditorInfo editor, IFileItem fileItem])
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
