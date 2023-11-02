namespace HmacManagement.Policies;

public class KeyCredentials
{
    public Guid PublicKey { get; set; } = Guid.Empty;
    public string PrivateKey { get; set; } = string.Empty;
}

public class HmacPolicyValidator : IValidator<HmacPolicy>
{
    private readonly NameValidator _nameValidator = new();
    private readonly KeyCredentialsValidator _keysValidator = new();
    public ValidationResult Validate(HmacPolicy validatable) =>
        new ValidationResult(
            _nameValidator.Validate(validatable.Name),
            _keysValidator.Validate(validatable.Keys)
        );
}

public class ValidationResult
{
    public ValidationResult(bool isValid, Exception? error = null)
    {
        IsValid = isValid;
        Error = error;
    }

    public ValidationResult(bool isValid, Exception? error, params ValidationResult[] validationResults) 
        : this(isValid && validationResults.All(validationResult => validationResult.IsValid), error)
    {
        ValidationResultAggregate.AddRange(validationResults);
    }

    public ValidationResult(params ValidationResult[] validationResults) 
        : this(validationResults.All(validationResult => validationResult.IsValid))
    {
        ValidationResultAggregate.AddRange(validationResults);
    }

    public readonly bool IsValid;
    public readonly Exception? Error;
    public readonly List<ValidationResult> ValidationResultAggregate = 
        new List<ValidationResult>();

    public Exception GetError() 
    {
        if (Error is not null)
        {
            return Error;
        }
        else
        {
            return ValidationResultAggregate.First().GetError();
        }
    }
}

public interface IValidator<TValidatable>
{
    ValidationResult Validate(TValidatable validatable);
}

public class NameValidator : IValidator<string>
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