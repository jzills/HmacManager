
# Example

Register [HmacManager](../../../README.md) through the [AddHmacManager](/src/HmacManager/Mvc/Extensions/IServiceCollectionExtensions.cs) extension method.

    builder.Services
        .AddHmacManager(options =>
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

Add a HttpClient and call [AddHmacHttpMessageHandler](/src/HmacManager/Mvc/HmacDelegatingHandler.cs) to register a handler to automatically sign outgoing requests.

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