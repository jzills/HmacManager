using System.Text.Json;
using HmacManager.Headers;

namespace HmacManager.Extensions;

internal static class Utf8JsonReaderExtensions
{
    public static bool TryGetString(this ref Utf8JsonReader reader, out string value)
    {
        value = reader.GetString() ?? string.Empty;
        return !string.IsNullOrWhiteSpace(value);
    }

    public static bool TryGetHeaders(this ref Utf8JsonReader reader, out List<Header> values)
    {
        values = new List<Header>();

        if (reader.TokenType is JsonTokenType.StartArray)
        {
            while (reader.Read())
            {
                if (TryGetHeader(ref reader, out var value))
                {
                    values.Add(value);
                }

                if (reader.TokenType is JsonTokenType.EndArray)
                {
                    reader.Read();
                    break;
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool TryGetHeader(this ref Utf8JsonReader reader, out Header value)
    {
        string? name = null;
        string? claimType = null;

        if (reader.TokenType is JsonTokenType.StartObject)
        {
            while (reader.Read())
            {
                if (reader.TokenType is JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    if (propertyName == nameof(Header.Name))
                    {
                        name = reader.GetString() ?? string.Empty;
                    }
                    else if (propertyName == nameof(Header.ClaimType))
                    {
                        claimType = reader.GetString() ?? string.Empty;
                    }
                }
                else if (reader.TokenType is not JsonTokenType.EndObject)
                {
                    break;
                }
            }

            value = new Header(name, claimType);
            return true;
        }
        else
        {
            value = new Header(name, claimType);
            return false;
        }
    }

}