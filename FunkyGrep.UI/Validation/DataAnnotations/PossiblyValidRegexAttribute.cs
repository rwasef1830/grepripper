using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using FunkyGrep.UI.ViewModels;

namespace FunkyGrep.UI.Validation.DataAnnotations
{
    public class PossiblyValidRegexAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext => true;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(validationContext.ObjectInstance is MainWindowViewModel viewModel))
            {
                return ValidationResult.Success;
            }

            if (!viewModel.SearchTextIsRegex)
            {
                return ValidationResult.Success;
            }

            if (value == null)
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
