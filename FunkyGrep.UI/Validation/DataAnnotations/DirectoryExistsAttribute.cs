#region License
// Copyright (c) 2020 Raif Atef Wasef
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
using System.ComponentModel.DataAnnotations;
using FunkyGrep.UI.Util;

namespace FunkyGrep.UI.Validation.DataAnnotations
{
    public class DirectoryExistsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return ValidationResult.Success;
            }

            if (!(value is string path))
            {
                return new ValidationResult("Invalid value type.");
            }

            try
            {
                var result = DirectoryUtil.ExistsOrNullIfTimeout(path, TimeSpan.FromSeconds(2));

                if (result == null)
                {
                    return new ValidationResult("Timed out accessing directory.");
                }

                return result.Value ? ValidationResult.Success : new ValidationResult("Directory doesn't exist.");
            }
            catch (Exception ex)
            {
                if (ex is AggregateException aggregate)
                {
                    ex = aggregate.InnerException ?? ex;
                }

                return new ValidationResult("Cannot access directory: " + ex.Message);
            }
        }
    }
}
