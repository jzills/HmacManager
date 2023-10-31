using HmacManagement.Caching;

namespace Console;

public class NonceCacheMock : INonceCache
{
    public Task<bool> ContainsAsync(Guid nonce) =>
        Task.FromResult<bool>(false);

    public Task SetAsync(Guid nonce, DateTimeOffset requestedOn) =>
        Task.CompletedTask;
}