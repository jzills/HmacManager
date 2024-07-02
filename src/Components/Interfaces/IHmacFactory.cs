using HmacManager.Headers;

namespace HmacManager.Components;

public interface IHmacFactory
{
    Task<Hmac?> CreateAsync(HttpRequestMessage request, HeaderScheme? headerScheme = null);
    Task<Hmac?> CreateAsync(HttpRequestMessage request, HmacPartial? hmac);
}