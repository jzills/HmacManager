using HmacManager.Policies;

namespace HmacManager.Mvc;

/// <summary>
/// A class representing an instance of <c>HmacManagerOptions</c>.
/// </summary>
public class HmacManagerOptions
{
    /// <summary>
    /// The policy collection options.
    /// </summary>
    protected readonly HmacPolicyCollectionOptions PolicyCollectionOptions = new HmacPolicyCollectionOptions();

    /// <summary>
    /// Represents the flag toggling consolidated header functionality. If enabled, only one header will be
    /// added to each request that represents the policy and scheme configurations. This value will also be base64 encoded. 
    /// </summary>
    internal bool IsConsolidatedHeadersEnabled = false;

    /// <summary>
    /// Creates an options instance.
    /// </summary>
    public HmacManagerOptions() { }

    internal HmacManagerOptions(IHmacPolicyCollection policies) => PolicyCollectionOptions.Policies = policies;

    /// <summary>
    /// Enables scoped policies using the specified delegate.
    /// </summary>
    /// <param name="policiesAccessor">The accessor used to retrieve an <c>IHmacPolicyCollection</c> implementation.</param> 
    public void EnableScopedPolicies(Func<IServiceProvider, IHmacPolicyCollection> policiesAccessor)
    {
        ArgumentNullException.ThrowIfNull(policiesAccessor, nameof(policiesAccessor));

        PolicyCollectionOptions.PoliciesAccessor = policiesAccessor;
    }

    /// <summary>
    /// Enables scoped policies using the specified delegate.
    /// </summary>
    public void EnableConsolidatedHeaders() => IsConsolidatedHeadersEnabled = true;

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