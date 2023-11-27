using HmacManagement.Common;

namespace HmacManagement.Caching;

public class NonceCacheCollection : ComponentCollection<INonceCache>
{
    public new void Add(string name, INonceCache cache) => base.Add(name, cache);
}