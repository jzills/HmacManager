namespace HmacManager.Caching.Extensions;

internal static class INonceCacheExtensions
{
    public static async Task<bool> IsValidNonceAsync(
        this INonceCache cache, 
        Guid nonce, 
        DateTimeOffset dateRequested
    )
    {
        var isValidNonce = !await cache.ContainsAsync(nonce);
        if (isValidNonce)
        {
            await cache.SetAsync(nonce, dateRequested);
        }

        return isValidNonce;
    }

    public static string GetNamespace<TNonceCache>(
        this INonceCache _,
        NonceCacheType cacheType,
        Guid nonce
    ) where TNonceCache : INonceCache
        => $"HmacManager:{Enum.GetName(cacheType)}:{nonce}";
}