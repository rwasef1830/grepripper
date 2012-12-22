using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FastGrep.Engine
{
    public class GlobExpression
    {
        readonly Regex _pattern;
        const RegexOptions c_RegexOptions = RegexOptions.Singleline;

        static readonly char[] s_InvalidChars =
            Path.GetInvalidFileNameChars().Except(new[] { '?', '*' }).ToArray();

        public GlobExpression(string pattern)
        {
            this._pattern = new Regex(
                MakeRegexPattern(pattern),
                c_RegexOptions);
        }

        static string MakeRegexPattern(string pattern)
        {
            ValidateGlobPattern(pattern);

            return "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";
        }

        static void ValidateGlobPattern(string pattern)
        {
            if (!IsValid(pattern))
            {
                throw new FormatException("Invalid pattern: '" + pattern + "'");
            }
        }

        public bool IsMatch(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            return fileName != null && this._pattern.IsMatch(fileName);
        }

        public static bool IsMatch(string filePath, string pattern)
        {
            return Regex.IsMatch(filePath, MakeRegexPattern(pattern), c_RegexOptions);
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
}