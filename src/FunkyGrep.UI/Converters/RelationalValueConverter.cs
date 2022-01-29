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
using System.Windows.Data;

namespace FunkyGrep.UI.Converters
{
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

            if (!(values[0] is IComparable v0) || !(values[1] is IComparable v1))
            {
                throw new ArgumentException(@"Must arguments must be IComparible", nameof(values));
            }

            var r = v0.CompareTo(v1);

            return this.Relation switch
            {
                RelationType.Gt => (r > 0),
                RelationType.Lt => (r < 0),
                RelationType.Gte => (r >= 0),
                RelationType.Lte => (r <= 0),
                RelationType.Eq => (r == 0),
                RelationType.Neq => (r != 0),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
