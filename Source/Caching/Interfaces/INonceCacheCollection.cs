namespace HmacManagement.Caching;

public interface INonceCacheCollection
{
    void Add(NonceCacheType cacheType, INonceCache cache);
    INonceCache? GetCache(NonceCacheType cacheType);
}