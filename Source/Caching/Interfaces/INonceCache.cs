namespace HmacManagement.Caching;

public interface INonceCache
{
    Task SetAsync(Guid nonce, DateTimeOffset DateRequested);
    Task<bool> ContainsAsync(Guid nonce);
}