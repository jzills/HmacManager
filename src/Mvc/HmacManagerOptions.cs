using HmacManager.Policies;

namespace HmacManager.Mvc;

public class HmacPolicyCollectionOptions
{
    public IHmacPolicyCollection Policies = new HmacPolicyCollection();
    public Func<IServiceProvider, IHmacPolicyCollection> PoliciesAccessor;
    public bool IsScopedPoliciesEnabled = false;
}

/// <summary>
/// A class representing an instance of <c>HmacManagerOptions</c>.
/// </summary>
public class HmacManagerOptions
{
    protected readonly HmacPolicyCollectionOptions PolicyCollectionOptions = new HmacPolicyCollectionOptions();

    public HmacManagerOptions() { }

    internal HmacManagerOptions(IHmacPolicyCollection policies) => PolicyCollectionOptions.Policies = policies;

    public void EnableScopedPolicies(Func<IServiceProvider, IHmacPolicyCollection> policiesAccessor)
    {
        ArgumentNullException.ThrowIfNull(policiesAccessor, nameof(policiesAccessor));

        PolicyCollectionOptions.PoliciesAccessor = policiesAccessor;
        PolicyCollectionOptions.IsScopedPoliciesEnabled = true;
    }

    /// <summary>
    /// Adds an <c>HmacPolicy</c> to the <c>HmacPolicyCollection</c>
    /// with the specified name and configuration defined by the <c>HmacPolicyBuilder</c> action.
    /// </summary>
    /// <param name="name">The name of the <c>HmacPolicy</c>.</param>
    /// <param name="configurePolicy">The configuration action for <c>HmacPolicyBuilder</c>.</param>
    public void AddPolicy(string name, Action<HmacPolicyBuilder> configurePolicy)
    {
        var builder = new HmacPolicyBuilder();
        configurePolicy.Invoke(builder);

        var policy = builder.Build(name);
        PolicyCollectionOptions.Policies.Add(policy);
    }

    internal HmacPolicyCollectionOptions GetPolicyCollectionOptions() => PolicyCollectionOptions;
}