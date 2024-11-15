using Microsoft.Extensions.DependencyInjection;
using HmacManager.Components;

namespace HmacManager.Mvc.Extensions;

/// <summary>
/// A class representing extension methods on an <c>IHttpClientBuilder</c> to add HMAC signing to HTTP requests.
/// </summary>
public static class IHttpClientBuilderExtensions
{
    /// <summary>
    /// Registers an <c>HttpMessageHandler</c> that handles creating an <c>HmacManager</c> through an implementation of <c>IHmacManagerFactory</c>. 
    /// Outgoing requests are then signed with the specified policy.
    /// </summary>
    /// <param name="builder">The <c>IHttpClientBuilder</c> to configure.</param>
    /// <param name="policy">The name of the registered policy to be used for signing the requests.</param>
    /// <returns>An <c>IHttpClientBuilder</c> that can be used to further configure the client.</returns>
    /// <remarks>
    /// This method adds a <c>HttpMessageHandler</c> that automatically signs outgoing HTTP requests
    /// using the specified HMAC policy.
    /// </remarks>
    public static IHttpClientBuilder AddHmacHttpMessageHandler(
        this IHttpClientBuilder builder, 
        string policy
    ) => builder.AddHttpMessageHandler(provider => 
            GetDelegatingHandler(provider, policy));

    /// <summary>
    /// Registers an <c>HttpMessageHandler</c> that handles creating an <c>HmacManager</c> through an implementation of <c>IHmacManagerFactory</c>. 
    /// Outgoing requests are then signed with the specified policy and optional scheme.
    /// </summary>
    /// <param name="builder">The <c>IHttpClientBuilder</c> to configure.</param>
    /// <param name="policy">The name of the registered policy to be used for signing the requests.</param>
    /// <param name="scheme">The name of the registered scheme to be used for signing the requests.</param>
    /// <returns>An <c>IHttpClientBuilder</c> that can be used to further configure the client.</returns>
    /// <remarks>
    /// This method adds a <c>HttpMessageHandler</c> that automatically signs outgoing HTTP requests
    /// using the specified HMAC policy and optional scheme.
    /// </remarks>
    public static IHttpClientBuilder AddHmacHttpMessageHandler(
        this IHttpClientBuilder builder, 
        string policy, 
        string scheme
    ) => builder.AddHttpMessageHandler(provider => 
            GetDelegatingHandler(provider, policy, scheme));

    /// <summary>
    /// Creates a <c>DelegatingHandler</c> that uses the specified policy and optional scheme to sign outgoing HTTP requests.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to resolve services.</param>
    /// <param name="policy">The name of the registered policy to be used for signing the requests.</param>
    /// <param name="scheme">The optional scheme to be used for signing the requests.</param>
    /// <returns>A <c>DelegatingHandler</c> that will sign outgoing HTTP requests with the specified policy and scheme.</returns>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="policy"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="policy"/> and <paramref name="scheme"/> configuration results in a null <see cref="HmacManager"/> instance.</exception>
    internal static DelegatingHandler GetDelegatingHandler(
        IServiceProvider serviceProvider,
        string policy,
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