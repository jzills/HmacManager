using System;
using System.Threading.Tasks;

namespace Source.Caching;

public interface INonceCache
{
    Task SetAsync(Guid nonce, DateTimeOffset requestedOn);
    Task<bool> ContainsAsync(Guid nonce);
}