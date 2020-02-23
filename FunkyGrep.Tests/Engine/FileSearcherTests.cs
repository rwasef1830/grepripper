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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using FunkyGrep.Engine;
using Xunit;

namespace FunkyGrep.Tests.Engine
{
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class FileSearcherTests
    {
        [Fact]
        public void FileSearcher_FindsAMatch_FiresEvents()
        {
            const string fileName = "Test";
            const string fileContent = "Speedy thing goes in, speedy thing comes out!";

            IEnumerable<IDataSource> dataSources =
                MakeDataSourceList(new Dictionary<string, string> { { fileName, fileContent } });

            var fileSearcher = new FileSearcher(
                new Regex("speedy", RegexOptions.None),
                dataSources, 
                false);

            bool matchFoundFired = false;
            Exception assertionException = null;

            fileSearcher.MatchFound +=
                (sender, args) =>
                {
                    matchFoundFired = true;

                    try
                    {
                        args.FilePath.Should().Be(fileName);
                        args.Matches.Count().Should().Be(1);
                        args.Matches.First().Number.Should().Be(1);
                        args.Matches.First().Text.Should().Be(fileContent);
                    }
                    catch (Exception ex)
                    {
                        assertionException = ex;
                    }
                };

            bool completedFired = false;

            fileSearcher.Completed +=
                (sender, args) => { completedFired = true; };

            fileSearcher.Begin();
            fileSearcher.Wait();

            matchFoundFired.Should().BeTrue("Match found event should have fired.");
            assertionException.Should().BeNull("No exceptions should have been raised in the event.");
            completedFired.Should().BeTrue("Completion event should have fired.");
        }

        [Theory]
        [MemberData(nameof(LongLinesClampTestSource))]
        public void Long_lines_in_results_are_clamped_properly_around_match(
            int contextLength, 
            string searchPattern, 
            string textToSearch, 
            SearchMatch expectedResult)
        {
            IEnumerable<IDataSource> dataSources = MakeDataSourceList(textToSearch);
            var searcher = new FileSearcher(
                new Regex(Regex.Escape(searchPattern)),
                dataSources,
                false,
                contextLength);

            bool eventWasFired = false;
            Exception failedAssertion = null;

            searcher.MatchFound +=
                (sender, args) =>
                {
                    try
                    {
                        eventWasFired = true;
                        args.Matches.First().Should().BeEquivalentTo(expectedResult);
                    }
                    catch (Exception ex)
                    {
                        failedAssertion = ex;
                    }
                };

            searcher.Begin();
            searcher.Wait();

            eventWasFired.Should().BeTrue();
            failedAssertion.Should().BeNull();
        }

        public static IEnumerable<object[]> LongLinesClampTestSource()
        {
            const int maxContextLength = 10;

            // Tail excess no CRLF
            yield return new object[]
            {
                maxContextLength,
                "A",
                "ABBBBBBBBCDEF",
                new SearchMatch(1, "ABBBBBBBBC", 0, 1)
            };

            // Head excess no CRLF
            yield return new object[]
            {
                maxContextLength,
                "A",
                "BBBBBBBBACDEF",
                new SearchMatch(1, "BBBBBBBBAC", 8, 1)
            };

            // Tail excess with CRLF
            yield return new object[]
            {
                maxContextLength,
                "A",
                "\r\nABBBBBBBBCDEF\r\n",
                new SearchMatch(2, "ABBBBBBBBC", 0, 1)
            };

            // Head excess with CRLF
            yield return new object[]
            {
                maxContextLength,
                "A",
                "\r\nBBBBBBBBACDEF\r\n",
                new SearchMatch(2, "BBBBBBBBAC", 8, 1)
            };

            // Match at end of line
            yield return new object[]
            {
                maxContextLength,
                "ABC",
                "\r\nXXXXXXXABC\r\n",
                new SearchMatch(2, "XXXXXXXABC", 7, 3)
            };

            // Match at the beginning of line
            yield return new object[]
            {
                maxContextLength,
                "ABC",
                "\r\nABCXXXXXXX\r\n",
                new SearchMatch(2, "ABCXXXXXXX", 0, 3)
            };
        }

        /// <summary>
        /// Each element of the passed list represents the fake file contents.
        /// The file name is a randomly generated GUID.
        /// </summary>
        static IEnumerable<IDataSource> MakeDataSourceList(
            string file1Data,
            params string[] fileNData)
        {
            return
                MakeDataSourceList(
                    new[] { file1Data }.Union(fileNData)
                        .Select(
                            x =>
                                new KeyValuePair<string, string>(
                                    Guid.NewGuid().ToString(),
                                    x)));
        }

        /// <summary>
        /// The dictionary key is the fake file name.
        /// The dictionary value is the fake file contents.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        static IEnumerable<IDataSource> MakeDataSourceList(
            IEnumerable<KeyValuePair<string, string>> dictionary)
        {
            return dictionary?.Select(
                x => new TestDataSource(x.Key, x.Value, Encoding.UTF8));
        }
    }
}
