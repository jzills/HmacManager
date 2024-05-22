using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using HmacManager.Caching;
using HmacManager.Caching.Memory;

namespace Unit.Tests.Caching.Memory;

[TestFixture(1)]
[TestFixture(5)]
[TestFixture(10)]
[TestFixture(20)]
[TestFixture(300)]
[TestFixture(int.MaxValue)]
public class Test_NonceCache_Memory_ContainsAsync : TestServiceCollection
{
    public INonceCache Cache;
    public readonly int MaxAgeInSeconds;

    public Test_NonceCache_Memory_ContainsAsync(int maxAgeInSeconds) => MaxAgeInSeconds = maxAgeInSeconds;

    [SetUp]
    public void Init()
    {
        Cache = new NonceMemoryCache(
            new MemoryCache(Options.Create(new MemoryCacheOptions())),
            new NonceCacheOptions { MaxAgeInSeconds = MaxAgeInSeconds }
        );
    }    

    [Test]
    [TestCaseSource(typeof(TestCaseData), nameof(TestCaseData.GetNonces))]
    public async Task Test_ContainsAsync_Exists(Guid nonce)
    {
        await Cache.SetAsync(nonce, DateTimeOffset.Now.AddSeconds(MaxAgeInSeconds));
        
        Assert.IsTrue(await Cache.ContainsAsync(nonce));
    }

    [Test]
    [TestCaseSource(typeof(TestCaseData), nameof(TestCaseData.GetNonces))]
    public async Task Test_ContainsAsync_DoesNotExist(Guid nonce)
    {
        Assert.IsFalse(await Cache.ContainsAsync(nonce));
    }
}