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
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace FunkyGrep.UI.Validation.DataAnnotations
{
    public class ValidIndexInListMemberAttribute : ValidationAttribute
    {
        public string ListMemberName { get; }

        public override bool RequiresValidationContext => true;

        public ValidIndexInListMemberAttribute(string listMemberName)
        {
            this.ListMemberName = listMemberName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is int index))
            {
                return new ValidationResult("Incorrect value type. Expecting Int32.");
            }

            var instance = validationContext.ObjectInstance;
            if (instance == null)
            {
                throw new InvalidOperationException("Validation context object instance is required.");
            }

            var listProperty = instance.GetType().GetProperty(this.ListMemberName);
            if (listProperty == null)
            {
                return new ValidationResult(
                    $"Could not find list member '{this.ListMemberName}' in object being validated.");
            }

            var listObj = listProperty.GetValue(instance);
            if (!(listObj is IList list))
            {
                return new ValidationResult($"List member '{this.ListMemberName}' must implement IList.");
            }

            if (index >= list.Count)
            {
                return new ValidationResult("Index out of range of collection.");
            }

            return ValidationResult.Success;
        }
    }
}
