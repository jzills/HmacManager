using HmacManagement.Caching;

namespace Console;

public class NonceCacheMock1 : INonceCache
{
    public Task<bool> ContainsAsync(Guid nonce) =>
        Task.FromResult<bool>(false);

    public Task SetAsync(Guid nonce, DateTimeOffset DateRequested) =>
        Task.CompletedTask;
}

public class NonceCacheMock2 : INonceCache
{
    public Task<bool> ContainsAsync(Guid nonce) =>
        Task.FromResult<bool>(false);

    public Task SetAsync(Guid nonce, DateTimeOffset DateRequested) =>
        Task.CompletedTask;
}

public class NonceCacheMock3 : INonceCache
{
    public Task<bool> ContainsAsync(Guid nonce) =>
        Task.FromResult<bool>(false);

    public Task SetAsync(Guid nonce, DateTimeOffset DateRequested) =>
        Task.CompletedTask;
}