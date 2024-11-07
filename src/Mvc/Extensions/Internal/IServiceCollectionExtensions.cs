using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using HmacManager.Caching;
using HmacManager.Common;
using HmacManager.Components;

namespace HmacManager.Mvc.Extensions.Internal;

internal static class IServiceCollectionExtensions
{
    internal static IServiceCollection AddHmacManager(
        this IServiceCollection services,
        HmacManagerOptions options
    )
    {
        services.AddScoped<IHmacHeaderParserFactory, HmacHeaderParserFactory>(_ => 
            new HmacHeaderParserFactory(options.IsConsolidatedHeadersEnabled));

        services.AddScoped<IHmacHeaderBuilderFactory, HmacHeaderBuilderFactory>(_ => 
            new HmacHeaderBuilderFactory(options.IsConsolidatedHeadersEnabled));

        return services
            .AddHttpContextAccessor()
            .AddPolicyCollection(options.GetPolicyCollectionOptions())
            .AddCacheCollection()
            .AddFactory()
            .AddScoped<IHmacAuthenticationContextProvider, HmacAuthenticationContextProvider>()
            .AddSingleton<IAuthorizationHandler, HmacAuthorizationHandler>(); // Add AuthorizationHandler for dynamic policies
    }

    internal static IServiceCollection AddPolicyCollection(
        this IServiceCollection services, 
        HmacPolicyCollectionOptions options
    )
    {
        if (options.IsScopedPoliciesEnabled)
        {
            services.AddScoped(options.PoliciesAccessor);
        }
        else
        {
            services.AddSingleton(options.Policies);
        }

        return services;
    }

    internal static IServiceCollection AddCacheCollection(this IServiceCollection services)
    {
        services.TryAddSingleton<IMemoryCache, MemoryCache>();
        services.TryAddSingleton<IDistributedCache, MemoryDistributedCache>();

        return services.AddScoped<IComponentCollection<INonceCache>, NonceCacheCollection>(GetCacheCollection);
    }

    internal static IServiceCollection AddFactory(
        this IServiceCollection services
    ) => services.AddScoped<IHmacManagerFactory, HmacManagerFactory>();

    private static NonceCacheCollection GetCacheCollection(
        IServiceProvider serviceProvider
    ) => new NonceCacheCollection().AddDefaultCaches(serviceProvider);
}