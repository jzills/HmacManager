using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using HmacManager.Caching;
using HmacManager.Caching.Distributed;
using HmacManager.Caching.Memory;
using HmacManager.Components;

namespace HmacManager.Mvc.Extensions;

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

    public static IServiceCollection AddHmacManager(
        this IServiceCollection services,
        Action<HmacOptions> configureOptions
    )
    {
        var options = new HmacOptions();
        configureOptions.Invoke(options);

        ArgumentNullException.ThrowIfNullOrEmpty(options.ClientId,     nameof(options.ClientId));
        ArgumentNullException.ThrowIfNullOrEmpty(options.ClientSecret, nameof(options.ClientSecret));

        var managerOptions = new HmacManagerOptions
        {
            MaxAge = options.MaxAge,
            MessageContentHeaders = options.MessageContentHeaders
        };

        services.AddScoped<IHmacManager, HmacManager.Components.HmacManager>(provider =>
            new HmacManager.Components.HmacManager(managerOptions, 
                provider.GetRequiredService<INonceCache>(), 
                provider.GetRequiredService<IHmacProvider>()));

        services.AddScoped<IHmacProvider, HmacProvider>(provider => 
            new HmacProvider(new HmacProviderOptions
            {
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
                ContentHashAlgorithm = options.ContentHashAlgorithm,
                SignatureHashAlgorithm = options.SignatureHashAlgorithm
            }));

        if (options.NonceCacheType == NonceCacheType.Memory)
        {
            services.AddScoped<INonceCache, NonceMemoryCache>(provider =>
                new NonceMemoryCache(
                    provider.GetRequiredService<IMemoryCache>(),
                    managerOptions
                ));
        }
        else
        {
            services.AddScoped<INonceCache, NonceDistributedCache>(provider =>
                new NonceDistributedCache(
                    provider.GetRequiredService<IDistributedCache>(),
                    managerOptions
                ));
        }

        return services;
    }
}