
# WebToApiAuthentication

This demo is an example of using [HmacManager](../../README.md) for authentication between two .NET applications. One, the [Web](Web) is signing requests to interact with protected [Api](Api) endpoints.

## Api

Register [HmacManager](../../README.md) through the [AddHmac](/src/Mvc/Extensions/AuthenticationBuilderExtensions.cs) extension method.

    builder.Services
        .AddMemoryCache()
        .AddAuthentication()
        .AddHmac(options =>
        {
            options.AddPolicy("MyPolicy", policy =>
            {
                policy.UsePublicKey(...);
                policy.UsePrivateKey(...);
                policy.UseMemoryCache(30);
                policy.AddScheme("RequireAccountAndEmail", scheme =>
                {
                    scheme.AddHeader("X-Account");
                    scheme.AddHeader("X-Email");
                });
            });
        });

Decorate controller or action with [HmacAuthenticateAttribute](/src/Mvc/HmacAuthenticateAttribute.cs).

    [ApiController]
    [Route("api/[controller]")]
    [HmacAuthenticate(Policy = "MyPolicy", Scheme = "RequireAccountAndEmail")]
    public class WeatherForecastController : ControllerBase

## Web

Register [HmacManager](../../README.md) through the [AddHmacManager](/src/Mvc/Extensions/IServiceCollectionExtensions.cs) extension method.

    builder.Services
        .AddHmacManager(options =>
        {
            options.AddPolicy("MyPolicy", policy =>
            {
                policy.UsePublicKey(...);
                policy.UsePrivateKey(...);
                policy.UseMemoryCache(30);
                policy.AddScheme("RequireAccountAndEmail", scheme =>
                {
                    scheme.AddHeader("X-Account");
                    scheme.AddHeader("X-Email");
                });
            });
        });

Add a HttpClient and call [AddHmacHttpMessageHandler](/src/Mvc/HmacDelegatingHandler.cs) to register a handler to automatically sign outgoing requests.

    builder.Services
        .AddHttpClient("Hmac_MyPolicy_RequireAccountAndEmail", client =>
        {
            client.BaseAddress = new Uri(...);
        }).AddHmacHttpMessageHandler("MyPolicy", "RequireAccountAndEmail");

Create a request, add any headers required by the scheme and fire away.

    using var request = new HttpRequestMessage(HttpMethod.Get, "api/weatherforecast");

    request.Headers.Add("X-Account", ...);
    request.Headers.Add("X-Email", ...);

    var response = await _client.SendAsync(request);