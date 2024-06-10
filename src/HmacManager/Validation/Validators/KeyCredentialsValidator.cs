using HmacManager.Policies;

namespace HmacManager.Validation;

internal class KeyCredentialsValidator : IValidator<KeyCredentials>
{
    private readonly PublicKeyValidator _publicKeyValidator = new();
    private readonly PrivateKeyValidator _privateKeyValidator = new();
    public ValidationResult Validate(KeyCredentials validatable) =>
        new ValidationResult(
            _publicKeyValidator.Validate(validatable.PublicKey),
            _privateKeyValidator.Validate(validatable.PrivateKey)
        );
}