using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Source.Components;

namespace Source.Caching.Memory;

public class NonceMemoryCache : INonceCache
{
    private readonly IMemoryCache _cache;
    private readonly HMACManagerOptions _options;

    public NonceMemoryCache(
        IMemoryCache cache,
        HMACManagerOptions options
    )
    {
        _cache = cache;
        _options = options;
    }

    public Task SetAsync(Guid nonce, DateTimeOffset requestedOn)
    {
        _cache.Set(
            nonce.ToString(), 
            nonce.ToString(), new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = requestedOn.Add(_options.MaxAge)
            });

        return Task.CompletedTask;
    }

    public Task<bool> ContainsAsync(Guid nonce)
    {
        var cacheNonce = _cache.Get(nonce.ToString());
        return Task.FromResult(cacheNonce is not null);
    }
}