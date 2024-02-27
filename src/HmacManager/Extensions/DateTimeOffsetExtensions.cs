namespace HmacManager.Extensions;

internal static class DateTimeOffsetExtensions
{
    public static bool HasValidDateRequested(
        this DateTimeOffset dateRequested, 
        TimeSpan maxAge
    ) => DateTimeOffset.UtcNow.Subtract(dateRequested) < maxAge;
}