using System;

namespace GrepRipper.Engine;

public class SearchErrorEventArgs
{
    public string FilePath { get; }
    public Exception Error { get; }

    public SearchErrorEventArgs(string filePath, Exception error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        this.FilePath = filePath;
        this.Error = error ?? throw new ArgumentNullException(nameof(error));
    }
}
