namespace HmacManagement.Components;

public interface IHmacManager
{
    Task<HmacResult> SignAsync(HttpRequestMessage request);
    Task<HmacResult> SignAsync(HttpRequestMessage request, string signingPolicy);
    Task<HmacResult> VerifyAsync(HttpRequestMessage request);
    Task<HmacResult> VerifyAsync(HttpRequestMessage request, string signingPolicy);
}
