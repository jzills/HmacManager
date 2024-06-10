namespace HmacManager.Caching;

/// <summary>
/// An <c>enum</c> representing a <c>NonceCacheType</c>.
/// </summary>
public enum NonceCacheType
{
    /// <summary>
    /// Represents a cache injected through the <c>IMemoryCache</c> interface.
    /// </summary>
    Memory,
    
    /// <summary>
    /// Represents a cache injected through the <c>IDistributedCache</c> interface.
    /// </summary>
    Distributed
}