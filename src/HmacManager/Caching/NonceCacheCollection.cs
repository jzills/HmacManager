using HmacManager.Common;

namespace HmacManager.Caching;

public class NonceCacheCollection : ComponentCollection<INonceCache>
{
    public new void Add(string name, INonceCache cache) => base.Add(name, cache);
}