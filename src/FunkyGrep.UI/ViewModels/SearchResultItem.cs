using System;
using FunkyGrep.Engine;

namespace FunkyGrep.UI.ViewModels;

public class SearchResultItem : IFileItem
{
    public string AbsoluteFilePath { get; }
    public string RelativeFilePath { get; }
    public SearchMatch Match { get; }

    public SearchResultItem(string absoluteFilePath, string relativeFilePath, SearchMatch match)
    {
        if (string.IsNullOrWhiteSpace(absoluteFilePath))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(absoluteFilePath));
        }

        if (string.IsNullOrWhiteSpace(relativeFilePath))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(relativeFilePath));
        }

        this.AbsoluteFilePath = absoluteFilePath;
        this.RelativeFilePath = relativeFilePath;
        this.Match = match ?? throw new ArgumentNullException(nameof(match));
    }
}
