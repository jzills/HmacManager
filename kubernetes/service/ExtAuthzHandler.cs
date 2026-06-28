using HmacManager.Exceptions;
using HmacManager.Mvc;

namespace HmacManager.Kubernetes;

internal class ExtAuthzHandler(IHmacAuthenticationContextProvider contextProvider)
{
    public async Task<IResult> CheckAsync(HttpContext ctx)
    {
        // Envoy lowercases all custom headers. Normalize to case-insensitive before
        // passing to the context provider whose internal dictionary is case-sensitive.
        var headers = ctx.Request.Headers.ToDictionary(
            h => h.Key,
            h => h.Value.FirstOrDefault() ?? string.Empty,
            StringComparer.OrdinalIgnoreCase);

        try
        {
            if (!contextProvider.TryGetAuthenticationContext(headers, out var authContext) ||
                authContext.HmacManager is null)
            {
                return Results.StatusCode(StatusCodes.Status403Forbidden);
            }

            var scheme = ctx.Request.Headers["x-forwarded-proto"].FirstOrDefault() ?? ctx.Request.Scheme;
            var host   = ctx.Request.Host.Value;
            var uri    = new Uri($"{scheme}://{host}{ctx.Request.PathBase}{ctx.Request.Path}{ctx.Request.QueryString}");

            var httpRequest = new HttpRequestMessage(new HttpMethod(ctx.Request.Method), uri);

            if (ctx.Request.ContentLength > 0 || ctx.Request.Headers.ContainsKey("Transfer-Encoding"))
            {
                ctx.Request.EnableBuffering();
                httpRequest.Content = new StreamContent(ctx.Request.Body);
            }

            foreach (var (key, values) in ctx.Request.Headers)
            {
                if (!httpRequest.Headers.TryAddWithoutValidation(key, (IEnumerable<string>)values))
                    httpRequest.Content?.Headers.TryAddWithoutValidation(key, (IEnumerable<string>)values);
            }

            var result = await authContext.HmacManager.VerifyAsync(httpRequest);

            return result.IsSuccess
                ? Results.Ok()
                : Results.StatusCode(StatusCodes.Status403Forbidden);
        }
        catch (Exception e) when (e is HmacPolicyNotFoundException or MissingHeaderException or BadHeaderFormatException)
        {
            return Results.StatusCode(StatusCodes.Status403Forbidden);
        }
    }
}
