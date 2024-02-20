using System;
using System.IO;

namespace GrepRipper.Engine;

public class FileDataSource : IDataSource
{
    public string Identifier { get; }

    public FileDataSource(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));
        }

        this.Identifier = filePath;
    }

    public Stream OpenRead()
    {
        return File.Open(this.Identifier, FileMode.Open, FileAccess.Read, FileShare.Read);
    }
}
