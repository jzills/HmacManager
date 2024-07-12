using Microsoft.Extensions.Caching.Memory;

namespace HmacManager.Caching.Memory;

internal class NonceMemoryCache : NonceCache
{
    protected readonly IMemoryCache Cache;

    public NonceMemoryCache(IMemoryCache cache, NonceCacheOptions options) 
        : base(options) => Cache = cache;

    public override Task SetAsync(Guid nonce, DateTimeOffset dateRequested)
    {
        Cache.Set(
            Key(nonce),
            dateRequested, 
            new MemoryCacheEntryOptions
                { AbsoluteExpiration = AbsoluteExpiration(dateRequested) }
        );

        return Task.CompletedTask;
    }

    public override Task<bool> ContainsAsync(Guid nonce)
    {
        var value = Cache.Get(Key(nonce));
        return Task.FromResult(value is not null);
    }
}