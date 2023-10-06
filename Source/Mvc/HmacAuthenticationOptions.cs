using System.Security.Claims;
using HmacManagement.Caching;
using HmacManagement.Components;
using HmacManagement.Mvc;
using HmacManagement.Policies;
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
    private Dictionary<string, Action<HmacOptions>> _policies { get; set; } = new();
    public new HmacEvents Events { get; set; } = new();
    
    public void AddDefaultPolicy(Action<HmacOptions> configurePolicy)
    {

    }
    public void AddPolicy(string name, Action<HmacOptions> configurePolicy)
    {

    }

    public IDictionary<string, HmacOptions> GetPolicies()
    {
        var policies = new Dictionary<string, HmacOptions>();
        foreach (var (policy, configureOptions) in _policies)
        {
            var options = new HmacOptions();
            configureOptions.Invoke(options);

            policies[policy] = options;
        }

        return policies;
    }
}