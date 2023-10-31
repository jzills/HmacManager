namespace HmacManagement.Policies;

public class HmacPolicyCollection : IHmacPolicyCollection
{
    protected IDictionary<string, HmacPolicy> Policies = 
        new Dictionary<string, HmacPolicy>();

    public void AddPolicy(string name, Action<HmacPolicy> configurePolicy)
    {
        var policy = new HmacPolicy();
        configurePolicy.Invoke(policy);

        Policies.Add(name, policy);
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