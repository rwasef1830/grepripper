using System;
using System.ComponentModel.DataAnnotations;
using GrepRipper.Engine;

namespace GrepRipper.UI.Validation.DataAnnotations;

public class ValidSpaceSeparatedGlobExpressionsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var spaceSeparatedExpressions = value?.ToString();

        if (string.IsNullOrWhiteSpace(spaceSeparatedExpressions))
        {
            return ValidationResult.Success;
        }

        var expressions = spaceSeparatedExpressions.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var expression in expressions)
        {
            if (!GlobExpression.IsValid(expression))
            {
                return new ValidationResult("Invalid expression.");
            }
        }

        return ValidationResult.Success;
    }
}
