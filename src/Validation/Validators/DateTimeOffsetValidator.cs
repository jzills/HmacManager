using HmacManager.Policies;

namespace HmacManager.Validation;

internal class DateTimeOffsetValidator : IValidator<DateTimeOffset>
{
    public ValidationResult Validate(DateTimeOffset dateTimeOffset)
    {
        if (dateTimeOffset == DateTimeOffset.MinValue)
        {
            return new ValidationResult(isValid: false,
                new FormatException($"The value for \"{nameof(dateTimeOffset)}\" cannot be the minimum value."));
        }

        if (dateTimeOffset == DateTimeOffset.MaxValue)
        {
            return new ValidationResult(isValid: false,
                new FormatException($"The value for \"{nameof(dateTimeOffset)}\" cannot be the maximum value."));
        }

        return new ValidationResult(isValid: true);
    }
}