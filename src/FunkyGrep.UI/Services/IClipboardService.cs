using System.Collections.Generic;
using JetBrains.Annotations;

namespace FunkyGrep.UI.Services;

[PublicAPI]
public interface IClipboardService
{
    void SetText(string text);
    void SetFileDropList(IReadOnlyCollection<string> filePaths);
}
