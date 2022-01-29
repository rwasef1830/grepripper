using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;

namespace FunkyGrep.UI.Services;

class ClipboardService : IClipboardService
{
    public void SetText(string text)
    {
        Clipboard.SetText(text, TextDataFormat.UnicodeText);
    }

    public void SetFileDropList(IReadOnlyCollection<string> filePaths)
    {
        var collection = new StringCollection();
        foreach (var filePath in filePaths)
        {
            collection.Add(filePath);
        }

        Clipboard.SetFileDropList(collection);
    }
}
