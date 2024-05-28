using HmacManager.Common;
using HmacManager.Validation;

namespace HmacManager.Policies;

public class HmacPolicyCollection : ComponentCollection<HmacPolicy>
{
    protected readonly IValidator<HmacPolicy> Validator;

    public HmacPolicyCollection()
    {
        Validator = new HmacPolicyValidator(); 
    }

    public HmacPolicyCollection(IEnumerable<HmacPolicy> policies) : this()
    {
        foreach (var policy in policies)
        {
            Add(policy);
        }
    }

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