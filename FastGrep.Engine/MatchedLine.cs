using System;
using EnsureThat;

namespace FastGrep.Engine
{
    public class MatchedLine
    {
        public int Number { get; private set; }
        public string Text { get; private set; }

        public MatchedLine(int number, string text)
        {
            Ensure.That(number, "number").IsGt(0);

            this.Number = number;
            this.Text = text ?? String.Empty;
        }
    }
}
