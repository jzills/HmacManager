using HmacManagement.Components;

namespace Client;

public interface IMyClient
{
    Task SendAsync(
        string relativeUrl, 
        object? body = null
    );
}

public class MyClient : IMyClient
{
    private readonly HttpClient _client;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MyClient(HttpClient client, IHttpContextAccessor httpContextAccessor)
    {
        _client = client;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task SendAsync(
        string relativeUrl, 
        object? body = null
    )
    {
        var response = await _client.GetAsync(relativeUrl);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var debug = responseContent;
    }
}

public class MyClientHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MyClientHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken
    )
    {
        var hmacManager = _httpContextAccessor.HttpContext.RequestServices
            .GetRequiredService<IHmacManager>();

        var signingResult = await hmacManager.SignAsync(request);

        return await base.SendAsync(request, cancellationToken);
    }
}