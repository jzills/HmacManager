using System.Text;
using HmacManager.Exceptions;
using HmacManager.Mvc;

namespace HmacManager.Components;

/// <inheritdoc/>
public class HmacOptionsHeaderParser : HmacHeaderParser
{
    /// <inheritdoc/>
    public HmacOptionsHeaderParser(IDictionary<string, string> headers) : base(Process(headers))
    {
    }

    private static IDictionary<string, string> Process(IDictionary<string, string> headers)
    {
        if (headers.TryGetValue(HmacAuthenticationDefaults.Headers.Options, out var options))
        {
            if (!string.IsNullOrWhiteSpace(options))
            {
                var headerOptionsValueFormatted = DecodeBase64(options);
                var headerOptionsValues = ToDictionary(headerOptionsValueFormatted);
                return headerOptionsValues;
            }
            else
            {
                throw new BadHeaderFormatException();
            }
        }
        else
        {
            throw new MissingHeaderException();
        }
    }

    private static string DecodeBase64(string value) => 
        Encoding.UTF8.GetString(Convert.FromBase64String(value));

    private static IDictionary<string, string> ToDictionary(string value)
    {
        var splitValues = value.Split("&");
        if (splitValues.Length > 0)
        {
            // TODO: This fails because signature can end in equals resulting in 3 entries
            // Need a better way, like including the HmacAuthenticationDefaults.Headers
            // for each split, i.e. "Authorization=", "Hmac-Policy=", etc..
            var splitKeyValues = splitValues.Select(splitValue => splitValue.Split("="));
            return splitKeyValues.ToDictionary(
                chunk => chunk.First(), 
                chunk => chunk.Last()
            );
        }
        else
        {
            throw new BadHeaderFormatException();
        }
    }
}