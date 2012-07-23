using System;
using System.Collections.Generic;
using EnsureThat;

namespace FastGrep.Engine
{
    public class MatchContext
    {
        public ICollection<string> MatchedLiterals { get; private set; }
        public ICollection<string> TextLines { get; private set; }

        public MatchContext(ICollection<string> matchedLiterals, ICollection<string> textLines)
        {
            Ensure.That(() => matchedLiterals).HasItems();
            Ensure.That(() => textLines).HasItems();

            this.MatchedLiterals = matchedLiterals;
            this.TextLines = textLines;
        }
    }
}