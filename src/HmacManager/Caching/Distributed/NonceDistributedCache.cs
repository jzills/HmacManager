using Microsoft.Extensions.Caching.Distributed;
using HmacManager.Caching.Extensions;

namespace HmacManager.Caching.Distributed;

internal class NonceDistributedCache : INonceCache
{
    private readonly IDistributedCache _cache;
    private readonly NonceCacheOptions _options;

    public NonceDistributedCache(
        IDistributedCache cache,
        NonceCacheOptions options
    )
    {
        _cache = cache;
        _options = options;
    }

    public Task SetAsync(Guid nonce, DateTimeOffset dateRequested) =>
        _cache.SetStringAsync(
            this.GetNamespace<NonceDistributedCache>(_options.CacheType, nonce), 
            nonce.ToString(), 
            new DistributedCacheEntryOptions
                { AbsoluteExpiration = dateRequested.Add(TimeSpan.FromSeconds(_options.MaxAgeInSeconds)) }
        );

    public async Task<bool> ContainsAsync(Guid nonce)
    {
        var cacheNonce = await _cache.GetAsync(this.GetNamespace<NonceDistributedCache>(_options.CacheType, nonce));
        return cacheNonce is not null;
    }
}