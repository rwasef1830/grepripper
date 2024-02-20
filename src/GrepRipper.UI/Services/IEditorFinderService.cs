using System.Collections.Generic;
using GrepRipper.UI.ViewModels;

namespace GrepRipper.UI.Services;

public interface IEditorFinderService
{
    IReadOnlyList<EditorInfo> FindInstalledSupportedEditors();
}
