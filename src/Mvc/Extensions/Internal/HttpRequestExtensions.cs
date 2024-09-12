using Microsoft.AspNetCore.Http;

namespace HmacManager.Mvc.Extensions.Internal;

internal static class HttpRequestExtensions
{
    internal static bool HasContent(this HttpRequest request)
    {
        if (request.Body is not null)
        {
            if (request.ContentLength.HasValue && request.ContentLength > 0)
            {
                return true;
            }
            
            if (request.Body.CanSeek)
            {
                request.Body.Seek(0, SeekOrigin.Begin);
                return request.Body.Length > 0;
            }
            else
            {
                using var stream = new MemoryStream();
                request.Body.CopyToAsync(stream).Wait();
                return stream.Length > 0;
            }
        }
        else
        {
            return false;
        }
    }
}