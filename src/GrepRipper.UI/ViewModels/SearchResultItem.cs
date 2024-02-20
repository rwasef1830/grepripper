using System;
using GrepRipper.Engine;

namespace GrepRipper.UI.ViewModels;

public class SearchResultItem : IFileItem
{
    public string AbsoluteFilePath { get; }
    public string RelativeFilePath { get; }
    public SearchMatch Match { get; }

    public SearchResultItem(string absoluteFilePath, string relativeFilePath, SearchMatch match)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(absoluteFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(relativeFilePath);
        ArgumentNullException.ThrowIfNull(match);
        
        this.AbsoluteFilePath = absoluteFilePath;
        this.RelativeFilePath = relativeFilePath;
        this.Match = match;
    }
}
