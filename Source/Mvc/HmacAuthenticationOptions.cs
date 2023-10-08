using System.Security.Claims;
using HmacManagement.Remodel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

public class HmacEvents
{
    public Func<HttpContext, Claim[]>? OnAuthenticationSuccess { get; set; }
    public Func<HttpContext, Exception>? OnAuthenticationFailure { get; set; }
}

public class HmacAuthenticationOptions : AuthenticationSchemeOptions
{
    private Dictionary<string, Action<HmacPolicy>> _policies { get; set; } = new();
    public new HmacEvents Events { get; set; } = new();
    
    public void AddDefaultPolicy(Action<HmacPolicy> configurePolicy)
    {

    }
    public void AddPolicy(string name, Action<HmacPolicy> configurePolicy)
    {

    }

    public HmacPolicy GetPolicy(string name)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));

        if (_policies.TryGetValue(name, out var policyConfigurator))
        {
            var policy = new HmacPolicy();
            policyConfigurator.Invoke(policy);

            return policy;
        }
        else
        {
            throw new KeyNotFoundException();
        }
    }

    public IDictionary<string, HmacPolicy> GetPolicies()
    {
        var policies = new Dictionary<string, HmacPolicy>();
        foreach (var (policy, configureOptions) in _policies)
        {
            var options = new HmacPolicy();
            configureOptions.Invoke(options);

            policies[policy] = options;
        }

        return policies;
    }
}