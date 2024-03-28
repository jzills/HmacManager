using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using HmacManager.Caching;
using HmacManager.Caching.Memory;

namespace Unit.Tests.Caching.Memory;

public class Test_NonceCache_Memory_SetAsync : TestServiceCollection
{
    public readonly INonceCache Cache;

    public Test_NonceCache_Memory_SetAsync()
    {
        Cache = new NonceMemoryCache(
            ServiceProvider.GetRequiredService<IMemoryCache>(),
            new NonceCacheOptions()
        );
    }    

    [Test]
    public async Task Test_SetRandomNonce_ShouldContain()
    {
        var nonce = Guid.NewGuid();
        await Cache.SetAsync(nonce, DateTimeOffset.Now);

        Assert.IsTrue(await Cache.ContainsAsync(nonce));
    }

    [Test]
    public async Task Test_SetEmptyNonce_ShouldContain()
    {
        await Cache.SetAsync(default, DateTimeOffset.Now);

        Assert.IsTrue(await Cache.ContainsAsync(default));
    }
}