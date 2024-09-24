namespace HmacManager.Extensions;

internal static class HttpRequestMessageExtensions
{
    internal static bool TryCopyAndAssignContent(this HttpRequestMessage request, out MemoryStream contentStream)
    {
        if (request.HasContent())
        {
            contentStream = new MemoryStream();
            request.Content.CopyToAsync(contentStream).Wait();

            if (contentStream.Length == 0)
            {
                contentStream = default;
                return false;
            }

            contentStream.Position = 0;

            var rewindableContent = new StreamContent(contentStream);
            foreach (var header in request.Content.Headers)
            {
                rewindableContent.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            request.Content = rewindableContent;
            return true;
        }
        else
        {
            contentStream = default;
            return false;
        }
    }

    internal static bool TryGetContent(this HttpRequestMessage request, out HttpContent content)
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
    // => request.Content is not null;
    // TODO: Review this as using extensions like PostAsJsonAsync does not 
    // set the ContentLength until later in the pipeline.
    //(request.Content.Headers.ContentLength > 0 || (request.Headers.TransferEncodingChunked ?? false));
}