using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using GrepRipper.Engine;
using Xunit;

namespace GrepRipper.Tests.Engine;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class GlobExpressionTests
{
    public static TheoryData<string, string> GetPositiveGlobTestCases()
    {
        return new TheoryData<string, string>
        {
            { "test.txt", "*.txt" },
            { "test1.txt", "test?.txt" },
            { "Xtest.txt", "?test.txt" },
            { "test.exe", "test.*" },
            { "a", "a" },
            { "test1Abc.tot", "test1???.t?t" },
            { "dummy\\test.txt", "test.*" }
        };
    }

    public static TheoryData<string, string> GetNegativeGlobTestCases()
    {
        return new TheoryData<string, string>
        {
            { "test.txt", "*.txx" },
            { "test1.txt", "test_.txt" },
            { "Xtest.txt", "_test.txt" },
            { "test.exe", "test.?" },
            { "a", "ab" },
            { "test1Abc.tot", "test1??X.t?t" },
            { "dummy\\test.txt", "wrong.*" }
        };
    }

    public static TheoryData<string, bool> GetGlobTestCases()
    {
        return new TheoryData<string, bool>
        {
            { "*.css", true },
            { "*.cs?", true },
            { "\\*.css", false },
            { "??*.??s", true },
            { "test", true },
            { "^", true }
        };
    }

    [Theory]
    [MemberData(nameof(GetNegativeGlobTestCases))]
    public void IsMatch_Instance_Negative(string input, string expression)
    {
        new GlobExpression(expression).IsMatch(input).Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetPositiveGlobTestCases))]
    public void IsMatch_Instance_Positive(string input, string expression)
    {
        new GlobExpression(expression).IsMatch(input).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(GetGlobTestCases))]
    public void IsValid(string expression, bool isPositive)
    {
        GlobExpression.IsValid(expression).Should().Be(isPositive);
    }

    // ReSharper restore UnusedMethodReturnValue.Local
}
