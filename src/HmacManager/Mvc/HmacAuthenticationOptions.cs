using Microsoft.AspNetCore.Authentication;
using HmacManager.Policies;

namespace HmacManager.Mvc;

public class HmacAuthenticationOptions : AuthenticationSchemeOptions
{
    protected readonly HmacManagerOptions Options = new();

    public new HmacEvents Events { get; set; } = new();

    public void AddPolicy(string name, Action<HmacPolicyBuilder> configurePolicy) => 
        Options.AddPolicy(name, configurePolicy);

    public HmacPolicyCollection GetPolicies() => 
        Options.GetPolicies();

    public HmacManagerOptions GetOptions() => Options;
}