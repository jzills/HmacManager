using HmacManager.Policies;

namespace HmacManager.Validation;

internal class NameValidator : IValidator<string?>
{
    public ValidationResult Validate(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new ValidationResult(isValid: false, 
                new ArgumentException($"The \"{nameof(name)}\" cannot be null, empty or whitespace."));
        }
        else
        {
            return new ValidationResult(isValid: true);
        }
    }
}