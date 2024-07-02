namespace HmacManager.Components.Extensions;

internal static class HmacExtensions
{
    internal static bool IsExactMatch(this Hmac? source, Hmac? other)
    {
        if (source is null || other is null)
        {
            return false;
        }
        else
        {
            return source.IsVerified(other);
        }
    }
}