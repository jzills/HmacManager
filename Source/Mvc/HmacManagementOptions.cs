using HmacManagement.Policies;

namespace HmacManagement.Mvc;

public class HmacManagementOptions
{
    protected HmacPolicyCollection Policies = new HmacPolicyCollection();

    public void AddPolicy(string name, Action<HmacPolicyBuilder> configurePolicy)
    {
        var builder = new HmacPolicyBuilder();
        configurePolicy.Invoke(builder);

        var internalPolicy = builder.Build();
        Policies.Add(name, policy =>
        {
            policy.Keys = internalPolicy.Keys;
            policy.Algorithms = internalPolicy.Algorithms;
            policy.Nonce = internalPolicy.Nonce;
            policy.HeaderSchemes = internalPolicy.HeaderSchemes;
        });
    }

    public HmacPolicyCollection GetPolicies() => Policies;
}