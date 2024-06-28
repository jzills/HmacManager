using HmacManager.Policies;

namespace HmacManager.Validation;

internal class PublicKeyValidator : IValidator<Guid>
{
    public ValidationResult Validate(Guid publicKey)
    {
        if (publicKey == Guid.Empty)
        {
            return new ValidationResult(isValid: false,
                new FormatException($"The value for \"{nameof(publicKey)}\" cannot be an empty guid."));
        }

        return new ValidationResult(isValid: true);
    }
}