using HmacManager.Common;

namespace HmacManager.Policies;

public class HmacPolicyCollection
    : ComponentCollection<HmacPolicy>, IConfigurator<HmacPolicy>
{
    protected IValidator<HmacPolicy> Validator => new HmacPolicyValidator();

    public void Add(string name, Action<HmacPolicy> configurePolicy)
    {
        var policy = new HmacPolicy(name);
        configurePolicy.Invoke(policy);

        var validationResult = Validator.Validate(policy);
        if (validationResult.IsValid)
        {
            base.Add(name, policy); 
        }
        else
        {
            throw validationResult.GetError();
        }
    }
}