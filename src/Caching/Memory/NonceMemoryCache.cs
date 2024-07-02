using Microsoft.Extensions.Caching.Memory;

namespace HmacManager.Caching.Memory;

internal class NonceMemoryCache : INonceCache
{
    protected readonly IMemoryCache Cache;
    protected readonly NonceCacheOptions Options;

    public NonceMemoryCache(IMemoryCache cache, NonceCacheOptions options) =>
        (Cache, Options) = (cache, options);

    public Task SetAsync(Guid nonce, DateTimeOffset dateRequested)
    {
        Cache.Set(
            Options.CreateKey(nonce),
            dateRequested, 
            new MemoryCacheEntryOptions
                { AbsoluteExpiration = dateRequested.AddSeconds(Options.MaxAgeInSeconds) }
        );

        return Task.CompletedTask;
    }

    public Task<bool> ContainsAsync(Guid nonce)
    {
        var value = Cache.Get(Options.CreateKey(nonce));
        return Task.FromResult(value is not null);
    }
}