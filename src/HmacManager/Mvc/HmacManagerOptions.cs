using HmacManager.Policies;

namespace HmacManager.Mvc;

/// <summary>
/// A class representing an instance of <c>HmacManagerOptions</c>..
/// </summary>
public class HmacManagerOptions
{
    protected HmacPolicyCollection Policies = new HmacPolicyCollection();

    /// <summary>
    /// Adds an <c>HmacPolicy</c> with to the <c>HmacPolicyCollection</c>
    /// with the specified name and configuration defined by the <c>HmacPolicyBuilder</c> action.
    /// </summary>
    /// <param name="name">The name of the <c>HmacPolicy</c>.</param>
    /// <param name="configurePolicy">The configuration action for <c>HmacPolicyBuilder</c>.</param>
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

    internal HmacPolicyCollection GetPolicies() => Policies;
}