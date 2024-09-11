using System.Text;

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

    internal static bool TryReadAndResetPosition(this MemoryStream stream, out string content)
    {
        try
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            content = reader.ReadToEndAsync().Result;
            stream.Position = 0;
            return true;
        }
        catch (Exception)
        {
            content = default;
            return false;
        }
    }
}