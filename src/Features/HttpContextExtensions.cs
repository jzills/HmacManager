using Microsoft.AspNetCore.Http;

namespace HmacManager.Features;

public static class HttpRequestMessageHttpContextExtensions
{
    public static HttpRequestMessage GetHttpRequestMessage(this HttpContext httpContext)
    {
        var feature = httpContext.Features.Get<IHttpRequestMessageFeature>();
        if (feature == null)
        {
            feature = new HttpRequestMessageFeature(httpContext);
            httpContext.Features.Set(feature);
        }

        return feature.HttpRequestMessage;
    }
}