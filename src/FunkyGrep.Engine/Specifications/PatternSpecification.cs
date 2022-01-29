using System;
using System.Text.RegularExpressions;

namespace FunkyGrep.Engine.Specifications;

public class PatternSpecification
{
    public Regex Expression { get; }

    public PatternSpecification(
        string text,
        bool isRegex,
        bool ignoreCase)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));
        }

        var options = RegexOptions.None;
        if (ignoreCase) options |= RegexOptions.IgnoreCase;

        if (!isRegex)
        {
            text = Regex.Escape(text);
        }

        this.Expression = new Regex(text, options);
    }
}
