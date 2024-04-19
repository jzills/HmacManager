namespace HmacManager.Policies;

public interface IValidator<TValidatable>
{
    ValidationResult Validate(TValidatable validatable);
}