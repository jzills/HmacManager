namespace HmacManager.Extensions;

/// <summary>
/// Provides extension methods for <see cref="HttpRequestMessage"/>.
/// </summary>
internal static class HttpRequestMessageExtensions
{
    /// <summary>
    /// Determines whether the <see cref="HttpRequestMessage"/> has content.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequestMessage"/> to check.</param>
    /// <returns><c>true</c> if the request has content; otherwise, <c>false</c>.</returns>
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