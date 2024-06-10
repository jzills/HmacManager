using HmacManager.Caching;
using HmacManager.Caching.Distributed;
using HmacManager.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace HmacManager.Mvc.Extensions.Internal;

internal static class NonceCacheCollectionExtensions
{
    public static Func<NonceCacheType, NonceCacheOptions> DefaultOptionsAccessor = 
        cacheType => new NonceCacheOptions
        {
            CacheType = cacheType,
            MaxAgeInSeconds = 30
        };

    public static NonceCacheCollection AddDefaultCaches(
        this NonceCacheCollection source, 
        IServiceProvider serviceProvider
    ) => source
            .AddDefaultMemoryCache(serviceProvider)
            .AddDefaultDistributedCache(serviceProvider);

    private static NonceCacheCollection AddDefaultMemoryCache(
        this NonceCacheCollection source, 
        IServiceProvider serviceProvider
    )
    {
        var cache = new NonceMemoryCache(
            serviceProvider.GetRequiredService<IMemoryCache>(), 
            DefaultOptionsAccessor(NonceCacheType.Memory)
        );

        source.Add(NonceCacheType.Memory, cache);
        return source;
    }

    private static NonceCacheCollection AddDefaultDistributedCache(
        this NonceCacheCollection source, 
        IServiceProvider serviceProvider
    )
    {
        var cache = new NonceDistributedCache(
            serviceProvider.GetRequiredService<IDistributedCache>(), 
            DefaultOptionsAccessor(NonceCacheType.Distributed)
        );

        source.Add(NonceCacheType.Distributed, cache);
        return source;
    }
}