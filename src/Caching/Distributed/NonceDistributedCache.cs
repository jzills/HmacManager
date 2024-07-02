using Microsoft.Extensions.Caching.Distributed;

namespace HmacManager.Caching.Distributed;

internal class NonceDistributedCache : INonceCache
{
    protected readonly IDistributedCache Cache;
    protected readonly NonceCacheOptions Options;

    public NonceDistributedCache(IDistributedCache cache, NonceCacheOptions options) =>
        (Cache, Options) = (cache, options);

    public Task SetAsync(Guid nonce, DateTimeOffset dateRequested) =>
        Cache.SetStringAsync(
            Options.CreateKey(nonce), 
            dateRequested.ToString(), 
            new DistributedCacheEntryOptions
                { AbsoluteExpiration = dateRequested.AddSeconds(Options.MaxAgeInSeconds) }
        );

    public async Task<bool> ContainsAsync(Guid nonce)
    {
        var value = await Cache.GetAsync(Options.CreateKey(nonce));
        return value is not null;
    }
}