using Microsoft.Extensions.DependencyInjection;
using Source.Caching;
using Source.Caching.Distributed;
using Source.Caching.Memory;
using Source.Components;

namespace Source.Mvc.Extensions;

public static class IServiceCollectionExtensions
{
    // public static IServiceCollection AddHMACManager(
    //     this IServiceCollection services,
    //     Action<HMACManagerOptions> configureOptions,
    //     Action<HMACProviderOptions> configureProviderOptions
    // )
    // {
    //     services.AddScoped<IHMACManager, HMACManager>(provider =>
    //     {
    //         var nonceCache = provider.GetRequiredService<INonceCache>();
    //         var hmacProvider = provider.GetRequiredService<IHMACProvider>();

    //         var options = new HMACManagerOptions();
    //         configureOptions.Invoke(options);

    //         return new HMACManager(options, nonceCache, hmacProvider);
    //     });

    //     services.AddScoped<IHMACProvider, HMACProvider>(provider =>
    //     {
    //         var options = new HMACProviderOptions();
    //         configureProviderOptions.Invoke(options);

    //         return new HMACProvider(options);
    //     });

    //     services.AddScoped<INonceCache, NonceMemoryCache>();
    //     services.AddScoped<INonceCache, NonceDistributedCache>();
    // }
}