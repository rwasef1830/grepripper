using System;
using System.Text.RegularExpressions;

namespace GrepRipper.Engine.Specifications;

public class PatternSpecification
{
    public Regex Expression { get; }

    public PatternSpecification(
        string text,
        bool isRegex,
        bool ignoreCase)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        
        var options = RegexOptions.None;
        if (ignoreCase) options |= RegexOptions.IgnoreCase;

        if (!isRegex)
        {
            text = Regex.Escape(text);
        }

        this.Expression = new Regex(text, options);
    }
}
