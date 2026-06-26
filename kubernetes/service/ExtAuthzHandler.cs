using HmacManager.Exceptions;
using HmacManager.Mvc;

namespace HmacManager.Kubernetes;

internal class ExtAuthzHandler(IHmacAuthenticationContextProvider contextProvider)
{
    public async Task<IResult> CheckAsync(HttpContext ctx)
    {
        try
        {
            if (!contextProvider.TryGetAuthenticationContext(ctx.Request.Headers, out var authContext) ||
                authContext.HmacManager is null)
            {
                return Results.StatusCode(StatusCodes.Status403Forbidden);
            }

            // Envoy preserves the original Host header in the check request.
            // x-forwarded-proto carries the original scheme (the connection to the
            // ext-authz service is plain HTTP regardless of the upstream scheme).
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
        catch (HmacPolicyNotFoundException)
        {
            return Results.StatusCode(StatusCodes.Status403Forbidden);
        }
    }
}
