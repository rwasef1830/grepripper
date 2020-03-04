#region License
// Copyright (c) 2020 Raif Atef Wasef
// This source file is licensed under the  MIT license.
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom
// the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY
// KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT
// OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FunkyGrep.UI.ViewModels;
using Microsoft.Win32;

namespace FunkyGrep.UI.Services
{
    class EditorFinderService : IEditorFinderService
    {
        static readonly IEnumerable<EditorInfo> s_SupportedEditors = new[]
        {
            new EditorInfo
            {
                DisplayName = "Visual Studio Code",
                ExecutablePath = "code.cmd",
                ArgumentsTemplate = "-g \"{0}\":{1}"
            },
            new EditorInfo
            {
                DisplayName = "Notepad3",
                ExecutablePath = "notepad3.exe",
                ArgumentsTemplate = "/g {1} \"{0}\""
            },
            new EditorInfo
            {
                DisplayName = "Notepad2",
                ExecutablePath = "notepad2.exe",
                ArgumentsTemplate = "/g {1} \"{0}\""
            },
            new EditorInfo
            {
                DisplayName = "Notepad++",
                ExecutablePath = "notepad++.exe",
                ArgumentsTemplate = "-n{1} \"{0}\""
            }
        };

        public IReadOnlyList<EditorInfo> FindInstalledSupportedEditors()
        {
            var result = new List<EditorInfo>();

            foreach (var editor in s_SupportedEditors)
            {
                var editorExecutablePath = GetFullPath(editor.ExecutablePath);
                if (editorExecutablePath != null)
                {
                    result.Add(
                        new EditorInfo
                        {
                            DisplayName = editor.DisplayName, 
                            ExecutablePath = editorExecutablePath,
                            ArgumentsTemplate = editor.ArgumentsTemplate,
                        });
                }
            }

            result.Add(EditorInfo.GetDefaultEditor());
            return result;
        }

        public static bool ExistsOnPath(string fileName)
        {
            return GetFullPath(fileName) != null;
        }

        public static string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
            {
                return Path.GetFullPath(fileName);
            }

            fileName = Path.GetFileName(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");
            var filePath = (values?.Split(Path.PathSeparator) ?? new string[0])
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

                fileName = key.GetValue(string.Empty)?.ToString();
                if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
                {
                    return fileName;
                }
            }

            return null;
        }
    }
}
