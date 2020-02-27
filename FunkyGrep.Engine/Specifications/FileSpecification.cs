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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FunkyGrep.Engine.Specifications
{
    public class FileSpecification
    {
        readonly List<string> _fileExcludePatterns;
        readonly List<string> _filePatterns;
        readonly string _folderPath;
        readonly bool _includeSubfolders;

        public FileSpecification(
            string folderPath,
            bool includeSubfolders,
            IEnumerable<string> filePatterns,
            IEnumerable<string> fileExcludePatterns)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(folderPath));
            }

            this._folderPath = folderPath;
            this._includeSubfolders = includeSubfolders;

            if (filePatterns == null)
            {
                filePatterns = new[] { "*" };
            }

            this._filePatterns = filePatterns
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            this._fileExcludePatterns = (fileExcludePatterns ?? new string[0])
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }

        public IEnumerable<IDataSource> EnumerateFiles()
        {
            var searchOption = this._includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            // https://stackoverflow.com/questions/7585087/multithreaded-use-of-regex
            var filePatternExpressions = this._filePatterns
                .Select(x => new GlobExpression(x))
                .ToArray();

            IEnumerable<string> enumerator = Directory
                .EnumerateFiles(
                    this._folderPath,
                    "*",
                    new EnumerationOptions
                    {
                        IgnoreInaccessible = true,
                        MatchType = MatchType.Simple,
                        RecurseSubdirectories = searchOption == SearchOption.AllDirectories,
                        MatchCasing = MatchCasing.PlatformDefault,
                        ReturnSpecialDirectories = false
                    })
                .Where(x => filePatternExpressions.Any(e => e.IsMatch(x)));

            if (this._fileExcludePatterns.Count > 0)
            {
                var fileExcludePatternExpressions = this._fileExcludePatterns
                    .Select(x => new GlobExpression(x))
                    .ToArray();
                enumerator = enumerator.Where(x => !fileExcludePatternExpressions.Any(e => e.IsMatch(x)));
            }

            return enumerator.Select(x => new FileDataSource(x));
        }
    }
}
