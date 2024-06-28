using HmacManager.Policies;

namespace HmacManager.Headers;

public class HeaderValueValidator : IValidator<HeaderValue>
{
    protected readonly HttpRequestMessage Request;

    public HeaderValueValidator(HttpRequestMessage request) => Request = request;

    public ValidationResult Validate(HeaderValue validatable)
    {
        IEnumerable<string>? values;
        if (!Request.Headers.TryGetValues(validatable.Name, out values))
        {
            return new ValidationResult(isValid: false, 
                new ArgumentException($"The \"{nameof(HeaderValue)}\" with name \"{validatable.Name}\" is not present in the request."));
        }
        else if (values is null)
        {
            return new ValidationResult(isValid: false, 
                new ArgumentException($"The \"{nameof(HeaderValue)}\" with name \"{validatable.Name}\" is present in the request but is missing a value."));
        }

        var value = values.FirstOrDefault();
        if (value is null)
        {
            return new ValidationResult(isValid: false, 
                new ArgumentException($"The \"{nameof(HeaderValue)}\" with name \"{validatable.Name}\" is present in the request but is missing a value."));
        }
        else if (value != validatable.Value)
        {
            return new ValidationResult(isValid: false, 
                new ArgumentException($"The \"{nameof(HeaderValue)}\" with name \"{validatable.Name}\" does not match the value present in the header."));
        }
        else
        {
            return new ValidationResult(isValid: true);
        }
    }
}