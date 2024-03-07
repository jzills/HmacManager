using HmacManager.Policies;

namespace HmacManager.Headers;

public class HeaderValidator : IValidator<Header>
{
    public ValidationResult Validate(Header validatable)
    {
        if (string.IsNullOrWhiteSpace(validatable.Name))
        {
            return new ValidationResult(isValid: false, 
                new ArgumentException("The \"Name\" property cannot be null, empty or whitespace."));
        }
        else if (string.IsNullOrWhiteSpace(validatable.ClaimType))
        {
            return new ValidationResult(isValid: false, 
                new ArgumentException("The \"ClaimType\" property cannot be null, empty or whitespace."));
        }
        else
        {
            return new ValidationResult(isValid: true);
        }
    }
}