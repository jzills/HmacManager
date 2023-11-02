using Microsoft.AspNetCore.Authentication;
using HmacManagement.Policies;
using HmacManagement.Mvc;

public class HmacAuthenticationOptions : AuthenticationSchemeOptions
{
    public IComponentCollection<HmacPolicy> Policies = new HmacPolicyCollection();
    public new HmacEvents Events { get; set; } = new();
}