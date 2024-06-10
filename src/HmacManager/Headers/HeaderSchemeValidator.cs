using HmacManager.Policies;

namespace HmacManager.Headers;

public class HeaderSchemeValidator : IValidator<HeaderScheme>
{
    public ValidationResult Validate(HeaderScheme validatable)
    {
        if (string.IsNullOrWhiteSpace(validatable.Name))
        {
            return new ValidationResult(isValid: false, 
                new ArgumentException("The \"Name\" property cannot be null, empty or whitespace."));
        }
        
        if (validatable.Headers.Count == 0)
        {
            return new ValidationResult(isValid: false, 
                new ArgumentException("The \"Headers\" property cannot be an empty collection."));
        }
        else
        {
            return new ValidationResult(isValid: true);
        }
    }
}