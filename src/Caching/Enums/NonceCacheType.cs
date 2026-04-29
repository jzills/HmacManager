namespace HmacManager.Caching;

/// <summary>
/// An <c>enum</c> representing a <see cref="NonceCacheType"/>.
/// </summary>
public enum NonceCacheType
{
    /// <summary>
    /// Represents a cache injected through the <see cref="IMemoryCache"/> interface.
    /// </summary>
    Memory,
    
    /// <summary>
    /// Represents a cache injected through the <see cref="IDistributedCache"/> interface.
    /// </summary>
    Distributed
}