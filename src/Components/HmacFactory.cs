using HmacManager.Headers;

namespace HmacManager.Components;

/// <summary>
/// A class responsible for creating hmacs.
/// </summary> 
public class HmacFactory : IHmacFactory
{
    /// <summary>
    /// An implementation of <see cref="IHmacSignatureProvider"/>. 
    /// </summary>
    protected readonly IHmacSignatureProvider Provider;

    /// <summary>
    /// Creates an instance of <see cref="HmacFactory"/>.
    /// </summary>
    /// <param name="provider">The provider to compute hmac signatures.</param>
    public HmacFactory(IHmacSignatureProvider provider) => Provider = provider;

    /// <summary>
    /// Creates an hmac from a http request, optionally based on a specified header scheme.
    /// </summary>
    /// <param name="request">A http request message.</param>
    /// <param name="headerScheme">An optional header scheme.</param>
    /// <returns></returns>
    public Task<Hmac?> CreateAsync(HttpRequestMessage request, HeaderScheme? headerScheme = null)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        return CreateInstance(new HmacBuilder(request, headerScheme));
    }

    /// <summary>
    /// Creates an hmac from a http request and partial hmac.
    /// </summary>
    /// <param name="request">A http request message.</param>
    /// <param name="hmac">A partial hmac.</param>
    /// <returns></returns>
    public Task<Hmac?> CreateAsync(HttpRequestMessage request, HmacPartial? hmac)
    {
        ArgumentNullException.ThrowIfNull(hmac, nameof(hmac));
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        return CreateInstance(new HmacBuilder(request, hmac));
    }

    private Task<Hmac?> CreateInstance(HmacBuilder builder) => 
        builder.WithProvider(Provider).BuildAsync();
}