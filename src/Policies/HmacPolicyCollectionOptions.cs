namespace HmacManager.Policies;

/// <summary>
/// A class representing an instance of <c>HmacPolicyCollectionOptions</c>.
/// </summary>
public class HmacPolicyCollectionOptions
{
    /// <summary>
    /// An implementation of <c>IHmacPolicyCollection</c>.
    /// </summary>
    public IHmacPolicyCollection Policies = new HmacPolicyCollection();

    /// <summary>
    /// An accessor given an <c>IServiceProvider</c> that returns an implementation
    /// of an <c>IHmacPolicyCollection</c>.
    /// </summary> 
    public Func<IServiceProvider, IHmacPolicyCollection>? PoliciesAccessor = null;

    /// <summary>
    /// True if a <see cref="PoliciesAccessor"/> has been initialized with a non null value, otherwise false. 
    /// </summary>
    public bool IsScopedPoliciesEnabled => PoliciesAccessor is not null;
}