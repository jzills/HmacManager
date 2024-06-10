using HmacManager.Policies;

namespace HmacManager.Validation.Extensions;

public static class IValidationExtensions
{
    public static void ValidateOrThrow<T>(this IValidator<T> validator, T validatable)
    {
        var validationResult = validator.Validate(validatable);
        if (!validationResult.IsValid)
        {
            throw validationResult.GetError();
        }
    }
}