namespace HmacManager.Caching.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="INonceCache"/> interface.
/// </summary>
internal static class INonceCacheExtensions
{
    /// <summary>
    /// Checks if the specified nonce is valid and stores it in the cache if valid.
    /// </summary>
    /// <param name="cache">The instance of <see cref="INonceCache"/> to operate on.</param>
    /// <param name="nonce">The unique identifier for the nonce.</param>
    /// <param name="dateRequested">The date and time when the nonce was requested.</param>
    /// <returns>
    /// A task that represents the asynchronous operation, 
    /// containing a boolean indicating whether the nonce is valid (i.e., not already present in the cache).
    /// </returns>
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