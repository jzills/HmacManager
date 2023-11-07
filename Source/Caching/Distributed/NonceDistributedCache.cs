using Microsoft.Extensions.Caching.Distributed;

namespace HmacManagement.Caching.Distributed;

public class NonceDistributedCache : INonceCache
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
            $"{nameof(NonceDistributedCache)}_{_options.CacheName}_{nonce.ToString()}", 
            nonce.ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = dateRequested.Add(_options.MaxAge)
            });

    public async Task<bool> ContainsAsync(Guid nonce)
    {
        var cacheNonce = await _cache.GetAsync($"{nameof(NonceDistributedCache)}_{_options.CacheName}_{nonce.ToString()}");
        return cacheNonce is not null;
    }
}