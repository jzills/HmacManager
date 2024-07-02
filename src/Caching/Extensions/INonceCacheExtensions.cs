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
}