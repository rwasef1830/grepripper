using System;

namespace GrepRipper.UI.ViewModels;

public class SearchErrorItem : IFileItem
{
    public string AbsoluteFilePath { get; }
    public string RelativeFilePath { get; }
    public string Error { get; }

    public SearchErrorItem(string absoluteFilePath, string relativeFilePath, string error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(absoluteFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(relativeFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(error);

        this.AbsoluteFilePath = absoluteFilePath;
        this.RelativeFilePath = relativeFilePath;
        this.Error = error;
    }
}
