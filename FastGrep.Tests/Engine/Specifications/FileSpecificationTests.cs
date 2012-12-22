using System;
using System.IO;
using System.Linq;
using System.Threading;
using FastGrep.Engine.Specifications;
using NUnit.Framework;

namespace FastGrep.Tests.Engine.Specifications
{
    [TestFixture]
    public class FileSpecificationTests
    {
        string _tempPath;
        string _tempSubfolder;

        [TestFixtureSetUp]
        public void Setup()
        {
            var random = new Random();

            string tempPath = Path.Combine(Path.GetTempPath(), "FNGREP_" + random.Next());
            if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);

            Directory.CreateDirectory(tempPath);
            this._tempPath = tempPath;

            string tempSubfolder = Path.Combine(tempPath, random.Next().ToString(Thread.CurrentThread.CurrentCulture));
            Directory.CreateDirectory(tempSubfolder);
            this._tempSubfolder = tempSubfolder;

            const string dummyFileContent = "Just a temp file for unit tests";
            var topLevelFileNames = new[] { "temp1.css", "temp2.txt" };
            var subLevelFileNames = new[] { "temp3.asp", "temp4.bmp" };

            foreach (var fileName in topLevelFileNames)
            {
                File.WriteAllText(Path.Combine(tempPath, fileName), dummyFileContent);
            }

            foreach (var fileName in subLevelFileNames)
            {
                File.WriteAllText(Path.Combine(tempSubfolder, fileName), dummyFileContent);
            }
        }

        [Test]
        public void Enumeration_Toplevel_Only()
        {
            var fileSpec = new FileSpecification(this._tempPath, false, null, null);
            var files = fileSpec.EnumerateFiles().ToList();

            Assert.That(files.Count, Is.EqualTo(2));
            Assert.That(
                files.Any(x => x.Identifier == Path.Combine(this._tempPath, "temp1.css")), Is.True);
            Assert.That(
                files.Any(x => x.Identifier == Path.Combine(this._tempPath, "temp2.txt")), Is.True);
        }

        [Test]
        public void Enumeration_IncludeSubdirectories()
        {
            var fileSpec = new FileSpecification(this._tempPath, true, null, null);
            var files = fileSpec.EnumerateFiles().ToList();

            Assert.That(files.Count, Is.EqualTo(4));
            Assert.That(
                files.Any(x => x.Identifier == Path.Combine(this._tempPath, "temp1.css")), Is.True);
            Assert.That(
                files.Any(x => x.Identifier == Path.Combine(this._tempPath, "temp2.txt")), Is.True);
            Assert.That(
                files.Any(x => x.Identifier == Path.Combine(this._tempSubfolder, "temp3.asp")), Is.True);
            Assert.That(
                files.Any(x => x.Identifier == Path.Combine(this._tempSubfolder, "temp4.bmp")), Is.True);
        }

        [Test]
        public void Enumeration_none_matching_filePattern_Returns_EmptyResult()
        {
            var fileSpec = new FileSpecification(this._tempPath, true, new[] { "*.xyz" }, null);
            var files = fileSpec.EnumerateFiles().ToList();

            Assert.That(files.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Enumeration_matching_filePatterns_Returns_CorrectResult()
        {
            var fileSpec = new FileSpecification(this._tempPath, true, new[] { "temp1.css", "temp2.txt" }, null);
            var files = fileSpec.EnumerateFiles().ToList();

            Assert.That(files.Count(), Is.EqualTo(2));
            Assert.That(
                files.Any(x => x.Identifier == Path.Combine(this._tempPath, "temp1.css")), Is.True);
            Assert.That(
                files.Any(x => x.Identifier == Path.Combine(this._tempPath, "temp2.txt")), Is.True);
        }

        [Test]
        public void Enumeration_excluding_patterns_Returns_CorrectResult()
        {
            var fileSpec = new FileSpecification(
                this._tempPath, true, null, new[] { "*.asp", "*.bmp", "*.txt" });
            var files = fileSpec.EnumerateFiles().ToList();

            Assert.That(files.Count(), Is.EqualTo(1));
            Assert.That(
                files.Any(x => x.Identifier == Path.Combine(this._tempPath, "temp1.css")), Is.True);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Directory.Delete(this._tempPath, true);
        }
    }
}