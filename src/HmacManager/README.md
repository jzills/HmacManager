
# HmacManager

[![NuGet Version](https://img.shields.io/nuget/v/HmacManager.svg)](https://www.nuget.org/packages/HmacManager/) [![NuGet Downloads](https://img.shields.io/nuget/dt/HmacManager.svg)](https://www.nuget.org/packages/HmacManager/) [![.NET](https://github.com/jzills/HmacManager/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jzills/HmacManager/actions/workflows/dotnet.yml)

- [Quickstart](#quickstart)
    * [Register without built-in authentication flow](#register-without-built-in-authentication-flow)
    * [Register with built-in authentication flow](#register-with-built-in-authentication-flow)
    * [Register with an `IConfigurationSection`](#register-with-an-iconfigurationsection)
    * [Register HttpClient with `HmacHttpMessageHandler`](#register-httpclient-with-hmachttpmessagehandler)
- [In-Depth Tutorial](#in-depth-tutorial)
    * The `HmacManager` Object
    * The `HmacManagerFactory` Object
    * [Dynamic Policies with `IHmacPolicyCollection`](#dynamic-policies-with-ihmacpolicycollection)

# Quickstart

A short and sweet overview of how to register `HmacManager` to help you get up and running. There are two methods of dependency injection registration. You should choose the one appropriate for your situation and how much flexibility you might require.

- [Register without built-in authentication flow](#register-without-built-in-authentication-flow)
    - Only registers the `HmacManager` object through an `IHmacManagerFactory` service where you will be required to handle signatures and verification manually. An implementation of `IHmacManagerFactory` is registered with the DI container automatically. This is how you will instantiate `HmacManager` objects.
- [Register with built-in authentication flow](#register-with-built-in-authentication-flow)
    - Automatically registers an authentication handler and maps any authenticated request headers defined by a scheme to claims. This method handles verifying incoming requests without any additional setup. 

## Register without built-in authentication flow

Use the `IServiceCollection` extension method `AddHmacManager` to add `IHmacManagerFactory` to the dependency injection container. 

    builder.Services
        .AddHmacManager(options =>
        {
            options.AddPolicy("SomePolicy", policy =>
            {
                policy.UsePublicKey(...);
                policy.UsePrivateKey(...);
                policy.UseMemoryCache(...);
            });
        });

Access configured policies from an `IHmacManagerFactory`.

    IHmacManager hmacManager = hmacManagerFactory.Create("SomePolicy")

The `IHmacManagerFactory` is automatically registered with the DI container so it can be accessed anywhere services are injected.

    private readonly IHmacManager _hmacManager;

    public SomeContructor(IHmacManagerFactory hmacManagerFactory)
    {
        _hmacManager = hmacManagerFactory.Create("Some Policy");
    }

A policy can be extended with schemes. These schemes represent the required headers that must be present in the request. These become a part of the content hash.

    builder.Services
        .AddHmacManager(options =>
        {
            options.AddPolicy("SomePolicy", policy =>
            {
                policy.UsePublicKey(...);
                policy.UsePrivateKey(...);
                policy.UseMemoryCache(...);
                policy.AddScheme("SomeScheme", scheme =>
                {
                    scheme.AddHeader("X-UserId");
                    scheme.AddHeader("X-Email");
                });
            });
        });

## Register with built-in authentication flow

The `AddHmacManager` extension method can be bypassed in favor of the `IAuthenticationBuilder` extension method `AddHmac`. 

    builder.Services
        .AddAuthentication()
        .AddHmac(options => options.AddPolicy("SomePolicy", policy => ...));

- The `HmacAuthenticationHandler` handles parsing incoming requests and authenticating the correct scheme.
    - By default, if there is a policy that matches the one defined in the request headers that can be successfully verified, then the handler returns a success. If more granular authentication is required, such as protecting routes with specific policies or schemes, then there are a couple options available:
        - Use `HmacAuthenticateAttribute` to specify exact policies and schemes required to authenticate a given endpoint.

                [HmacAuthenticate(Policy = "HmacPolicy", Scheme = "HmacScheme")]
                public class HomeController : Controller

        - Use `HmacAuthenticateAttribute` as an `IAuthorizationRequirement` to an authorization policy and register the `HmacAuthorizationHandler` to handle the requirement automatically.

                builder.Services.AddAuthorization(options => 
                {
                    options.AddPolicy("RequireHmac", policy => 
                        policy.AddRequirements(new HmacAuthenticateAttribute 
                        { 
                            Policy = "HmacPolicy", 
                            Scheme = "HmacScheme"
                        }));
                });

        - Use the `AuthorizationPolicyBuilder` extensions `RequireHmacPolicy` and `RequireHmacScheme` to add hmac policy and scheme requirements to an authorization policy.

                builder.Services.AddAuthorization(options => 
                {
                    options.AddPolicy("RequireHmac", policy =>
                    {
                        policy.RequireHmacPolicy("HmacPolicy");
                        policy.RequireHmacScheme("HmacScheme");
                    });
                });

        - Use the `AuthorizationPolicyBuilder` extension `RequireHmacAuthentication` to add hmac policy and scheme requirements to an authorization policy.

                builder.Services.AddAuthorization(options => 
                {
                    options.AddPolicy("RequireHmac", policy =>
                    {
                        policy.RequireHmacAuthentication("HmacPolicy", "HmacScheme");
                    });
                });

- Any scheme headers are mapped to their specified claim types. If no claim type is specified, the name of the header is used.

## Register with an `IConfigurationSection`

- Both `AddHmacManager` and `AddHmac` have an overload which accepts an `IConfigurationSection` that corresponds to the json schema below. Additionally, an example project can be found [here](../../samples/WebToApiAuthenticationWithJsonConfiguration/README.md).

    ```
    [
        {
            "Name": "Some_Policy",
            "Keys": {
                "PublicKey": "37e3e675-370a-4ba9-af74-68f99b539f03",
                "PrivateKey": "zvg29s2cQ4idOqbUJWETOw=="
            },
            "Algorithms": {
                "ContentHashAlgorithm": "SHA256",
                "SigningHashAlgorithm": "HMACSHA256"
            },
            "Nonce": {
                "CacheType": "Memory",
                "MaxAgeInSeconds": 100
            },
            "HeaderSchemes": [
                {
                    "Name": "Some_Scheme",
                    "Headers": [
                        {
                            "Name": "Some_Header_1",
                            "ClaimType": "Header_1_ClaimType"
                        }
                    ]
                }
            ]
        }
    ]
    ```

## Register `HttpClient` with `HmacHttpMessageHandler`

The `AddHmacHttpMessageHandler` extension method registers an instance of `HmacDelegatingHandler` to the specified `HttpClient` with the configured policy and the optional scheme. This handler will automatically sign outgoing requests for that client. If the request cannot be signed, then an `HmacSigningException` exception is thrown.

    builder.Services
        .AddHttpClient("Hmac", client => ...)
        .AddHmacHttpMessageHandler("MyPolicy", "MyScheme");

- If a scheme is specified, then all headers in that scheme must be added to the request prior to calling `Send` or `SendAsync` on the `HttpClient`. The corresponding header values will be part of the signing content used to create the hmac.

# In-Depth Tutorial

This is where you can find a comprehensive guide on all of the functionality available to your disposal. This is currently a work in progress.

## Dynamic Policies with `IHmacPolicyCollection`

An implementation of `IHmacPolicyCollection` is registered as a singleton automatically when using `AddHmacManager` or `AddHmac` extension methods. This can be requested through the DI container and manipulated at runtime.

    private readonly IHmacPolicyCollection _policies;

    public SomeContructor(IHmacPolicyCollection policies)
    {
        _policies = policies;
    }    

Policies can be added by constructing a new `HmacPolicy`.

    _policies.Add(new HmacPolicy { ... });

Policies can be removed by specifying the name of the policy to remove.

    _policies.Remove("Some Policy");