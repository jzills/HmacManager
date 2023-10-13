using HmacManagement.Caching;

namespace Unit.Mocks;

public class NonceCacheProviderMock : INonceCacheProvider
{
    public INonceCache? GetCache(NonceCacheType cacheType) => new NonceCacheMock();
}