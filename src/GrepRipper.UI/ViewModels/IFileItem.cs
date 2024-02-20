namespace GrepRipper.UI.ViewModels;

public interface IFileItem
{
    string AbsoluteFilePath { get; }
    string RelativeFilePath { get; }
}
