using System;
using System.ComponentModel.DataAnnotations;
using GrepRipper.Engine;

namespace GrepRipper.UI.Validation.DataAnnotations;

public class ValidSpaceSeparatedGlobExpressionsAttribute : ValidationAttribute
{
    static readonly char[] s_Separators = [' '];

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var spaceSeparatedExpressions = value?.ToString();

        if (string.IsNullOrWhiteSpace(spaceSeparatedExpressions))
        {
            return ValidationResult.Success;
        }

        var expressions = spaceSeparatedExpressions.Split(s_Separators, StringSplitOptions.RemoveEmptyEntries);
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
