
# HmacManager


### Register without built-in authentication flow

Use the IServiceCollection extension method AddHmacManager to add IHmacManagerFactory to the dependency injection container. 

    builder.Services
        .AddHmacManager(options =>
        {
            options.AddPolicy("SomePolicy", policy =>
            {
                policy.UsePublicKey(...);
                policy.UsePrivateKey(...);
                policy.UseInMemoryCache(...);
            });
        });

Access configured policies from IHmacManagerFactory

    IHmacManager hmacManager = hmacManagerFactory.Create("SomePolicy")

A policy can be extended with schemes. These schemes represent the required headers that must be present in the request. These become a part of the content hash.

    builder.Services
        .AddHmacManager(options =>
        {
            options.AddPolicy("SomePolicy", policy =>
            {
                policy.UsePublicKey(...);
                policy.UsePrivateKey(...);
                policy.UseInMemoryCache(...);
                policy.AddScheme("SomeScheme", scheme =>
                {
                    scheme.AddHeader("X-UserId");
                    scheme.AddHeader("X-Email");
                });
            });
        });

### Register with built-in authentication flow

The AddHmacManager extension method can be bypassed in favor of the IAuthenticationBuilder extension method AddHmac. 

    builder.Services
        .AddAuthentication()
        .AddHmac(options => options.AddPolicy("SomePolicy", policy => ...));

- The HmacAuthenticationHandler is handles parsing incoming requests and authenticating the correct scheme.
    - By default, if there is a policy that matches the one defined in the request headers that can be successfully verified, then the handler returns a success. If more granular authentication is required, such as protected specific routes with different policies, then use the HmacAuthorize attribute to enable stricter authentication requirements.
- Any scheme headers are mapped to their specified claim types. If no claim type is specified, the name of the header is used.
