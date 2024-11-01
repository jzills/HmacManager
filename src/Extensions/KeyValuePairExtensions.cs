namespace HmacManager.Extensions;

internal static class KeyValuePairExtensions
{
    internal static string FormatAsEquality(this KeyValuePair<string, string?> source) => $"{source.Key}={source.Value}";

    internal static bool NonEmpty(this KeyValuePair<string, string?> source) =>
        !string.IsNullOrWhiteSpace(source.Key) &&
        !string.IsNullOrWhiteSpace(source.Value);
}