using System.Windows;
using System.Windows.Input;

namespace GrepRipper.UI.Util;

public static class FocusUtil
{
    public static readonly DependencyProperty IsFocusedProperty =
        DependencyProperty.RegisterAttached(
            "IsFocused",
            typeof(bool),
            typeof(FocusUtil),
            new UIPropertyMetadata(false, null, OnIsFocusedCoerceValue));

    public static bool GetIsFocused(DependencyObject obj)
    {
        return (bool) obj.GetValue(IsFocusedProperty);
    }

    public static void SetIsFocused(DependencyObject obj, bool value)
    {
        obj.SetValue(IsFocusedProperty, value);
    }

    static object OnIsFocusedCoerceValue(DependencyObject d, object baseValue)
    {
        var uie = (UIElement)d;
        if (!(bool)baseValue)
        {
            return false;
        }

        uie.Focus();
        Keyboard.Focus(uie);
        return true;
    }
}
