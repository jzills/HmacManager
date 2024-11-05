namespace HmacManager.Components;

/// <summary>
/// An interface representing an <c>IHmacManager</c>. This contract
/// handles signing outgoing requests and verifying incoming requests.
/// </summary>
public interface IHmacManager
{
    /// <summary>
    /// An instance of <c>HmacManagerOptions</c>.
    /// </summary>
    public HmacManagerOptions Options { get; }

    /// <summary>
    /// Signs a <c>HttpRequestMessage</c> asynchronously.
    /// </summary>
    /// <param name="request"><c>HttpRequestMessage</c></param>
    /// <returns>A <c>HmacResult</c> that contains the success of the operation, the generated signature and associated metadata for the signature.</returns>
    Task<HmacResult> SignAsync(HttpRequestMessage request);
    /// <summary>
    /// Verifies a signed <c>HttpRequestMessage</c> asynchronously.
    /// </summary>
    /// <param name="request"><c>HttpRequestMessage</c></param>
    /// <returns>A <c>HmacResult</c> that contains the success of the operation, the generated signature and associated metadata for the signature.</returns>
    Task<HmacResult> VerifyAsync(HttpRequestMessage request);
}