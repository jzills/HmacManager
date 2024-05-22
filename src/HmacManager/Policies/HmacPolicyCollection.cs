using HmacManager.Common;
using HmacManager.Validation;

namespace HmacManager.Policies;

public class HmacPolicyCollection : ComponentCollection<HmacPolicy>
{
    protected IValidator<HmacPolicy> Validator => new HmacPolicyValidator();

    public void Add(HmacPolicy policy)
    {
        var validationResult = Validator.Validate(policy);
        if (validationResult.IsValid)
        {
            Add(policy.Name, policy); 
        }
        else
        {
            throw validationResult.GetError();
        }
    }
}