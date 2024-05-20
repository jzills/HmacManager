using Microsoft.Extensions.Caching.Memory;
using HmacManager.Caching.Extensions;

namespace HmacManager.Caching.Memory;

internal class NonceMemoryCache : INonceCache
{
    private readonly IMemoryCache _cache;
    private readonly NonceCacheOptions _options;

    public NonceMemoryCache(
        IMemoryCache cache,
        NonceCacheOptions options
    ) 
    {
        _cache = cache;
        _options = options;
    }

    public Task SetAsync(Guid nonce, DateTimeOffset dateRequested)
    {
        _cache.Set(
            this.GetNamespace<NonceMemoryCache>(_options.CacheType, nonce), 
            nonce.ToString(), 
            new MemoryCacheEntryOptions
                { AbsoluteExpiration = dateRequested.Add(_options.MaxAge) }
        );

        return Task.CompletedTask;
    }

    public Task<bool> ContainsAsync(Guid nonce)
    {
        var cacheNonce = _cache.Get(this.GetNamespace<NonceMemoryCache>(_options.CacheType, nonce));
        return Task.FromResult(cacheNonce is not null);
    }
}