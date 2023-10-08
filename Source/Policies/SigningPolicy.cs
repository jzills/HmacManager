using HmacManagement.Mvc;
using HmacManagement.Remodel;

namespace HmacManagement.Policies;

public interface ISigningPolicyCollection
{
    void AddPolicy(Action<SigningPolicy> mappings);
    SigningPolicy GetPolicy(string name);
}

public class SigningPolicyCollection : ISigningPolicyCollection
{
    public IDictionary<string, SigningPolicy> _signingPolicies 
        = new Dictionary<string, SigningPolicy>();

    public void AddPolicy(Action<SigningPolicy> configurePolicy)
    {
        var policy = new SigningPolicy();
        configurePolicy.Invoke(policy);

        _signingPolicies.TryAdd(policy.PolicyName, policy);
    }

    public SigningPolicy GetPolicy(string name)
    {
        if (_signingPolicies.ContainsKey(name))
        {
            return _signingPolicies[name];
        }
        else
        {
            throw new Exception();
        }
    }
}

public class SigningPolicy
{
    public string PolicyName { get; set; }
    public HmacPolicy Manager { get; set; }
    public HeaderClaimMappingCollection HeaderClaimMappings { get; init; }
}

public class HeaderClaimMapping
{
    public string HeaderName { get; set; }
    public string ClaimType { get; set; }
}

public class HeaderClaimMappingCollection
{
    private readonly IDictionary<string, string> _mappings 
        = new Dictionary<string, string>();

    public string[] GetRequiredHeaders() => _mappings.Keys.ToArray();

    public HeaderClaimMapping[] GetHeaderClaimMappings() => _mappings.Select(mapping => new HeaderClaimMapping { HeaderName = mapping.Key, ClaimType = mapping.Value }).ToArray();

    public void AddRequiredHeader(string headerName)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(headerName, nameof(headerName));

        var claimType = string.Empty;
        if (headerName.StartsWith("X"))
        {
            claimType = headerName.Substring(1);
        }

        claimType = claimType.Replace("-", string.Empty);

        _mappings.TryAdd(headerName, claimType);
    }

    public void AddRequiredHeader(string headerName, string claimType)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(headerName, nameof(headerName));
        ArgumentNullException.ThrowIfNullOrEmpty(claimType , nameof(claimType));

        _mappings.TryAdd(headerName, claimType);
    }
}