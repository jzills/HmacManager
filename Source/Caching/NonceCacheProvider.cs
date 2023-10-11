namespace HmacManagement.Caching;

public class NonceCacheProvider : INonceCacheProvider
{
    protected IDictionary<NonceCacheType, INonceCache> Caches;

    public NonceCacheProvider(
        IDictionary<NonceCacheType, INonceCache> caches
    ) => Caches = caches;

    public INonceCache? GetCache(NonceCacheType cacheType)
    {
        if (Caches.ContainsKey(cacheType))
        {
            return Caches[cacheType];
        }
        else
        {
            return default;
        }
    }
}