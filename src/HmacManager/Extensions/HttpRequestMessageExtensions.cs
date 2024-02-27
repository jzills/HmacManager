namespace HmacManager.Extensions;

internal static class HttpRequestMessageExtensions
{
    public static bool TryGetContent(this HttpRequestMessage request, out HttpContent content)
    {
        if (request.HasContent())
        {
            content = request.Content!;
            return true;
        }
        else
        {
            content = default!;
            return false;
        }
    }

    public static bool HasContent(this HttpRequestMessage request) =>
        request.Content is not null &&
        request.Content.Headers.ContentLength > 0;
}