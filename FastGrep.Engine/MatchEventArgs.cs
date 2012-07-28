using System;
using System.Text.RegularExpressions;
using EnsureThat;

namespace FastGrep.Engine
{
    public class MatchEventArgs : EventArgs
    {
        public string FilePath { get; set; }
        public string FileContent { get; set; }
        public MatchCollection Matches { get; set; }

        public MatchEventArgs(string filePath, string fileContent, MatchCollection matches)
        {
            Ensure.That(() => filePath).IsNotNullOrWhiteSpace();
            Ensure.That(() => fileContent).IsNotNull();
            Ensure.That(() => matches).HasItems();

            this.FilePath = filePath;
            this.FileContent = fileContent;
            this.Matches = matches;
        }
    }
}