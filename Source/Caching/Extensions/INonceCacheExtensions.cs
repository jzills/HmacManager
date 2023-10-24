namespace HmacManagement.Caching.Extensions;

public static class INonceCacheExtensions
{
    public static async Task<bool> HasValidNonceAsync(
        this INonceCache cache, 
        Guid nonce
    ) => !(await cache.ContainsAsync(nonce));
}