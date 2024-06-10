using HmacManager.Policies;

namespace HmacManager.Validation;

internal class NonceValidator : IValidator<Guid>
{
    public ValidationResult Validate(Guid nonce)
    {
        if (nonce == Guid.Empty)
        {
            return new ValidationResult(isValid: false,
                new FormatException($"The value for \"{nameof(nonce)}\" cannot be an empty guid."));
        }

        return new ValidationResult(isValid: true);
    }
}