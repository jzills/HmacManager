namespace HmacManagement.Components;

public interface IHmacManager
{
    Task<HmacResult> VerifyAsync(HttpRequestMessage request);
    Task<HmacResult> SignAsync(
        HttpRequestMessage request, 
        Header[]? headersToSign = null
    );
}
