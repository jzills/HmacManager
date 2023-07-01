using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Source.Caching;
using Source.Caching.Distributed;
using Source.Caching.Memory;
using Source.Components;

namespace Source.Mvc.Extensions;

public class HMACManagerConfiguration
{
    public string ClientId { get; set; } 
    public string ClientSecret { get; set; }
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(30);
    public string[] AdditionalContentHeaders { get; set; }
}

public static class IServiceCollectionExtensions
{
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
            AdditionalContentHeaders = options.AdditionalContentHeaders
        };

        services.AddScoped<IHMACManager, HMACManager>(provider =>
            new HMACManager(managerOptions, 
                provider.GetRequiredService<INonceCache>(), 
                provider.GetRequiredService<IHMACProvider>()));

        services.AddScoped<IHMACProvider, HMACProvider>(provider => 
            new HMACProvider(new HMACProviderOptions
            {
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret
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