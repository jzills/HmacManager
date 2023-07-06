using Microsoft.Extensions.Caching.Distributed;
using Source.Components;

namespace Source.Caching.Distributed;

public class NonceDistributedCache : INonceCache
{
    private readonly IDistributedCache _cache;
    private readonly HMACManagerOptions _options;

    public NonceDistributedCache(
        IDistributedCache cache,
        HMACManagerOptions options
    )
    {
        _cache = cache;
        _options = options;
    }

    public Task SetAsync(Guid nonce, DateTimeOffset requestedOn) =>
        _cache.SetStringAsync(
            nonce.ToString(), 
            nonce.ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = requestedOn.Add(_options.MaxAge)
            });

    public async Task<bool> ContainsAsync(Guid nonce)
    {
        var cacheNonce = await _cache.GetAsync(nonce.ToString());
        return cacheNonce is not null;
    }
}