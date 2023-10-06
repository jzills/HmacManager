using HmacManagement.Remodel;

namespace HmacManagement.Components;

public interface IHmacManager
{
    Task<HmacResult> SignAsync(HttpRequestMessage request, HeaderScheme scheme);
    Task<HmacResult> VerifyAsync(HttpRequestMessage request, HeaderScheme scheme);
}