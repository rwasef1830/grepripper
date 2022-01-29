using System;

namespace FunkyGrep.UI.ViewModels;

public class SearchErrorItem : IFileItem
{
    public string AbsoluteFilePath { get; }
    public string RelativeFilePath { get; }
    public string Error { get; }

    public SearchErrorItem(string absoluteFilePath, string relativeFilePath, string error)
    {
        if (string.IsNullOrWhiteSpace(absoluteFilePath))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(absoluteFilePath));
        }

        if (string.IsNullOrWhiteSpace(relativeFilePath))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(relativeFilePath));
        }

        if (string.IsNullOrWhiteSpace(error))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(error));
        }

        this.AbsoluteFilePath = absoluteFilePath;
        this.RelativeFilePath = relativeFilePath;
        this.Error = error;
    }
}
