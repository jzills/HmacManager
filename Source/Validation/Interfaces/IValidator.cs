namespace HmacManagement.Policies;

public interface IValidator<TValidatable>
{
    ValidationResult Validate(TValidatable validatable);
}