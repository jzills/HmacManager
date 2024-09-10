using Microsoft.AspNetCore.Http;

namespace HmacManager.Features;

public class HttpRequestMessageFeature : IHttpRequestMessageFeature
{
    private readonly HttpContext _httpContext;
    private HttpRequestMessage _httpRequestMessage;

    public HttpRequestMessageFeature(HttpContext httpContext)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        _httpContext = httpContext;
    }

    public HttpRequestMessage HttpRequestMessage
    {
        get
        {
            if (_httpRequestMessage == null)
            {
                _httpRequestMessage = CreateHttpRequestMessage(_httpContext);
            }

            return _httpRequestMessage;
        }

        set => _httpRequestMessage = value;
    }

    private static HttpRequestMessage CreateHttpRequestMessage(HttpContext httpContext)
    {
        var httpRequest = httpContext.Request;
        var uriString =
            httpRequest.Scheme + "://" +
            httpRequest.Host +
            httpRequest.PathBase +
            httpRequest.Path +
            httpRequest.QueryString;

        var message = new HttpRequestMessage(new HttpMethod(httpRequest.Method), uriString);
        message.Options.TryAdd(nameof(HttpContext), httpContext);
        message.Content = new StreamContent(httpRequest.Body);

        foreach (var header in httpRequest.Headers)
        {
            if (!message.Headers.TryAddWithoutValidation(header.Key, (IEnumerable<string>)header.Value))
            {
                message.Content.Headers.TryAddWithoutValidation(header.Key, (IEnumerable<string>)header.Value);
            }
        }

        return message;
    }
}