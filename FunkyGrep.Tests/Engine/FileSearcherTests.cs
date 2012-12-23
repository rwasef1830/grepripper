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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FunkyGrep.Engine;
using NUnit.Framework;

namespace FunkyGrep.Tests.Engine
{
    [TestFixture]
    public class FileSearcherTests
    {
        static IEnumerable<TestCaseData> LongLinesClampTestSource()
        {
            const int maxContextLength = 10;

            yield return new TestCaseData(
                maxContextLength,
                "A",
                "ABBBBBBBBCDEF",
                "ABBBBBBBBC")
                .SetName("Tail excess no CRLF");

            yield return new TestCaseData(
                maxContextLength,
                "A",
                "BBBBBBBBACDEF",
                "BBBBBBBBAC")
                .SetName("Head excess no CRLF");

            yield return new TestCaseData(
                maxContextLength,
                "A",
                "\r\nABBBBBBBBCDEF\r\n",
                "ABBBBBBBBC")
                .SetName("Tail excess with CRLF");

            yield return new TestCaseData(
                maxContextLength,
                "A",
                "\r\nBBBBBBBBACDEF\r\n",
                "BBBBBBBBAC")
                .SetName("Head excess with CRLF");
        }

        /// <summary>
        ///     Each element of the passed list represents the fake file contents.
        ///     The file name is a randomly generated GUID.
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
                                               Guid.NewGuid().ToString(), x)));
        }

        /// <summary>
        ///     The dictionary key is the fake file name.
        ///     The dictionary value is the fake file contents.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        static IEnumerable<IDataSource> MakeDataSourceList(
            IEnumerable<KeyValuePair<string, string>> dictionary)
        {
            return dictionary == null
                       ? null
                       : dictionary.Select(
                           x => new TestDataSource(x.Key, x.Value, Encoding.Unicode));
        }

        [Test]
        public void FileSearcher_FindsAMatch_FiresEvents()
        {
            const string fileName = "Test";
            const string fileContent = "Speedy thing goes in, speedy thing comes out!";

            IEnumerable<IDataSource> dataSources =
                MakeDataSourceList(new Dictionary<string, string> { { fileName, fileContent } });

            var fileSearcher = new FileSearcher(
                new Regex("speedy", RegexOptions.None), dataSources);

            bool matchFoundFired = false;
            Exception assertionException = null;

            fileSearcher.MatchFound +=
                (sender, args) =>
                {
                    matchFoundFired = true;

                    try
                    {
                        Assert.That(args.FilePath, Is.EqualTo(fileName));
                        Assert.That(args.Matches.Count(), Is.EqualTo(1));
                        Assert.That(args.Matches.First().Number, Is.EqualTo(1));
                        Assert.That(args.Matches.First().Text, Is.EqualTo(fileContent));
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

            Assert.That(matchFoundFired, "Match found event was not fired.");
            Assert.That(assertionException, Is.Null);
            Assert.That(completedFired, "Completion event was not fired.");
        }

        [Test]
        [TestCaseSource("LongLinesClampTestSource")]
        public void Long_lines_in_results_are_clamped_properly_around_match(
            int contextLength, string matchText, string lineToSearch, string expectedContext)
        {
            IEnumerable<IDataSource> dataSources = MakeDataSourceList(lineToSearch);
            var searcher = new FileSearcher(
                new Regex(Regex.Escape(matchText)), dataSources, contextLength);

            Exception failedAssertion = new AssertionException("MatchFound event was not fired.");

            searcher.MatchFound +=
                (sender, args) =>
                {
                    try
                    {
                        Assert.That(args.Matches.First().Text, Is.EqualTo(expectedContext));
                        failedAssertion = null;
                    }
                    catch (Exception ex)
                    {
                        failedAssertion = ex;
                    }
                };

            searcher.Begin();
            searcher.Wait();

            if (failedAssertion != null)
            {
                throw failedAssertion;
            }
        }
    }
}