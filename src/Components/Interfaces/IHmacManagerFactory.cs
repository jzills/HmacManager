namespace HmacManager.Components;

/// <summary>
/// An interface representing an <c>IHmacManagerFactory</c>, which
/// handles the creation of a configured <c>HmacManager</c>.
/// </summary>
public interface IHmacManagerFactory
{
    /// <summary>
    /// Creates an implementation of <c>IHmacManager</c> based on the specified policy.
    /// </summary>
    /// <param name="policy">The name of the policy.</param>
    /// <returns>An implementation of <c>IHmacManager</c>, by default, a <c>HmacManager</c> object.</returns>
    IHmacManager? Create(string policy);
    
    /// <summary>
    /// Creates an implementation of <c>IHmacManager</c> based on the specified policy and header scheme.
    /// </summary>
    /// <param name="policy">The name of the policy.</param>
    /// <param name="scheme">The name of the header scheme.</param>
    /// <returns>An implementation of <c>IHmacManager</c>, by default, a <c>HmacManager</c> object.</returns>
    IHmacManager? Create(string policy, string? scheme);
}