using Microsoft.Extensions.Caching.Distributed;
using HmacManager.Components;

namespace HmacManager.Caching.Distributed;

public class NonceDistributedCache : INonceCache
{
    private readonly IDistributedCache _cache;
    private readonly HmacManagerOptions _options;

    public NonceDistributedCache(
        IDistributedCache cache,
        HmacManagerOptions options
    )
    {
        _cache = cache;
        _options = options;
    }

    public Task SetAsync(Guid nonce, DateTimeOffset requestedOn) =>
        _cache.SetStringAsync(
            $"{nameof(NonceDistributedCache)}_{nonce.ToString()}", 
            nonce.ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = requestedOn.Add(_options.MaxAge)
            });

    public async Task<bool> ContainsAsync(Guid nonce)
    {
        var cacheNonce = await _cache.GetAsync($"{nameof(NonceDistributedCache)}_{nonce.ToString()}");
        return cacheNonce is not null;
    }
}