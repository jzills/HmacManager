namespace HmacManager.Extensions;

internal static class HttpRequestMessageExtensions
{
    public static bool TryCopyAndAssignContent(this HttpRequestMessage request, out MemoryStream contentStream)
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

    public static bool HasContent(this HttpRequestMessage request) => request.Content is not null;
        // TODO: Review this as using extensions like PostAsJsonAsync does not 
        // set the ContentLength until later in the pipeline.
        //(request.Content.Headers.ContentLength > 0 || (request.Headers.TransferEncodingChunked ?? false));
}