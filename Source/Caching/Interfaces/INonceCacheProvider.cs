namespace HmacManagement.Caching;

public interface INonceCacheProvider
{
    INonceCache? GetCache(NonceCacheType cacheType);
}