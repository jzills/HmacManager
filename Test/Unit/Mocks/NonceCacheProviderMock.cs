using HmacManagement.Caching;
using HmacManagement.Remodel;

namespace Unit.Mocks;

public class NonceCacheProviderMock : INonceCacheProvider
{
    public INonceCache Get(NonceCacheType cacheType) => new NonceCacheMock();
}