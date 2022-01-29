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
using System.Linq;
using System.Text;
using System.Windows.Data;
using FunkyGrep.UI.ViewModels;

namespace FunkyGrep.UI.Converters
{
    [ValueConversion(typeof(SearchResultItem), typeof(string))]
    public class SearchResultItemToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SearchResultItem item))
            {
                return null;
            }

            var requiredCapacity = (item.Match.PreMatchLines?.Sum(x => x.Length) ?? 0)
                                   + Environment.NewLine.Length * (item.Match.PreMatchLines?.Count ?? 0)
                                   + item.Match.Line.Length
                                   + Environment.NewLine.Length
                                   + (item.Match.PostMatchLines?.Sum(x => x.Length) ?? 0)
                                   + Environment.NewLine.Length * (item.Match.PostMatchLines?.Count ?? 0);

            var builder = new StringBuilder(requiredCapacity);
            var match = item.Match;

            if (match.PreMatchLines != null && match.PreMatchLines.Count > 0)
            {
                foreach (var preMatchLine in match.PreMatchLines)
                {
                    builder.AppendLine(preMatchLine);
                }
            }

            builder.AppendLine(match.Line);

            if (match.PostMatchLines != null && match.PostMatchLines.Count > 0)
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
}
