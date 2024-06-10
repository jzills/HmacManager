
# Example

See this [example](../../WebToApiAuthentication/Api/README.md). Additionally, when calling `AddAuthorization`, the `AuthorizationPolicyBuilder` extension method `RequireHmacAuthentication` can be used to register policies and schemes:

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