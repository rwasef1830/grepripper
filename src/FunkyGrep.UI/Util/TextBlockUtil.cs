using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FunkyGrep.UI.Util;

// Based on: https://blog.scottlogic.com/2011/01/31/automatically-showing-tooltips-on-a-trimmed-textblock-silverlight-wpf.html
public class TextBlockUtil
{
    public static readonly DependencyProperty HasAutoTooltipProperty = DependencyProperty.RegisterAttached(
        "HasAutoTooltip",
        typeof(bool),
        typeof(TextBlockUtil),
        new PropertyMetadata(false, OnAutoTooltipPropertyChanged));

    public static bool GetHasAutoTooltip(DependencyObject obj)
    {
        return (bool)obj.GetValue(HasAutoTooltipProperty);
    }

    public static void SetHasAutoTooltip(DependencyObject obj, bool value)
    {
        obj.SetValue(HasAutoTooltipProperty, value);
    }

    static void OnAutoTooltipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBlock textBlock)
        {
            return;
        }

        if (e.NewValue.Equals(true))
        {
            textBlock.TextTrimming = TextTrimming.WordEllipsis;
            ComputeAutoTooltip(textBlock);
            textBlock.SizeChanged += HandleTextBlockSizeChanged;
        }
        else
        {
            textBlock.SizeChanged -= HandleTextBlockSizeChanged;
            ClearToolTip(textBlock);
        }
    }

    static void HandleTextBlockSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is TextBlock textBlock)
        {
            ComputeAutoTooltip(textBlock);
        }
    }

    static void ComputeAutoTooltip(TextBlock textBlock)
    {
        textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        var width = textBlock.DesiredSize.Width;

        if (textBlock.ActualWidth < width)
        {
            BindingOperations.SetBinding(
                textBlock,
                FrameworkElement.ToolTipProperty,
                new Binding(nameof(textBlock.Text)) { Source = textBlock, Mode = BindingMode.OneWay });
        }
        else
        {
            ClearToolTip(textBlock);
        }
    }

    static void ClearToolTip(FrameworkElement textBlock)
    {
        BindingOperations.ClearBinding(textBlock, FrameworkElement.ToolTipProperty);
        textBlock.ToolTip = null;
    }
}
