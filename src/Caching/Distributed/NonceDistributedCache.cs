using Microsoft.Extensions.Caching.Distributed;

namespace HmacManager.Caching.Distributed;

internal class NonceDistributedCache : NonceCache
{
    protected readonly IDistributedCache Cache;

    public NonceDistributedCache(IDistributedCache cache, NonceCacheOptions options)
        : base(options) => Cache = cache;

    public override Task SetAsync(Guid nonce, DateTimeOffset dateRequested) =>
        Cache.SetStringAsync(
            Options.CreateKey(nonce), 
            dateRequested.ToString(), 
            new DistributedCacheEntryOptions
                { AbsoluteExpiration = dateRequested.AddSeconds(Options.MaxAgeInSeconds) }
        );

    public override async Task<bool> ContainsAsync(Guid nonce)
    {
        var value = await Cache.GetAsync(Options.CreateKey(nonce));
        return value is not null;
    }
}