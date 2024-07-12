using Microsoft.Extensions.Caching.Distributed;

namespace HmacManager.Caching.Distributed;

internal class NonceDistributedCache : NonceCache
{
    protected readonly IDistributedCache Cache;

    public NonceDistributedCache(IDistributedCache cache, NonceCacheOptions options)
        : base(options) => Cache = cache;

    public override Task SetAsync(Guid nonce, DateTimeOffset dateRequested) =>
        Cache.SetStringAsync(
            Key(nonce), 
            dateRequested.ToString(), 
            new DistributedCacheEntryOptions 
                { AbsoluteExpiration = AbsoluteExpiration(dateRequested) }
        );

    public override async Task<bool> ContainsAsync(Guid nonce)
    {
        var value = await Cache.GetAsync(Key(nonce));
        return value is not null;
    }
}