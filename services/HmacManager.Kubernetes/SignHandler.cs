using System.Text;
using HmacManager.Components;

namespace HmacManager.Kubernetes;

internal record SignRequest(
    string Policy,
    string Method,
    string Uri,
    string? Scheme = null,
    string? Body = null
);

internal class SignHandler(IHmacManagerFactory factory)
{
    public async Task<IResult> SignAsync(SignRequest signRequest)
    {
        var manager = factory.Create(signRequest.Policy, signRequest.Scheme);
        if (manager is null)
            return Results.NotFound($"Policy '{signRequest.Policy}' not found.");

        var httpRequest = new HttpRequestMessage(new HttpMethod(signRequest.Method), signRequest.Uri);

        if (signRequest.Body is not null)
            httpRequest.Content = new StringContent(signRequest.Body, Encoding.UTF8, "application/json");

        var result = await manager.SignAsync(httpRequest);
        if (!result.IsSuccess)
            return Results.Problem("Signing failed.");

        var headers = httpRequest.Headers
            .Concat(httpRequest.Content?.Headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>())
            .Where(h => h.Key.StartsWith("Hmac", StringComparison.OrdinalIgnoreCase) || h.Key == "Authorization")
            .ToDictionary(h => h.Key, h => h.Value.FirstOrDefault() ?? string.Empty);

        return Results.Ok(headers);
    }
}
