namespace HmacManagement.Caching;

public interface INonceCache
{
    Task SetAsync(Guid nonce, DateTimeOffset dateRequested);
    Task<bool> ContainsAsync(Guid nonce);
}