namespace Source.Components;

public interface IHMACManager
{
    Task<HMACResult> VerifyAsync(HttpRequestMessage request);
    Task<HMACResult> SignAsync(
        HttpRequestMessage request, 
        MessageContent[]? messageContentHeaders = null
    );
}