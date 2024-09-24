namespace HmacManager.Extensions;

internal static class HttpRequestMessageExtensions
{
    internal static bool HasContent(this HttpRequestMessage request)
    {
        if (request.Content is not null)
        {
            if (request.Content.Headers.Contains("Content-Length"))
            {
                return request.Content.Headers.ContentLength > 0;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }
}