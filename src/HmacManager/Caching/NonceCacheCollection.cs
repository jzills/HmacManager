using HmacManager.Common;

namespace HmacManager.Caching;

/// <summary>
/// An <c>class</c> representing a <c>NonceCacheCollection</c>.
/// </summary>
public class NonceCacheCollection : ComponentCollection<INonceCache>
{
    /// <summary>
    /// Adds an <c>INonceCache</c> specified by <c>NonceCacheType</c>.
    /// </summary>
    /// <param name="cacheType">A <c>NonceCacheType</c>.</param>
    /// <param name="cache">A <c>INonceCache</c>.</param>
    public void Add(NonceCacheType cacheType, INonceCache cache) => 
        Add(Enum.GetName(cacheType)!, cache);
}