using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FunkyGrep.UI.Validation.DataAnnotations
{
    public class PossiblyValidRegexAttribute : ValidationAttribute
    {
        public string ValueIsRegexMemberName { get; }

        public override bool RequiresValidationContext => true;

        public PossiblyValidRegexAttribute(string valueIsRegexMemberName)
        {
            this.ValueIsRegexMemberName = valueIsRegexMemberName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is string expression))
            {
                return new ValidationResult("Incorrect value type. Expecting String.");
            }

            var instance = validationContext.ObjectInstance;
            if (instance == null)
            {
                throw new InvalidOperationException("Validation context object instance is required.");
            }

            var valueIsRegexProperty = instance.GetType().GetProperty(this.ValueIsRegexMemberName);
            if (valueIsRegexProperty == null)
            {
                return new ValidationResult(
                    $"Could not find member '{this.ValueIsRegexMemberName}' in object being validated.");
            }

            var valueIsRegexObj = valueIsRegexProperty.GetValue(instance);
            if (!(valueIsRegexObj is bool valueIsRegex))
            {
                return new ValidationResult($"Member '{this.ValueIsRegexMemberName}' must be Boolean.");
            }

            if (!valueIsRegex)
            {
                return ValidationResult.Success;
            }

            try
            {
                _ = Regex.IsMatch(string.Empty, value.ToString());
                return ValidationResult.Success;
            }
            catch
            {
                return new ValidationResult("Invalid regex pattern.");
            }
        }
    }
}
