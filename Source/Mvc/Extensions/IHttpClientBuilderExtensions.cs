using Microsoft.Extensions.DependencyInjection;
using HmacManager.Components;
using HmacManager.Mvc;

namespace HmacManager.Mvc.Extensions;

public static class IHttpClientBuilderExtensions
{
    public static IHttpClientBuilder AddHmacHttpMessageHandler(
        this IHttpClientBuilder builder, 
        string policy
    ) => builder.AddHttpMessageHandler(provider => 
            GetDelegatingHandler(provider, policy));

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
        ArgumentNullException.ThrowIfNullOrWhiteSpace(policy, nameof(policy));

        var hmacManagerFactory = serviceProvider.GetRequiredService<IHmacManagerFactory>();
        var hmacManager = (policy, scheme) switch
        {
            (_, null) => hmacManagerFactory.Create(policy),
            (_, _)    => hmacManagerFactory.Create(policy, scheme)
        };

        return new HmacDelegatingHandler(hmacManager);
    }
}