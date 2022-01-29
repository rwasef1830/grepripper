using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace FunkyGrep.Engine;

[PublicAPI]
public class GlobExpression
{
    const RegexOptions c_RegexOptions = RegexOptions.Singleline;

    static readonly char[] s_InvalidChars =
        Path.GetInvalidFileNameChars().Except(new[] { '?', '*' }).ToArray();

    readonly Regex _pattern;

    public GlobExpression(string pattern)
    {
        this._pattern = new Regex(
            MakeRegexPattern(pattern),
            c_RegexOptions);
    }

    static string MakeRegexPattern(string pattern)
    {
        ValidateGlobPattern(pattern);

        return @"\G" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";
    }

    static void ValidateGlobPattern(string pattern)
    {
        if (!IsValid(pattern))
        {
            throw new FormatException($"Invalid pattern: '{pattern}'");
        }
    }

    public bool IsMatch(string filePath)
    {
        return this._pattern.IsMatch(filePath, filePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
    }

    public static char[] GetInvalidChars()
    {
        return (char[])s_InvalidChars.Clone();
    }

    public static bool IsValid(string pattern)
    {
        return pattern.IndexOfAny(s_InvalidChars) == -1;
    }
}
