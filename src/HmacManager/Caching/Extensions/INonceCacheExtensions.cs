namespace HmacManager.Caching.Extensions;

internal static class INonceCacheExtensions
{
    public static async Task<bool> HasValidNonceAsync(
        this INonceCache cache, 
        Guid nonce
    ) => !await cache.ContainsAsync(nonce);

    public static string GetNamespace<TNonceCache>(
        this INonceCache _,
        string cacheName,
        Guid nonce
    ) where TNonceCache : INonceCache
        => $"HmacManager:{cacheName}:{nonce}";
}