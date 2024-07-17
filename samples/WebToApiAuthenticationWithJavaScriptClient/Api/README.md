
# Example

Register [HmacManager](../../../README.md) through the [AddHmac](/src/Mvc/Extensions/AuthenticationBuilderExtensions.cs) extension method.

    builder.Services
        .AddMemoryCache()
        .AddAuthentication()
        .AddHmac(options =>
        {
            options.AddPolicy("MyPolicy", policy =>
            {
                policy.UsePublicKey(...);
                policy.UsePrivateKey(...);
                policy.UseMemoryCache(TimeSpan.FromSeconds(30));
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