using HmacManager.Policies;

namespace HmacManager.Headers;

/// <summary>
/// Validates instances of <see cref="HeaderScheme"/>.
/// </summary>
public class HeaderSchemeValidator : IValidator<HeaderScheme>
{
    /// <summary>
    /// Validates a <see cref="HeaderScheme"/> object.
    /// </summary>
    /// <param name="validatable">The <see cref="HeaderScheme"/> instance to validate.</param>
    /// <returns>A <see cref="ValidationResult"/> indicating whether the validation was successful or not.</returns>
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