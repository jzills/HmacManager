namespace HmacManager.Mvc.Extensions.Internal;

internal static class StreamExtensions
{
    internal static void Rewind(this Stream stream)
    {
        if (stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }
    }
}