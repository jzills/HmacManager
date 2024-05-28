using HmacManager.Common;

namespace HmacManager.Caching;

public class NonceCacheCollection : ComponentCollection<INonceCache>
{
    public new void Add(NonceCacheType cacheType, INonceCache cache) => Add(Enum.GetName(cacheType), cache);
}