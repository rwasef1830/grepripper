using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace GrepRipper.Engine.Specifications;

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

        if (ReferenceEquals(filePatterns, Enumerable.Empty<string>()))
        {
            filePatterns = new[] { "*" };
        }

        this._filePatterns = filePatterns
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        this._fileExcludePatterns = fileExcludePatterns
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }

    [SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
    [SuppressMessage("ReSharper", "InvertIf")]
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
