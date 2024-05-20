using HmacManager.Common;

namespace HmacManager.Caching;

internal class NonceCacheCollection : ComponentCollection<INonceCache>
{
    public new void Add(NonceCacheType cacheType, INonceCache cache) => Add(Enum.GetName(cacheType), cache);
}