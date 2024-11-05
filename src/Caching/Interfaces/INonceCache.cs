namespace HmacManager.Caching;

/// <summary>
/// Defines methods for managing nonces in a cache.
/// </summary>
public interface INonceCache
{
    /// <summary>
    /// Stores a nonce along with the date and time it was requested.
    /// </summary>
    /// <param name="nonce">The unique identifier for the nonce.</param>
    /// <param name="dateRequested">The date and time when the nonce was requested.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetAsync(Guid nonce, DateTimeOffset dateRequested);

    /// <summary>
    /// Checks if the specified nonce exists in the cache.
    /// </summary>
    /// <param name="nonce">The unique identifier for the nonce to check.</param>
    /// <returns>A task that represents the asynchronous operation, containing a boolean indicating whether the nonce exists.</returns>
    Task<bool> ContainsAsync(Guid nonce);
}