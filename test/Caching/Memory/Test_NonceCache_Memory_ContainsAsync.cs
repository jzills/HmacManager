using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using HmacManager.Caching;
using HmacManager.Caching.Memory;

namespace Unit.Tests.Caching.Memory;

[TestFixture]
public class Test_NonceCache_Memory_ContainsAsync : TestServiceCollection
{
    public INonceCache Cache;

    [SetUp]
    public void Init()
    {
        // TODO: Clear memory cache
        // because tests are reusing the DI registration of IMemoryCache
        // and existing entries are interfering with one another.
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

    [Test]
    public async Task Test_SetEmptyNonce_ShouldNotContain()
    {
        await Cache.SetAsync(Guid.NewGuid(), DateTimeOffset.Now);

        Assert.IsFalse(await Cache.ContainsAsync(default));
    }
}