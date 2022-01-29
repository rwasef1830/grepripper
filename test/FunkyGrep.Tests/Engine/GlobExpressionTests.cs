using System.Collections.Generic;
using FluentAssertions;
using FunkyGrep.Engine;
using Xunit;

namespace FunkyGrep.Tests.Engine;

public class GlobExpressionTests
{
    // ReSharper disable UnusedMethodReturnValue.Local
    public static IEnumerable<object[]> GetPositiveGlobTestCases()
    {
        return new[]
        {
            new[] { @"test.txt", "*.txt" },
            new[] { @"test1.txt", "test?.txt" },
            new[] { @"Xtest.txt", "?test.txt" },
            new[] { @"test.exe", "test.*" },
            new[] { @"a", "a" },
            new[] { @"test1Abc.tot", "test1???.t?t" },
            new[] { @"dummy\test.txt", "test.*" }
        };
    }

    public static IEnumerable<object[]> GetNegativeGlobTestCases()
    {
        return new[]
        {
            new[] { @"test.txt", "*.txx" },
            new[] { @"test1.txt", "test_.txt" },
            new[] { @"Xtest.txt", "_test.txt" },
            new[] { @"test.exe", "test.?" },
            new[] { @"a", "ab" },
            new[] { @"test1Abc.tot", "test1??X.t?t" },
            new[] { @"dummy\test.txt", "wrong.*" }
        };
    }

    public static IEnumerable<object[]> GetGlobTestCases()
    {
        return new[]
        {
            new object[] { "*.css", true },
            new object[] { "*.cs?", true },
            new object[] { "\\*.css", false },
            new object[] { "??*.??s", true },
            new object[] { "test", true },
            new object[] { "^", true }
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
