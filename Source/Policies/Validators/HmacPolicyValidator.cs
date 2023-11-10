namespace HmacManagement.Policies;

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