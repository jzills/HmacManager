using Microsoft.AspNetCore.Authentication;
using HmacManagement.Policies;

namespace HmacManagement.Mvc;

public class HmacAuthenticationOptions : AuthenticationSchemeOptions
{
    protected readonly HmacManagementOptions Options = new();

    public new HmacEvents Events { get; set; } = new();

    public void AddPolicy(string name, Action<HmacPolicyBuilder> configurePolicy) => 
        Options.AddPolicy(name, configurePolicy);

    public HmacPolicyCollection GetPolicies() => 
        Options.GetPolicies();

    public HmacManagementOptions GetOptions() => Options;
}