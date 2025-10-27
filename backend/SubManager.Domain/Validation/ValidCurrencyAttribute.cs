using SubManager.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace SubManager.Domain.Validation
{
    public class ValidCurrencyAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is string currency && CurrencyConstants.AcceptedCurrencies.Contains(currency))
                return ValidationResult.Success;

            return new ValidationResult("Invalid currency code");
        }
    }
}
