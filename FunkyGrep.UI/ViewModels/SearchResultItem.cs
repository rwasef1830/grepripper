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
using System.ComponentModel.DataAnnotations;
using FunkyGrep.Engine;

namespace FunkyGrep.UI.ViewModels
{
    public class SearchResultItem
    {
        [Display(Name = "File")]
        public string FilePath { get; }
        
        [Display(Name = "Line")]
        public int LineNumber { get; }

        [Display(Name = "Text")]
        public string MatchedLine { get; }

        public SearchResultItem(string filePath, MatchedLine matchedLine)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));
            }

            if (matchedLine == null)
            {
                throw new ArgumentNullException(nameof(matchedLine));
            }

            this.FilePath = filePath;
            this.LineNumber = matchedLine.Number;
            this.MatchedLine = matchedLine.Text;
        }
    }
}
