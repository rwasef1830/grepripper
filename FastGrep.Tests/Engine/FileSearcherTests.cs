using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            IEnumerable<DataSource> dataSources =
                MakeFileDataSourceList(new Dictionary<string, string> { { fileName, fileContent } });

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

            fileSearcher.Start();
            fileSearcher.Wait();

            Assert.That(matchFoundFired, "Match found event was not fired.");
            Assert.That(assertionException, Is.Null);
            Assert.That(completedFired, "Completion event was not fired.");
        }

        /// <summary>
        /// Each element of the passed list represents the fake file contents.
        /// The file name is a randomly generated GUID.
        /// </summary>
        static IEnumerable<DataSource> MakeFileDataSourceList(
            string file1Data,
            params string[] fileNData)
        {
            return
                MakeFileDataSourceList(
                    new[] { file1Data }.Union(fileNData)
                        .Select(x => new KeyValuePair<string, string>(Guid.NewGuid().ToString(), x)));
        }

        /// <summary>
        /// The dictionary key is the fake file name.
        /// The dictionary value is the fake file contents.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        static IEnumerable<DataSource> MakeFileDataSourceList(
            IEnumerable<KeyValuePair<string, string>> dictionary)
        {
            if (dictionary == null) return null;

            return dictionary.Select(
                x =>
                {
                    var stringReader = new StringReader(x.Value);
                    return new DataSource(x.Key, stringReader, x.Value.Length);
                });
        }
    }
}