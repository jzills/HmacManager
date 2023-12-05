using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using HmacManagement.Caching;
using HmacManagement.Caching.Distributed;
using HmacManagement.Caching.Memory;
using HmacManagement.Common;
using HmacManagement.Components;
using HmacManagement.Policies;

namespace HmacManagement.Mvc.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHmacManagement(
        this IServiceCollection services, 
        Action<HmacManagementOptions> configureOptions
    )
    {
        var options = new HmacManagementOptions();
        configureOptions.Invoke(options);

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

        services.AddScoped<IComponentCollection<INonceCache>, NonceCacheCollection>(_ => caches);
        services.AddScoped<IComponentCollection<HmacPolicy>,  HmacPolicyCollection>(_ => options.GetPolicies());
        services.AddScoped<IHmacManagerFactory, HmacManagerFactory>();
        
        return services;
    }
}