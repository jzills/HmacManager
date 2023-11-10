namespace HmacManagement.Policies;

public class KeyCredentialsValidator : IValidator<KeyCredentials>
{
    public ValidationResult Validate(KeyCredentials keys)
    {
        if (keys.PublicKey == Guid.Empty)
        {
            return new ValidationResult(isValid: false,
                new FormatException("The value for \"PublicKey\" cannot be an empty guid."));
        }

        if (string.IsNullOrEmpty(keys.PrivateKey))
        {
            return new ValidationResult(isValid: false,
                new FormatException("The value for \"PublicKey\" cannot be null or empty."));
        }

        var buffer = new Span<byte>(new byte[keys.PrivateKey.Length]);
        if (!Convert.TryFromBase64String(keys.PrivateKey, buffer, out _))
        {
            return new ValidationResult(isValid: false,
                new FormatException("The supplied \"PrivateKey\" is not a valid Base64 encoded string."));
        }

        return new ValidationResult(isValid: true);
    }
}