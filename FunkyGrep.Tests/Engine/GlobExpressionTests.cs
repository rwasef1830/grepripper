#region License
// Copyright (c) 2012 Raif Atef Wasef
// This source file is licensed under the  MIT license.
// 
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom 
// the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY 
// KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT 
// OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using FunkyGrep.Engine;
using NUnit.Framework;

namespace FunkyGrep.Tests.Engine
{
    [TestFixture]
    public class GlobExpressionTests
    {
        // ReSharper disable UnusedMethodReturnValue.Local
        static IEnumerable<TestCaseData> GetPositiveGlobTestCases()
        {
            return new[]
            {
                new TestCaseData("test.txt", "*.txt"),
                new TestCaseData("test1.txt", "test?.txt"),
                new TestCaseData("Xtest.txt", "?test.txt"),
                new TestCaseData("test.exe", "test.*"),
                new TestCaseData("a", "a"),
                new TestCaseData("test1Abc.tot", "test1???.t?t")
            };
        }

        static IEnumerable<TestCaseData> GetNegativeGlobTestCases()
        {
            return new[]
            {
                new TestCaseData("test.txt", "*.txx"),
                new TestCaseData("test1.txt", "test_.txt"),
                new TestCaseData("Xtest.txt", "_test.txt"),
                new TestCaseData("test.exe", "test.?"),
                new TestCaseData("a", "ab"),
                new TestCaseData("test1Abc.tot", "test1??X.t?t")
            };
        }

        static IEnumerable<TestCaseData> GetGlobTestCases()
        {
            return new[]
            {
                new TestCaseData("*.css", true),
                new TestCaseData("*.cs?", true),
                new TestCaseData("\\*.css", false),
                new TestCaseData("??*.??s", true),
                new TestCaseData("test", true),
                new TestCaseData("^", true)
            };
        }

        [Test]
        [TestCaseSource("GetNegativeGlobTestCases")]
        public void IsMatch_Instance_Negative(string input, string expression)
        {
            Assert.That(!new GlobExpression(expression).IsMatch(input));
        }

        [Test]
        [TestCaseSource("GetPositiveGlobTestCases")]
        public void IsMatch_Instance_Positive(string input, string expression)
        {
            Assert.That(new GlobExpression(expression).IsMatch(input));
        }

        [Test]
        [TestCaseSource("GetNegativeGlobTestCases")]
        public void IsMatch_Static_Negative(string input, string expression)
        {
            Assert.That(!GlobExpression.IsMatch(input, expression));
        }

        [Test]
        [TestCaseSource("GetPositiveGlobTestCases")]
        public void IsMatch_Static_Positive(string input, string expression)
        {
            Assert.That(GlobExpression.IsMatch(input, expression));
        }

        [Test]
        [TestCaseSource("GetGlobTestCases")]
        public void IsValid(string expression, bool isPositive)
        {
            bool isValid = GlobExpression.IsValid(expression);
            Assert.That(isValid, Is.EqualTo(isPositive));
        }

        // ReSharper restore UnusedMethodReturnValue.Local
    }
}