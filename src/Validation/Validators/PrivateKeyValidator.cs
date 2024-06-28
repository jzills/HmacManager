using HmacManager.Policies;

namespace HmacManager.Validation;

internal class PrivateKeyValidator : IValidator<string>
{
    public ValidationResult Validate(string privateKey)
    {
        if (string.IsNullOrWhiteSpace(privateKey))
        {
            return new ValidationResult(isValid: false,
                new FormatException($"The value for \"{nameof(privateKey)}\" cannot be null, empty or whitespace."));
        }

        var buffer = new Span<byte>(new byte[privateKey.Length]);
        if (!Convert.TryFromBase64String(privateKey, buffer, out _))
        {
            return new ValidationResult(isValid: false,
                new FormatException($"The supplied \"{nameof(privateKey)}\" is not a valid base64 encoded string."));
        }

        return new ValidationResult(isValid: true);
    }
}