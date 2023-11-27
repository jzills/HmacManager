using HmacManagement.Policies;

namespace HmacManagement.Headers;

public class HeaderSchemeValidator : IValidator<HeaderScheme>
{
    public ValidationResult Validate(HeaderScheme validatable)
    {
        if (string.IsNullOrWhiteSpace(validatable.Name))
        {
            return new ValidationResult(isValid: false, 
                new ArgumentException("The \"Name\" property cannot be null, empty or whitespace."));
        }
        else
        {
            return new ValidationResult(isValid: true);
        }
    }
}