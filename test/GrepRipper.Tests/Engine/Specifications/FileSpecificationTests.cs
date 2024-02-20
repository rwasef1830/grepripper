using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using FluentAssertions;
using GrepRipper.Engine;
using GrepRipper.Engine.Specifications;
using Xunit;

namespace GrepRipper.Tests.Engine.Specifications;

[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
[SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public sealed class FileSpecificationTests : IDisposable
{
    readonly string _tempPath;
    readonly string _tempSubfolder;

    public FileSpecificationTests()
    {
        var random = new Random();

        string tempPath = Path.Combine(Path.GetTempPath(), "FNGREP_" + random.Next());
        if (Directory.Exists(tempPath))
        {
            Directory.Delete(tempPath, true);
        }

        Directory.CreateDirectory(tempPath);
        this._tempPath = tempPath;

        string tempSubfolder = Path.Combine(
            tempPath,
            random.Next().ToString(Thread.CurrentThread.CurrentCulture));
        Directory.CreateDirectory(tempSubfolder);
        this._tempSubfolder = tempSubfolder;

        const string dummyFileContent = "Just a temp file for unit tests";
        var topLevelFileNames = new[] { "temp1.css", "temp2.txt" };
        var subLevelFileNames = new[] { "temp3.asp", "temp4.bmp" };

        foreach (string fileName in topLevelFileNames)
        {
            File.WriteAllText(Path.Combine(tempPath, fileName), dummyFileContent);
        }

        foreach (string fileName in subLevelFileNames)
        {
            File.WriteAllText(Path.Combine(tempSubfolder, fileName), dummyFileContent);
        }
    }

    public void Dispose()
    {
        Directory.Delete(this._tempPath, true);
    }

    [Fact]
    public void Enumeration_IncludeSubdirectories()
    {
        var fileSpec = new FileSpecification(
            this._tempPath,
            true,
            Enumerable.Empty<string>(),
            Enumerable.Empty<string>());
        List<IDataSource> files = fileSpec.EnumerateFiles().ToList();

        files.Count.Should().Be(4);
        files.Should().Contain(x => x.Identifier == Path.Combine(this._tempPath, "temp1.css"));
        files.Should().Contain(x => x.Identifier == Path.Combine(this._tempPath, "temp2.txt"));
        files.Should().Contain(x => x.Identifier == Path.Combine(this._tempSubfolder, "temp3.asp"));
        files.Should().Contain(x => x.Identifier == Path.Combine(this._tempSubfolder, "temp4.bmp"));
    }

    [Fact]
    public void Enumeration_TopLevel_Only()
    {
        var fileSpec = new FileSpecification(
            this._tempPath,
            false,
            Enumerable.Empty<string>(),
            Enumerable.Empty<string>());

        List<IDataSource> files = fileSpec.EnumerateFiles().ToList();

        files.Count.Should().Be(2);
        files.Should().Contain(x => x.Identifier == Path.Combine(this._tempPath, "temp1.css"));
        files.Should().Contain(x => x.Identifier == Path.Combine(this._tempPath, "temp2.txt"));
    }

    [Fact]
    public void Enumeration_excluding_patterns_Returns_CorrectResult()
    {
        var fileSpec = new FileSpecification(
            this._tempPath,
            true,
            Enumerable.Empty<string>(),
            new[] { "*.asp", "*.bmp", "*.txt" });
        List<IDataSource> files = fileSpec.EnumerateFiles().ToList();

        files.Count.Should().Be(1);
        files.Should().Contain(x => x.Identifier == Path.Combine(this._tempPath, "temp1.css"));
    }

    [Fact]
    public void Enumeration_matching_filePatterns_Returns_CorrectResult()
    {
        var fileSpec = new FileSpecification(
            this._tempPath,
            true,
            new[] { "temp1.css", "temp2.txt" },
            Enumerable.Empty<string>());
        List<IDataSource> files = fileSpec.EnumerateFiles().ToList();

        files.Count.Should().Be(2);
        files.Should().Contain(x => x.Identifier == Path.Combine(this._tempPath, "temp1.css"));
        files.Should().Contain(x => x.Identifier == Path.Combine(this._tempPath, "temp2.txt"));
    }

    [Fact]
    public void Enumeration_none_matching_filePattern_Returns_EmptyResult()
    {
        var fileSpec = new FileSpecification(
            this._tempPath,
            true,
            new[] { "*.xyz" },
            Enumerable.Empty<string>());
        List<IDataSource> files = fileSpec.EnumerateFiles().ToList();

        files.Count.Should().Be(0);
    }
}
