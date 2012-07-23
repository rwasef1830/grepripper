using System;
using System.Text.RegularExpressions;
using EnsureThat;

namespace FastGrep.Engine
{
    public class MatchEventArgs : EventArgs
    {
        public string FilePath { get; set; }
        public MatchCollection Matches { get; set; }

        public MatchEventArgs(string filePath, MatchCollection matches)
        {
            Ensure.That(() => filePath).IsNotNullOrWhiteSpace();
            Ensure.That(() => matches).HasItems();

            this.FilePath = filePath;
            this.Matches = matches;
        }
    }
}