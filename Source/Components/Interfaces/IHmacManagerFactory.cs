namespace HmacManager.Components;

/// <summary>
/// A class representing a HmacManagerFactory, which
/// handles the creation of configured HmacManager objects.
/// </summary>
public interface IHmacManagerFactory
{
    /// <summary>
    /// Creates an implementation of IHmacManager based on the specified policy.
    /// </summary>
    /// <param name="policy">Policy Name</param>
    /// <returns>An implementation of IHmacManager, by default, a HmacManager object.</returns>
    IHmacManager Create(string policy);
    /// <summary>
    /// Creates an implementation of IHmacManager based on the specified policy and header scheme.
    /// </summary>
    /// <param name="policy">Policy Name</param>
    /// <param name="headerScheme">Header Scheme Name</param>
    /// <returns>An implementation of IHmacManager, by default, a HmacManager object.</returns>
    IHmacManager Create(string policy, string headerScheme);
}