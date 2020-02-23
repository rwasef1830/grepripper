#region License
// Copyright (c) 2020 Raif Atef Wasef
// This source file is licensed under the  MIT license.
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom
// the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY
// KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT
// OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using FunkyGrep.Engine;
using FunkyGrep.UI.ViewModels;

namespace FunkyGrep.UI.Converters
{
    [ValueConversion(typeof(MatchedLine), typeof(object))]
    public class MatchedLineToXamlConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty ContextRunStyleProperty = DependencyProperty.Register(
            nameof(ContextRunStyle),
            typeof(Style),
            typeof(MatchedLineToXamlConverter),
            new PropertyMetadata(),
            value => value == null || ((Style)value).TargetType == typeof(Run));

        public static readonly DependencyProperty MatchRunStyleProperty = DependencyProperty.Register(
            nameof(MatchRunStyle),
            typeof(Style),
            typeof(MatchedLineToXamlConverter),
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

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SearchResultItem item))
            {
                throw new ArgumentException("Invalid value type.");
            }

            var textBlock = new TextBlock();

            if (item.MatchIndex > 0)
            {
                var contextBeforeText = item.MatchedLine.Substring(0, item.MatchIndex);
                var contextBeforeRun = new Run(contextBeforeText)
                {
                    Style = this.ContextRunStyle
                };
                textBlock.Inlines.Add(contextBeforeRun);
            }

            var matchText = item.MatchedLine.Substring(item.MatchIndex, item.MatchLength);
            var matchRun = new Run(matchText)
            {
                Style = this.MatchRunStyle
            };
            textBlock.Inlines.Add(matchRun);

            var contextAfterLength = item.MatchedLine.Length - item.MatchIndex - item.MatchLength;
            if (contextAfterLength > 0)
            {
                var contextAfterText = item.MatchedLine.Substring(item.MatchIndex + item.MatchLength, contextAfterLength);
                var contextAfterRun = new Run(
                    contextAfterText)
                {
                    Style = this.ContextRunStyle
                };
                textBlock.Inlines.Add(contextAfterRun);
            }

            return textBlock;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
