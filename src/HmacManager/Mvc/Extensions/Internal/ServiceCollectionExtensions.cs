using Microsoft.AspNetCore.Authorization;
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
            .AddFactory()
            .AddSingleton<IAuthorizationHandler, HmacAuthorizationHandler>(); // Add AuthorizationHandler for dynamic policies

    internal static IServiceCollection AddPolicyCollection(
        this IServiceCollection services, 
        HmacPolicyCollection policies
    ) => services.AddScoped<IComponentCollection<HmacPolicy>,  HmacPolicyCollection>(_ => policies);

    internal static IServiceCollection AddCacheCollection(this IServiceCollection services)
    {
        var caches = new NonceCacheCollection();

        return services
            .AddMemoryCacheIfNotExists(caches)
            .AddDistributedCacheIfNotExists(caches)
            .AddScoped<IComponentCollection<INonceCache>, NonceCacheCollection>(_ => caches);
    }

    internal static IServiceCollection AddFactory(
        this IServiceCollection services
    ) => services.AddScoped<IHmacManagerFactory, HmacManagerFactory>();

    internal static IServiceCollection AddMemoryCacheIfNotExists(this IServiceCollection services, NonceCacheCollection caches)
    {
        var serviceProvider = services.BuildServiceProvider();

        // Add memory cache if it isn't registered
        var memoryCache = serviceProvider.GetService<IMemoryCache>();
        if (memoryCache is null)
        {
            services.AddMemoryCache();
            serviceProvider = services.BuildServiceProvider();
            memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        }

        caches.Add("Memory", new NonceMemoryCache(memoryCache, new NonceCacheOptions
        { 
            CacheName = "Memory",
            MaxAge = TimeSpan.FromMinutes(1) 
        }));

        return services;
    }

    internal static IServiceCollection AddDistributedCacheIfNotExists(this IServiceCollection services, NonceCacheCollection caches)
    {
        var serviceProvider = services.BuildServiceProvider();

        // Add distributed cache if it isn't registered
        var distributedCache = serviceProvider.GetService<IDistributedCache>();
        if (distributedCache is null)
        {
            services.AddDistributedMemoryCache();
            serviceProvider = services.BuildServiceProvider();
            distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();
        }

        caches.Add("Distributed", new NonceDistributedCache(distributedCache, new NonceCacheOptions
        {
            CacheName = "Distributed", 
            MaxAge = TimeSpan.FromMinutes(1) 
        }));

        return services;
    }
}