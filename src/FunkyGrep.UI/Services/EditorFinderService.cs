using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using FunkyGrep.UI.ViewModels;
using Microsoft.Win32;

namespace FunkyGrep.UI.Services;

class EditorFinderService : IEditorFinderService
{
    static readonly IEnumerable<EditorInfo> s_SupportedEditors = new[]
    {
        new EditorInfo("Visual Studio Code", "code.cmd", "-g \"{0}\":{1}"),
        new EditorInfo("Notepad3", "notepad3.exe", "/g {1} \"{0}\""),
        new EditorInfo("Notepad2", "notepad2.exe", "/g {1} \"{0}\""),
        new EditorInfo("Notepad++", "notepad++.exe", "-n{1} \"{0}\"")
    };

    public IReadOnlyList<EditorInfo> FindInstalledSupportedEditors()
    {
        var result = new List<EditorInfo>();

        foreach (var (displayName, executablePath, argumentsTemplate) in s_SupportedEditors)
        {
            var editorExecutablePath = GetFullPath(executablePath);
            if (editorExecutablePath != null)
            {
                result.Add(new EditorInfo(
                    displayName,
                    editorExecutablePath,
                    argumentsTemplate));
            }
        }

        result.Add(EditorInfo.GetDefaultEditor());
        return result;
    }

    [SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
    public static string? GetFullPath(string fileName)
    {
        if (File.Exists(fileName))
        {
            return Path.GetFullPath(fileName);
        }

        fileName = Path.GetFileName(fileName);

        var values = Environment.GetEnvironmentVariable("PATH");
        var filePath = (values?.Split(Path.PathSeparator) ?? Array.Empty<string>())
            .Select(path => Path.Combine(path, fileName))
            .FirstOrDefault(File.Exists);

        if (filePath != null)
        {
            return filePath;
        }

        string subKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\" + fileName;

        foreach (var key in new[]
                 {
                     Registry.CurrentUser.OpenSubKey(subKey), 
                     Registry.LocalMachine.OpenSubKey(subKey)
                 })
        {
            if (key == null)
            {
                continue;
            }

            var fileNameResult = key.GetValue(string.Empty)?.ToString();
            if (!string.IsNullOrWhiteSpace(fileNameResult) && File.Exists(fileNameResult))
            {
                return fileNameResult;
            }
        }

        return null;
    }
}
