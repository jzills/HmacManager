namespace HmacManagement.Caching.Extensions;

public static class INonceCacheExtensions
{
    public static async Task<bool> HasValidNonceAsync(
        this INonceCache cache, 
        Guid nonce
    ) => !(await cache.ContainsAsync(nonce));

    public static string GetNamespace<TNonceCache>(
        this INonceCache cache,
        string cacheName,
        Guid nonce
    ) where TNonceCache : INonceCache
        => $"HmacManagement:{typeof(TNonceCache).Name}:{cacheName}:{nonce.ToString()}";
}