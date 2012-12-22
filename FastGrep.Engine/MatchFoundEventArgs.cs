using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace FastGrep.Engine
{
    public class MatchFoundEventArgs : EventArgs
    {
        public string FilePath { get; private set; }
        public IEnumerable<MatchedLine> Matches { get; private set; }

        public MatchFoundEventArgs(string filePath, IEnumerable<MatchedLine> matches)
        {
            Ensure.That(filePath, "filePath").IsNotNullOrWhiteSpace();
            // ReSharper disable PossibleMultipleEnumeration
            Ensure.That(matches, "matches").IsNotNull();

            var matchedLines = matches as IList<MatchedLine> ?? matches.ToList();
            // ReSharper restore PossibleMultipleEnumeration

            this.FilePath = filePath;
            this.Matches = matchedLines;
        }
    }
}