using HmacManager.Policies;

namespace HmacManager.Headers;

public class HeaderValueCollectionValidator : IValidator<IEnumerable<HeaderValue>>
{
    protected readonly HttpRequestMessage Request;
    protected readonly HeaderValueValidator Validator;

    public HeaderValueCollectionValidator(HttpRequestMessage request)
    {
        Request = request;
        Validator = new HeaderValueValidator(Request);
    }

    public ValidationResult Validate(IEnumerable<HeaderValue> headerValues) =>
        new ValidationResult(headerValues.Select(Validator.Validate).ToArray());
}