using HmacManagement.Caching;

namespace Unit.Mocks;

public class NonceCacheCollectionMock : INonceCacheCollection
{
    public void Add(NonceCacheType cacheType, INonceCache cache)
    {
    }

    public INonceCache? GetCache(NonceCacheType cacheType) => new NonceCacheMock();
}