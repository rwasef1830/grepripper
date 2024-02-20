namespace GrepRipper.UI.ViewModels;

/// <param name="ArgumentsTemplate">
/// {0} = FilePath
/// {1} = LineNumber
/// </param>
public record EditorInfo(string DisplayName, string ExecutablePath, string ArgumentsTemplate)
{
    public static EditorInfo GetDefaultEditor()
    {
        return new EditorInfo("Notepad", "notepad.exe", "\"{0}\"");
    }
}
