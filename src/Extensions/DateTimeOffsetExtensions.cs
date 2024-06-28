namespace HmacManager.Extensions;

internal static class DateTimeOffsetExtensions
{
    public static bool HasValidDateRequested(
        this DateTimeOffset dateRequested, 
        int maxAgeInSeconds
    ) => DateTimeOffset.UtcNow.Subtract(dateRequested) < TimeSpan.FromSeconds(maxAgeInSeconds);
}