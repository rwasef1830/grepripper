using System;
using System.IO;
using System.Text.RegularExpressions;

namespace FastGrep.Engine
{
    public class GlobExpression
    {
        readonly Regex _pattern;
        const RegexOptions c_RegexOptions = RegexOptions.Singleline;

        public GlobExpression(string pattern)
        {
            this._pattern = new Regex(
                MakeRegexPattern(pattern),
                c_RegexOptions);
        }

        static string MakeRegexPattern(string pattern)
        {
            return "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";
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
    }
}