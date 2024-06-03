
# WebToApiAuthenticationWithJsonConfiguration

This demo is an example of using [HmacManager](../../README.md) for authentication between two .NET applications. One, the [Web](Web) is signing requests to interact with protected [Api](Api) endpoints.

## Api

See this [example](../WebToApiAuthentication/Api/README.md). Additionally, when calling `AddAuthorization`, the `AuthorizationPolicyBuilder` extension method `RequireHmacAuthentication` can be used to register policies and schemes:

    builder.Services.AddAuthorization(options => 
    {
        // This is one way to register
        // options.AddPolicy("Require_Hmac_PolicyScheme_2", policy => 
        //     policy.AddRequirements(new HmacAuthenticateAttribute 
        //     { 
        //         Policy = "HmacPolicy_2", 
        //         Scheme = "HmacScheme_2"
        //     }));

        // This is another
        // options.AddPolicy("Require_Hmac_PolicyScheme_2", policy =>
        // {
        //     policy.RequireHmacPolicy(`"HmacPolicy_2");
        //     policy.RequireHmacScheme("HmacScheme_2");
        // });

        // This is the preferred approach due to it's simplicity
        options.AddPolicy("Require_Hmac_PolicyScheme_2", policy =>
        {
            policy.RequireHmacAuthentication("HmacPolicy_2", "HmacScheme_2");
        });
    });

## Web

See this [example](../WebToApiAuthentication/Web/README.md).