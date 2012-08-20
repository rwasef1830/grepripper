using System;
using System.Text.RegularExpressions;
using EnsureThat;

namespace FastGrep.Engine.Specifications
{
    public class PatternSpecification
    {
        public Regex Expression { get; private set; }

        public PatternSpecification(
            string text,
            bool isRegex,
            bool ignoreCase)
        {
            Ensure.That(() => text).IsNotNullOrWhiteSpace();

            var options = RegexOptions.None;
            if (ignoreCase) options |= RegexOptions.IgnoreCase;

            if (!isRegex)
            {
                text = Regex.Escape(text);
            }

            this.Expression = new Regex(text, options);
        }
    }
}