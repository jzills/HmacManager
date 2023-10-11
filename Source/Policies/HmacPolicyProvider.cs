namespace HmacManagement.Policies;

public class HmacPolicyProvider : IHmacPolicyProvider
{
    protected IDictionary<string, HmacPolicy> Policies;

    public HmacPolicyProvider(IDictionary<string, HmacPolicy> policies)
    {
        ArgumentNullException.ThrowIfNull(policies);
        Policies = policies;
    }

    public HmacPolicy? GetPolicy(string name)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));

        if (Policies.ContainsKey(name))
        {
            return Policies[name];
        }
        else
        {
            return default;
        }
    }

    public IReadOnlyCollection<HmacPolicy> GetPolicies() => 
        Policies.Values.ToList().AsReadOnly();
}