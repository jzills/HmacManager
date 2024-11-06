using HmacManager.Components;
using HmacManager.Exceptions;

namespace HmacManager.Mvc;

/// <summary>
/// A delegating handler that adds HMAC authentication to outgoing HTTP requests.
/// </summary>
public class HmacDelegatingHandler : DelegatingHandler
{
    private readonly IHmacManager _hmacManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="HmacDelegatingHandler"/> class.
    /// </summary>
    /// <param name="hmacManager">An instance of <see cref="IHmacManager"/> to sign the request.</param>
    public HmacDelegatingHandler(IHmacManager hmacManager) => _hmacManager = hmacManager;

    /// <summary>
    /// Sends the HTTP request synchronously, signing it before sending.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The HTTP response message returned by the base handler.</returns>
    /// <exception cref="HmacSigningException">Thrown when the HMAC signing fails.</exception>
    protected override HttpResponseMessage Send(
        HttpRequestMessage request, 
        CancellationToken cancellationToken
    ) => SendAsync(request, cancellationToken).Result;

    /// <summary>
    /// Asynchronously sends the HTTP request, signing it before sending.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the HTTP response message returned by the base handler.</returns>
    /// <exception cref="HmacSigningException">Thrown when the HMAC signing fails.</exception>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var signingResult = await _hmacManager.SignAsync(request);
        if (signingResult.IsSuccess)
        {
            return await base.SendAsync(request, cancellationToken);
        }
        else
        {
            throw new HmacSigningException(signingResult);  
        }
    }
}