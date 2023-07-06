using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Source.Caching;
using Source.Caching.Distributed;
using Source.Caching.Memory;
using Source.Components;

namespace Source.Mvc.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddHMACAuthentication(
        this IServiceCollection services,
        Action<HMACManagerConfiguration> configureManagerOptions,
        Action<HMACAuthenticationOptions> configureAuthenticationOptions
    )
    {
        services
            .AddHMACManager(configureManagerOptions)
            .AddAuthentication()
            .AddHMAC(configureAuthenticationOptions);

        return services;
    }

    public static IServiceCollection AddHMACManager(
        this IServiceCollection services,
        Action<HMACManagerConfiguration> configureOptions
    )
    {
        var options = new HMACManagerConfiguration();
        configureOptions.Invoke(options);

        var managerOptions = new HMACManagerOptions
        {
            MaxAge = options.MaxAge,
            MessageContentHeaders = options.MessageContentHeaders
        };

        services.AddScoped<IHMACManager, HMACManager>(provider =>
            new HMACManager(managerOptions, 
                provider.GetRequiredService<INonceCache>(), 
                provider.GetRequiredService<IHMACProvider>()));

        services.AddScoped<IHMACProvider, HMACProvider>(provider => 
            new HMACProvider(new HMACProviderOptions
            {
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
                ContentHashAlgorithm = options.ContentHashAlgorithm,
                SignatureHashAlgorithm = options.SignatureHashAlgorithm
            }));

        services.AddScoped<INonceCache, NonceMemoryCache>(provider =>
            new NonceMemoryCache(
                provider.GetRequiredService<IMemoryCache>(),
                managerOptions
            ));
            
        // services.AddScoped<INonceCache, NonceDistributedCache>();

        return services;
    }
}