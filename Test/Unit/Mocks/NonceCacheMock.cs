using HmacManagement.Caching;

namespace Unit.Mocks;

public class NonceCacheMock : INonceCache
{
    public Task<bool> ContainsAsync(Guid nonce)
    {
        return Task.FromResult<bool>(false);
    }

    public Task SetAsync(Guid nonce, DateTimeOffset dateRequested)
    {
        return Task.CompletedTask;
    }
}