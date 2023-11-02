using HmacManagement.Policies;
using Microsoft.Extensions.Caching.Memory;

namespace HmacManagement.Caching;

// public class NonceCacheCollection : INonceCacheCollection
// {
//     protected IDictionary<NonceCacheType, INonceCache> Caches = 
//         new Dictionary<NonceCacheType, INonceCache>();

//     public void Add(NonceCacheType cacheType, INonceCache cache)
//     {
//         if (!Caches.ContainsKey(cacheType))
//         {
//             Caches.Add(cacheType, cache);
//         }
//         else
//         {
//             throw new Exception("Only one cache per cache type can be registered.");
//         }
//     }

//     public INonceCache? GetCache(NonceCacheType cacheType)
//     {
//         if (Caches.ContainsKey(cacheType))
//         {
//             return Caches[cacheType];
//         }
//         else
//         {
//             return default;
//         }
//     }
// }

public class NonceCacheCollection : ComponentCollection<INonceCache>
{
    public new void Add(string name, INonceCache cache)
    {
        base.Add(name, cache);
    }
}