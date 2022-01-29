using System;

namespace FunkyGrep.Engine;

public class SearchErrorEventArgs
{
    public string FilePath { get; }
    public Exception Error { get; }

    public SearchErrorEventArgs(string filePath, Exception error)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));
        }

        this.FilePath = filePath;
        this.Error = error ?? throw new ArgumentNullException(nameof(error));
    }
}
