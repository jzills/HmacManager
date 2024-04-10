namespace HmacManager.Caching;

/// <summary>
/// A class representing basic configurations
/// for a <c>Nonce</c> and the subsequent caching strategies.
/// </summary>
public class Nonce
{
    /// <summary>
    /// Represents the maximum age for a request. 
    /// Nonce values are cached with a TTL
    /// that matches this value.
    /// </summary>
    /// <value>Defaults to 30 seconds.</value>
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(30);
    /// <summary>
    /// Represents the name of the cache used to store nonce values.
    /// </summary>
    /// <value>Defaults to an empty string.</value>
    public string CacheName { get; set; } = string.Empty;
}