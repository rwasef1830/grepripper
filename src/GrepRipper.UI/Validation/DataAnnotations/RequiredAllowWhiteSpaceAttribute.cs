using System.ComponentModel.DataAnnotations;

namespace GrepRipper.UI.Validation.DataAnnotations;

public class RequiredAllowWhiteSpaceAttribute : RequiredAttribute
{
    public override bool IsValid(object? value)
    {
        return value is string { Length: > 0 } || base.IsValid(value);
    }
}
