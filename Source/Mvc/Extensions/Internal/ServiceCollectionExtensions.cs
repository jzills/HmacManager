using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using HmacManager.Caching;
using HmacManager.Caching.Distributed;
using HmacManager.Caching.Memory;
using HmacManager.Common;
using HmacManager.Components;
using HmacManager.Policies;

namespace HmacManager.Mvc.Extensions.Internal;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddHmacManager(
        this IServiceCollection services,
        HmacManagerOptions options
    ) =>
        services
            .AddHttpContextAccessor()
            .AddCacheCollection()
            .AddPolicyCollection(options.GetPolicies())
            .AddFactory();

    internal static IServiceCollection AddPolicyCollection(
        this IServiceCollection services, 
        HmacPolicyCollection policies
    ) => services.AddScoped<IComponentCollection<HmacPolicy>,  HmacPolicyCollection>(_ => policies);

    internal static IServiceCollection AddCacheCollection(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        // TODO: The caches are not configured
        // based on user configuration. Right now,
        // these values are hard coded. Max age and cache name
        // needs to be configurable to support diferent max ages.
        var caches = new NonceCacheCollection();
        var memoryCache = serviceProvider.GetService<IMemoryCache>();
        if (memoryCache is not null)
        {
            caches.Add("InMemory", new NonceMemoryCache(memoryCache, new NonceCacheOptions
            { 
                CacheName = "InMemory",
                MaxAge = TimeSpan.FromMinutes(1) 
            }));
        }

        var distributedCache = serviceProvider.GetService<IDistributedCache>();
        if (distributedCache is not null)
        {
            caches.Add("Distributed", new NonceDistributedCache(distributedCache, new NonceCacheOptions
            {
                CacheName = "Distributed", 
                MaxAge = TimeSpan.FromMinutes(1) 
            }));
        }

        return services.AddScoped<IComponentCollection<INonceCache>, NonceCacheCollection>(_ => caches);
    }

    internal static IServiceCollection AddFactory(
        this IServiceCollection services
    ) => services.AddScoped<IHmacManagerFactory, HmacManagerFactory>();
}