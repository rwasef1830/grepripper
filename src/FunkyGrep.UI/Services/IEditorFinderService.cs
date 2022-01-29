using System.Collections.Generic;
using FunkyGrep.UI.ViewModels;

namespace FunkyGrep.UI.Services;

public interface IEditorFinderService
{
    IReadOnlyList<EditorInfo> FindInstalledSupportedEditors();
}
