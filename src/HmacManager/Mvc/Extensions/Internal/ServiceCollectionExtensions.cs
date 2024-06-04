using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using HmacManager.Caching;
using HmacManager.Common;
using HmacManager.Components;
using HmacManager.Policies;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
    ) => services.AddSingleton<IHmacPolicyCollection,  HmacPolicyCollection>(_ => policies);

    internal static IServiceCollection AddCacheCollection(this IServiceCollection services)
    {
        services.TryAddSingleton<IMemoryCache, MemoryCache>();
        services.TryAddSingleton<IDistributedCache, MemoryDistributedCache>();

        return services
            .AddScoped<IComponentCollection<INonceCache>, NonceCacheCollection>(GetCacheCollection);
    }

    internal static IServiceCollection AddFactory(
        this IServiceCollection services
    ) => services.AddScoped<IHmacManagerFactory, HmacManagerFactory>();

    private static NonceCacheCollection GetCacheCollection(
        IServiceProvider serviceProvider
    ) => new NonceCacheCollection().AddDefaultCaches(serviceProvider);
}