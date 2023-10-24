namespace HmacManagement.Extensions;

public static class DateTimeOffsetExtensions
{
    public static bool HasValidRequestedOn(
        this DateTimeOffset requestedOn, 
        TimeSpan maxAge
    ) => DateTimeOffset.UtcNow.Subtract(requestedOn) < maxAge;
}