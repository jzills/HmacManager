namespace HmacManager.Components;

public interface IHmacManager
{
    Task<HmacResult> VerifyAsync(HttpRequestMessage request);
    Task<HmacResult> SignAsync(
        HttpRequestMessage request, 
        MessageContent[]? messageContentHeaders = null
    );
}