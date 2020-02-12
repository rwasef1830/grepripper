#region License
// Copyright (c) 2012 Raif Atef Wasef
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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FunkyGrep.Engine
{
    public class GlobExpression
    {
        const RegexOptions c_RegexOptions = RegexOptions.Singleline;

        static readonly char[] s_InvalidChars =
            Path.GetInvalidFileNameChars().Except(new[] { '?', '*' }).ToArray();

        readonly Regex _pattern;

        public GlobExpression(string pattern)
        {
            this._pattern = new Regex(
                MakeRegexPattern(pattern),
                c_RegexOptions);
        }

        static string MakeRegexPattern(string pattern)
        {
            ValidateGlobPattern(pattern);

            return "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";
        }

        static void ValidateGlobPattern(string pattern)
        {
            if (!IsValid(pattern))
            {
                throw new FormatException("Invalid pattern: '" + pattern + "'");
            }
        }

        public bool IsMatch(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            return fileName != null && this._pattern.IsMatch(fileName);
        }

        public static bool IsMatch(string filePath, string pattern)
        {
            return Regex.IsMatch(filePath, MakeRegexPattern(pattern), c_RegexOptions);
        }

        public static char[] GetInvalidChars()
        {
            return (char[])s_InvalidChars.Clone();
        }

        public static bool IsValid(string pattern)
        {
            return pattern.IndexOfAny(s_InvalidChars) == -1;
        }
    }
}
