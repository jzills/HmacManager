using HmacManager.Components;
using HmacManager.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Unit.Tests.Common;

public static class HttpRequestExtensions
{
    public static void ConfigureFor(this HttpRequest request, Uri uri, HttpMethod method)
    {
        request.Host = new HostString(uri.Authority);
        request.PathBase = new PathString(uri.AbsolutePath);
        request.IsHttps = true;
        request.Method = method.ToString();
    }

    public static void AddHmacHeaders(this HttpRequest request, HmacResult? signingResult)
    {
        ArgumentNullException.ThrowIfNull(signingResult, nameof(signingResult));

        var headers = request.Headers;
        var hmac = signingResult.Hmac;
        if (hmac is not null)
        {
            headers.Append(
                HmacAuthenticationDefaults.Headers.DateRequested, 
                hmac.DateRequested.ToUnixTimeMilliseconds().ToString()
            );

            headers.Append(
                HmacAuthenticationDefaults.Headers.Nonce, 
                hmac.Nonce.ToString()
            );

            headers.Append(
                HmacAuthenticationDefaults.Headers.Policy, 
                signingResult?.Policy
            );

            headers.Append(
                HmacAuthenticationDefaults.Headers.Scheme, 
                signingResult?.HeaderScheme
            );

            if (hmac.HeaderValues.Any())
            {
                foreach (var headerValue in hmac.HeaderValues)
                {
                    headers.Append(headerValue.Name, headerValue.Value);
                }
            }

            var authorizationHeaderValue = $"{HmacAuthenticationDefaults.AuthenticationScheme} {hmac.Signature}";
            headers.Authorization = new StringValues(authorizationHeaderValue);
        }
    }
}