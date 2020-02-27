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
                false,
                0);

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
                        args.Matches.First().LineNumber.Should().Be(1);
                        args.Matches.First().Line.Should().Be(fileContent);
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

        public class MatchesWithLongLinesTests
        {
            const int c_MaxContextLength = 10;

            [Fact]
            public void Tail_excess_single_line()
            {
                this.DoTest(
                    "A",
                    "ABBBBBBBBCDEF",
                    0,
                    new SearchMatch(1, "ABBBBBBBBC", 0, 1, null, null));
            }

            [Fact]
            public void Head_excess_single_line()
            {
                this.DoTest(
                    "A",
                    "BBBBBBBBACDEF",
                    0,
                    new SearchMatch(1, "BBBBBACDEF", 5, 1, null, null));
            }

            [Fact]
            public void Tail_excess_multiple_lines()
            {
                this.DoTest(
                    "A",
                    "\r\nABBBBBBBBCDEF\r\n",
                    0,
                    new SearchMatch(2, "ABBBBBBBBC", 0, 1, null, null));
            }

            [Fact]
            public void Head_excess_multiple_lines()
            {
                this.DoTest(
                    "A",
                    "\r\nBBBBBBBBACDEF\r\n",
                    0,
                    new SearchMatch(2, "BBBBBACDEF", 5, 1, null, null));
            }

            [Fact]
            public void Match_at_end_of_line()
            {
                this.DoTest(
                    "ABC",
                    "\r\nXXXXXXXABC\r\n",
                    0,
                    new SearchMatch(2, "XXXXXXXABC", 7, 3, null, null));
            }

            [Fact]
            public void Match_at_start_of_line()
            {
                this.DoTest(
                    "ABC",
                    "\r\nABCXXXXXXX\r\n",
                    0,
                    new SearchMatch(2, "ABCXXXXXXX", 0, 3, null, null));
            }

            [Fact]
            public void Context_extraction_around_match()
            {
                this.DoTest(
                    "ABC",
                    "1234567890ABC12345",
                    0,
                    new SearchMatch(1, "7890ABC123", 4, 3, null, null));
            }

            [Fact]
            public void Match_larger_than_context_clamp_limit()
            {
                this.DoTest(
                    "123456789012345",
                    "XXXXXX123456789012345XXXXXX",
                    0,
                    new SearchMatch(1, "123456789012345", 0, 15, null, null));
            }

            [Fact]
            public void Context_lines_should_be_clamped()
            {
                this.DoTest(
                    "ABC",
                    "12345ABC9012345\r\n!!!!!!!!!!!ABC!!!!!!!!!\r\n______ABC_______\r\nXXXXXXABCX\r\nXXXXXXXXXXXXXABC",
                    2,
                    new SearchMatch(
                        1,
                        "2345ABC901",
                        4,
                        3,
                        null,
                        new[] { "!!!!!!!!!!", "______ABC_" }),
                    new SearchMatch(
                        2,
                        "!!!!ABC!!!",
                        4,
                        3,
                        new[] { "12345ABC90" },
                        new[] { "______ABC_", "XXXXXXABCX" }),
                    new SearchMatch(
                        3,
                        "____ABC___",
                        4,
                        3,
                        new[] { "12345ABC90", "!!!!!!!!!!" },
                        new[] { "XXXXXXABCX", "XXXXXXXXXX" }),
                    new SearchMatch(
                        4,
                        "XXXXXXABCX",
                        6,
                        3,
                        new[] { "!!!!!!!!!!", "______ABC_" },
                        new[] { "XXXXXXXXXX" }),
                    new SearchMatch(
                        5,
                        "XXXXXXXABC",
                        7,
                        3,
                        new[] { "______ABC_", "XXXXXXABCX" },
                        null));
            }

            void DoTest(
                string searchPattern,
                string textToSearch,
                int contextLineCount,
                params SearchMatch[] expectedResults)
            {
                IEnumerable<IDataSource> dataSources = MakeDataSourceList(textToSearch);
                var searcher = new FileSearcher(
                    new Regex(Regex.Escape(searchPattern)),
                    dataSources,
                    false,
                    contextLineCount,
                    c_MaxContextLength);

                using var monitor = searcher.Monitor();
                searcher.Begin();
                searcher.Wait();
                monitor.OccurredEvents.Should()
                    .Contain(x => x.EventName == nameof(searcher.MatchFound))
                    .Which.Parameters[1].Should().BeOfType<MatchFoundEventArgs>()
                    .Which.Matches.Should()
                    .SatisfyRespectively(
                        expectedResults.Select<SearchMatch, Action<SearchMatch>>(
                            e => m => m.Should().BeEquivalentTo(e)));
            }
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
