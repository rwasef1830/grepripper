﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FastGrep.Engine;
using NUnit.Framework;

namespace FastGrep.Tests.Engine
{
    [TestFixture]
    public class FileSearcherTests
    {
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
        public void Long_lines_in_results_are_clamped_properly_around_match(int contextLength, string matchText, string lineToSearch, string expectedContext)
        {
            var dataSources = MakeDataSourceList(lineToSearch);
            var searcher = new FileSearcher(new Regex(Regex.Escape(matchText)), dataSources, contextLength);

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
                        .Select(x => new KeyValuePair<string, string>(Guid.NewGuid().ToString(), x)));
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
            return dictionary == null
                       ? null
                       : dictionary.Select(x => new TestDataSource(x.Key, x.Value, Encoding.Unicode));
        }
    }
}