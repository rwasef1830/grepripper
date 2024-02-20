using System.Collections.Generic;
using JetBrains.Annotations;

namespace GrepRipper.UI.Services;

[PublicAPI]
public interface IClipboardService
{
    void SetText(string text);
    void SetFileDropList(IReadOnlyCollection<string> filePaths);
}
