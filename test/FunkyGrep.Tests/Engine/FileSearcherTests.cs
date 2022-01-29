using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using FunkyGrep.Engine;
using Xunit;

namespace FunkyGrep.Tests.Engine;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
public class FileSearcherTests
{
    [Fact]
    public void FileSearcher_FindsAMatch_FiresEvents()
    {
        const string fileName = "Test";
        const string fileContent = "Speedy thing goes in, speedy thing comes out!";

        var dataSource = new TestDataSource(fileName, fileContent, Encoding.UTF8);

        var fileSearcher = new FileSearcher(
            new Regex("speedy", RegexOptions.None),
            new[] { dataSource },
            false,
            0);

        using var monitor = fileSearcher.Monitor();
        fileSearcher.Begin();
        fileSearcher.Wait();

        monitor.OccurredEvents.Should().Contain(x => x.EventName == nameof(fileSearcher.MatchFound))
            .Which.Parameters[1].Should().BeOfType<MatchFoundEventArgs>()
            .Which.Matches.Should().BeEquivalentTo(new[] { new SearchMatch(1, fileContent, 22, 6, null, null) });

        monitor.OccurredEvents.Should().Contain(x => x.EventName == nameof(fileSearcher.Completed))
            .Which.Parameters[1].Should().BeOfType<CompletedEventArgs>()
            .Which.FailureReason.Should().BeNull();

        monitor.OccurredEvents.Should().NotContain(x => x.EventName == nameof(fileSearcher.Error));
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Match_tests
    {
        const int c_MaxContextLength = 10;

        [Fact]
        public void Empty_file_with_Unicode_BOM()
        {
            DoTest(
                "A",
                new TestDataSource(string.Empty, new UnicodeEncoding(false, true)),
                0);
        }

        [Fact]
        public void Tail_excess_single_line()
        {
            DoTest(
                "A",
                "ABBBBBBBBCDEF",
                0,
                new SearchMatch(1, "ABBBBBBBBC", 0, 1, null, null));
        }

        [Fact]
        public void Head_excess_single_line()
        {
            DoTest(
                "A",
                "BBBBBBBBACDEF",
                0,
                new SearchMatch(1, "BBBBBACDEF", 5, 1, null, null));
        }

        [Fact]
        public void Tail_excess_multiple_lines()
        {
            DoTest(
                "A",
                "\r\nABBBBBBBBCDEF\r\n",
                0,
                new SearchMatch(2, "ABBBBBBBBC", 0, 1, null, null));
        }

        [Fact]
        public void Head_excess_multiple_lines()
        {
            DoTest(
                "A",
                "\r\nBBBBBBBBACDEF\r\n",
                0,
                new SearchMatch(2, "BBBBBACDEF", 5, 1, null, null));
        }

        [Fact]
        public void Match_at_end_of_line()
        {
            DoTest(
                "ABC",
                "\r\nXXXXXXXABC\r\n",
                0,
                new SearchMatch(2, "XXXXXXXABC", 7, 3, null, null));
        }

        [Fact]
        public void Match_at_start_of_line()
        {
            DoTest(
                "ABC",
                "\r\nABCXXXXXXX\r\n",
                0,
                new SearchMatch(2, "ABCXXXXXXX", 0, 3, null, null));
        }

        [Fact]
        public void Context_extraction_around_match()
        {
            DoTest(
                "ABC",
                "1234567890ABC12345",
                0,
                new SearchMatch(1, "7890ABC123", 4, 3, null, null));
        }

        [Fact]
        public void Match_larger_than_context_clamp_limit()
        {
            DoTest(
                "123456789012345",
                "XXXXXX123456789012345XXXXXX",
                0,
                new SearchMatch(1, "123456789012345", 0, 15, null, null));
        }

        [Fact]
        public void Post_match_context_lines_more_than_file_lines()
        {
            DoTest(
                "A",
                "A\r\nB",
                3,
                new SearchMatch(1, "A", 0, 1, null, new[] { "B" }));
        }

        [Fact]
        public void Context_lines_should_be_clamped()
        {
            DoTest(
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

        static void DoTest(
            string searchPattern,
            string textToSearch,
            int contextLineCount,
            params SearchMatch[] expectedResults)
        {
            var dataSource = new TestDataSource(textToSearch, Encoding.UTF8);
            DoTest(searchPattern, dataSource, contextLineCount, expectedResults);
        }

        static void DoTest(
            string searchPattern,
            IDataSource dataSource,
            int contextLineCount,
            params SearchMatch[] expectedResults)
        {
            var searcher = new FileSearcher(
                new Regex(Regex.Escape(searchPattern)),
                new[] { dataSource },
                false,
                contextLineCount,
                c_MaxContextLength);

            using var monitor = searcher.Monitor();
            searcher.Begin();
            searcher.Wait();

            if (expectedResults.Length > 0)
            {
                monitor.OccurredEvents.Should().Contain(x => x.EventName == nameof(searcher.MatchFound))
                    .Which.Parameters[1].Should().BeOfType<MatchFoundEventArgs>()
                    .Which.Matches.Should()
                    .SatisfyRespectively(
                        expectedResults.Select<SearchMatch, Action<SearchMatch>>(
                            e => m => m.Should().BeEquivalentTo(e)));
            }
            else
            {
                monitor.OccurredEvents.Should().NotContain(x => x.EventName == nameof(searcher.MatchFound));
            }

            monitor.OccurredEvents.Should().Contain(x => x.EventName == nameof(searcher.Completed))
                .Which.Parameters[1].Should().BeOfType<CompletedEventArgs>()
                .Which.FailureReason.Should().BeNull();

            monitor.OccurredEvents.Should().NotContain(x => x.EventName == nameof(searcher.Error));
        }
    }
}
