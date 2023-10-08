using HmacManagement.Remodel;

namespace HmacManagement.Components;

public interface IHmacManager
{
    Task<HmacResult> SignAsync(HttpRequestMessage request);
    Task<HmacResult> VerifyAsync(HttpRequestMessage request);
}