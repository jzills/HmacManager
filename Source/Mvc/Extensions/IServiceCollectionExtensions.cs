using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using HmacManagement.Caching;
using HmacManagement.Caching.Distributed;
using HmacManagement.Caching.Memory;
using HmacManagement.Components;

namespace HmacManagement.Mvc.Extensions;

public static class IServiceCollectionExtensions
{
    // public static IServiceCollection AddHmacAuthentication(
    //     this IServiceCollection services,
    //     Action<HmacOptions> configureManagerOptions,
    //     Action<HmacAuthenticationOptions> configureAuthenticationOptions
    // )
    // {
    //     services
    //         .AddHmacManager(configureManagerOptions)
    //         .AddAuthentication()
    //         .AddHmac(configureAuthenticationOptions);

    //     return services;
    // }

    public static IServiceCollection AddHmacManagement(
        this IServiceCollection services,
        Action<HmacOptions> configureOptions
    )
    {
        var options = new HmacOptions();
        configureOptions.Invoke(options);

        EnsureClientCredentials(
            options.ClientId, 
            options.ClientSecret
        );

        var managerOptions = new HmacManagerOptions
        {
            MaxAge = options.MaxAge,
            SignedHeaders = options.SignedHeaders
        };

        return services
            .AddHmacManager(managerOptions)
            .AddHmacProvider(options)
            .AddNonceCache(managerOptions, options.NonceCacheType);
    }

    private static IServiceCollection AddHmacManager(
        this IServiceCollection services, 
        HmacManagerOptions options
    ) =>
        services.AddScoped<IHmacManager, HmacManager>(provider =>
            new HmacManager(options, 
                provider.GetRequiredService<INonceCache>(), 
                provider.GetRequiredService<IHmacProvider>()));

    private static IServiceCollection AddHmacProvider(
        this IServiceCollection services,
        HmacOptions options
    ) => 
        services.AddScoped<IHmacProvider, HmacProvider>(provider => 
            new HmacProvider(new HmacProviderOptions
            {
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
                ContentHashAlgorithm = options.ContentHashAlgorithm,
                SignatureHashAlgorithm = options.SignatureHashAlgorithm
            }));

    private static IServiceCollection AddNonceCache(
        this IServiceCollection services, 
        HmacManagerOptions options,
        NonceCacheType cacheType
    )
    {
        if (cacheType == NonceCacheType.Memory)
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