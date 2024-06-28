using Microsoft.Extensions.DependencyInjection;
using HmacManager.Components;

namespace HmacManager.Mvc.Extensions;

/// <summary>
/// A class representing extension methods on an <c>IHttpClientBuilder</c>.
/// </summary>
public static class IHttpClientBuilderExtensions
{
    /// <summary>
    /// Registers an <c>HttpMessageHandler</c> that handles creating
    /// an <c>HmacManager</c> through an implementation of <c>IHmacManagerFactory</c>. 
    /// Outgoing requests are then signed with the
    /// specified policy.
    /// </summary>
    /// <param name="builder">The <c>IHttpClientBuilder</c>.</param>
    /// <param name="policy">The name of the registered policy.</param>
    /// <returns>An <c>IHttpClientBuilder</c> that can be used to configure the client.</returns>
    public static IHttpClientBuilder AddHmacHttpMessageHandler(
        this IHttpClientBuilder builder, 
        string policy
    ) => builder.AddHttpMessageHandler(provider => 
            GetDelegatingHandler(provider, policy));

    /// <summary>
    /// Registers an <c>HttpMessageHandler</c> that handles creating
    /// an <c>HmacManager</c> through an implementation of <c>IHmacManagerFactory</c>. 
    /// Outgoing requests are then signed with the
    /// specified policy and optional scheme.
    /// </summary>
    /// <param name="builder">The <c>IHttpClientBuilder</c>.</param>
    /// <param name="policy">The name of the registered policy.</param>
    /// <param name="scheme">The name of the registered scheme.</param>
    /// <returns>An <c>IHttpClientBuilder</c> that can be used to configure the client.</returns>
    public static IHttpClientBuilder AddHmacHttpMessageHandler(
        this IHttpClientBuilder builder, 
        string policy, 
        string scheme
    ) => builder.AddHttpMessageHandler(provider => 
            GetDelegatingHandler(provider, policy, scheme));

    internal static DelegatingHandler GetDelegatingHandler(
        IServiceProvider serviceProvider,
        string  policy,
        string? scheme = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(policy, nameof(policy));

        var hmacManagerFactory = serviceProvider.GetRequiredService<IHmacManagerFactory>();
        var hmacManager = (policy, scheme) switch
        {
            (_, null) => hmacManagerFactory.Create(policy),
            (_, _)    => hmacManagerFactory.Create(policy, scheme)
        };

        ArgumentNullException.ThrowIfNull(hmacManager, nameof(hmacManager));

        return new HmacDelegatingHandler(hmacManager);
    }
}