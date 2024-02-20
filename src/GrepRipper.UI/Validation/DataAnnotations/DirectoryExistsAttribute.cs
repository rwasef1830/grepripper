using System;
using System.ComponentModel.DataAnnotations;
using GrepRipper.UI.Util;

namespace GrepRipper.UI.Validation.DataAnnotations;

public class DirectoryExistsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(value?.ToString()))
        {
            return ValidationResult.Success;
        }

        if (value is not string path)
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
