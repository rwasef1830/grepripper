using System;
using System.Collections.Generic;
using EnsureThat;

namespace FastGrep.Engine
{
    public class MatchFoundEventArgs : EventArgs
    {
        public string FilePath { get; set; }
        public IEnumerable<MatchedLine> Matches { get; set; }

        public MatchFoundEventArgs(string filePath, IEnumerable<MatchedLine> matches)
        {
            Ensure.That(filePath, "filePath").IsNotNullOrWhiteSpace();
            Ensure.That(matches, "matches").IsNotNull();

            this.FilePath = filePath;
            this.Matches = matches;
        }
    }
}