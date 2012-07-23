using System;
using System.Text.RegularExpressions;
using EnsureThat;

namespace FastGrep.Engine.Specifications
{
    public class PatternSpecification
    {
        public Regex Expression { get; private set; }

        public PatternSpecification(string regularExpression, bool ignoreCase)
        {
            Ensure.That(() => regularExpression).IsNotNullOrWhiteSpace();

            RegexOptions options = RegexOptions.None;
            if (ignoreCase) options |= RegexOptions.IgnoreCase;

            this.Expression = new Regex(regularExpression, options);
        }
    }
}