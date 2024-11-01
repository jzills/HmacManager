using System.Text;
using HmacManager.Exceptions;
using HmacManager.Extensions;
using HmacManager.Headers;
using HmacManager.Mvc;

namespace HmacManager.Components;

/// <summary>
/// A class representing a builder to create required hmac headers as a single options header to be added to a request.
/// </summary> 
public class HmacOptionsHeaderBuilder : HmacHeaderBuilder
{
    /// <inheritdoc/>
    public HmacOptionsHeaderBuilder(HmacManagerOptions options, Hmac hmac) 
        : base(options, hmac)
    {
    }

    /// <inheritdoc/>
    public override IReadOnlyCollection<HeaderValue> Build()
    {
        if (NonEmptyHeaderValues.Any())
        {
            var headerOptionsValueFormatted = string.Join("&", NonEmptyHeaderValues.Select(element => element.FormatAsEquality()));
            var headerOptionsValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(headerOptionsValueFormatted));
            return [new HeaderValue(HmacAuthenticationDefaults.Headers.HmacOptions, headerOptionsValue)];
        }
        else
        {
            throw new MissingHeaderException();
        }
    }
}