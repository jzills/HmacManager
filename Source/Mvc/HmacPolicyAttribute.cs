namespace HmacManager.Mvc;

// TODO: Implement ability to define exact policy/scheme used
// for request signing on a particular controller/action.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class HmacPolicyAttribute : Attribute
{
    private readonly string? _policy;
    private readonly string? _scheme;

    public HmacPolicyAttribute(string policy)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(policy, nameof(policy));
        _policy = policy;
    }

    public HmacPolicyAttribute(string policy, string scheme) 
        : this(policy)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(scheme, nameof(scheme));
        _scheme = scheme;
    }
}