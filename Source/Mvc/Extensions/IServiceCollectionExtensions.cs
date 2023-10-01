using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using HmacManagement.Caching;
using HmacManagement.Caching.Distributed;
using HmacManagement.Caching.Memory;
using HmacManagement.Components;
using HmacManagement.Policies;

namespace HmacManagement.Mvc.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddHmacManagement(
        this IServiceCollection services,
        Action<HmacManagementOptions> configureOptions
    )
    {
        var options = new HmacManagementOptions();
        configureOptions.Invoke(options);

        EnsureClientCredentials(
            options.ClientId, 
            options.ClientSecret
        );

        var managerOptions = new HmacManagerOptions
        {
            MaxAge = options.MaxAge,
        };

        var providerOptions = new HmacProviderOptions
        {
            ClientId = options.ClientId!,
            ClientSecret = options.ClientSecret!,
            ContentHashAlgorithm = options.ContentHashAlgorithm,
            SignatureHashAlgorithm = options.SignatureHashAlgorithm
        };

        var nonceCacheOptions = new NonceCacheOptions
        {
            MaxAge = options.MaxAge,
            Type = options.NonceCacheType
        };

        return services
            .AddHmacManager(managerOptions, new SigningPolicyCollection())
            .AddHmacProvider(providerOptions)
            .AddNonceCache(nonceCacheOptions);
    }

    private static IServiceCollection AddHmacManager(
        this IServiceCollection services, 
        HmacManagerOptions options,
        ISigningPolicyCollection signingPolicies
    ) =>
        services.AddScoped<IHmacManager, HmacManager>(provider =>
            new HmacManager(options, 
                provider.GetRequiredService<INonceCache>(), 
                provider.GetRequiredService<IHmacProvider>(),
                signingPolicies));

    private static IServiceCollection AddHmacProvider(
        this IServiceCollection services,
        HmacProviderOptions options
    ) => 
        services.AddScoped<IHmacProvider, HmacProvider>(provider => 
            new HmacProvider(options));

    private static IServiceCollection AddNonceCache(
        this IServiceCollection services, 
        NonceCacheOptions options
    )
    {
        if (options.Type == NonceCacheType.Memory)
        {
            services.AddScoped<INonceCache, NonceMemoryCache>(provider =>
                new NonceMemoryCache(
                    provider.GetRequiredService<IMemoryCache>(),
                    options
                ));
        }
        else
        {
            services.AddScoped<INonceCache, NonceDistributedCache>(provider =>
                new NonceDistributedCache(
                    provider.GetRequiredService<IDistributedCache>(),
                    options
                ));
        }

        return services;
    }

    private static void EnsureClientCredentials(
        string? clientId, 
        string? clientSecret
    )
    {
        ArgumentNullException.ThrowIfNullOrEmpty(clientId,     nameof(clientId));
        ArgumentNullException.ThrowIfNullOrEmpty(clientSecret, nameof(clientSecret));
    }
}