using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace FunkyGrep.UI.Validation.DataAnnotations;

public class ValidIndexInListMemberAttribute : ValidationAttribute
{
    public string ListMemberName { get; }

    public override bool RequiresValidationContext => true;

    public ValidIndexInListMemberAttribute(string listMemberName)
    {
        this.ListMemberName = listMemberName;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not int index)
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
        if (listObj is not IList list)
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
