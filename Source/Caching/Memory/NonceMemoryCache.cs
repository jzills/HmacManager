using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace HmacManagement.Caching.Memory;

public class NonceMemoryCache : INonceCache
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

    public Task SetAsync(Guid nonce, DateTimeOffset requestedOn)
    {
        _cache.Set(
            $"{nameof(NonceMemoryCache)}_{nonce.ToString()}", 
            nonce.ToString(), new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = requestedOn.Add(_options.MaxAge)
            });

        return Task.CompletedTask;
    }

    public Task<bool> ContainsAsync(Guid nonce)
    {
        var cacheNonce = _cache.Get($"{nameof(NonceMemoryCache)}_{nonce.ToString()}");
        return Task.FromResult(cacheNonce is not null);
    }
}