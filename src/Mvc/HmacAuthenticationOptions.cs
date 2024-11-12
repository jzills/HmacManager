using Microsoft.AspNetCore.Authentication;
using HmacManager.Policies;

namespace HmacManager.Mvc;

/// <summary>
/// A class representing <c>HmacAuthenticationOptions</c>.
/// </summary>
public class HmacAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// Creates an instance of <c>HmacAuthenticationOptions</c>.
    /// </summary>
    public HmacAuthenticationOptions()
    {
    }

    internal HmacAuthenticationOptions(IHmacPolicyCollection policies) =>
        Options = new HmacManagerOptions(policies);

    /// <summary>
    /// The options for configuring an <c>HmacManager</c> instance.
    /// </summary>
    protected readonly HmacManagerOptions Options = new();

    /// <summary>
    /// The available <c>HmacEvents</c> for authentication handling.
    /// </summary>
    public new HmacEvents Events { get; set; } = new();

    /// <summary>
    /// Enables scoped policies using the specified delegate.
    /// </summary>
    /// <param name="policiesAccessor">The accessor used to retrieve an <c>IHmacPolicyCollection</c> implementation.</param> 
    public void EnableScopedPolicies(Func<IServiceProvider, IHmacPolicyCollection> policiesAccessor) =>
        Options.EnableScopedPolicies(policiesAccessor);

    /// <summary>
    /// Enables consolidated base64 encoded hmac headers. Instead of having a header for each
    /// hmac header value, i.e. Hmac-Policy, Hmac-Scheme, Hmac-DateRequested and so on, they will be
    /// rolled up into a single header.  
    /// </summary>
    public void EnableConsolidatedHeaders() => Options.EnableConsolidatedHeaders();

    /// <summary>
    /// Adds an <c>HmacPolicy</c> to the <c>HmacPolicyCollection</c>
    /// with the specified name and configuration defined by the <c>HmacPolicyBuilder</c> action.
    /// </summary>
    /// <param name="name">The name of the <c>HmacPolicy</c>.</param>
    /// <param name="configurePolicy">The configuration action for <c>HmacPolicyBuilder</c>.</param>
    public void AddPolicy(string name, Action<HmacPolicyBuilder> configurePolicy) => 
        Options.AddPolicy(name, configurePolicy);

    /// <summary>
    /// Gets the options for this authentication.
    /// </summary>
    /// <returns>The options.</returns> 
    internal HmacManagerOptions GetOptions() => Options;
}