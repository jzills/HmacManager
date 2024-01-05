namespace HmacManager.Components;

/// <summary>
/// A class representing a HmacManager, which
/// handles signing outgoing requests and verifying
/// incoming requests.
/// </summary>
public interface IHmacManager
{
    /// <summary>
    /// Signs a HttpRequestMessage asynchronously.
    /// </summary>
    /// <param name="request">HttpRequestMessage</param>
    /// <returns>A HmacResult that contains the success of the operation, the generated signature and associated metadata for the signature.</returns>
    Task<HmacResult> SignAsync(HttpRequestMessage request);
    /// <summary>
    /// Verifies a signed HttpRequestMessage asynchronously.
    /// </summary>
    /// <param name="request">HttpRequestMessage</param>
    /// <returns>A HmacResult that contains the success of the operation, the generated signature and associated metadata for the signature.</returns>
    Task<HmacResult> VerifyAsync(HttpRequestMessage request);
}