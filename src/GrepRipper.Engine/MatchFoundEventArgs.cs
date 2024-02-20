using System;
using System.Collections.Generic;

namespace GrepRipper.Engine;

public class MatchFoundEventArgs
{
    public string FilePath { get; }
    public IReadOnlyList<SearchMatch> Matches { get; }

    public MatchFoundEventArgs(string filePath, IReadOnlyList<SearchMatch> matches)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        this.FilePath = filePath;
        this.Matches = matches ?? throw new ArgumentNullException(nameof(matches));
    }
}
