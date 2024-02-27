namespace HmacManager.Policies;

internal class NameValidator : IValidator<string>
{
    public ValidationResult Validate(string validatable)
    {
        if (string.IsNullOrWhiteSpace(validatable))
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