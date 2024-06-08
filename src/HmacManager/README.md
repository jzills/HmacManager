
# HmacManager

[![NuGet Version](https://img.shields.io/nuget/v/HmacManager.svg)](https://www.nuget.org/packages/HmacManager/) [![NuGet Downloads](https://img.shields.io/nuget/dt/HmacManager.svg)](https://www.nuget.org/packages/HmacManager/) [![.NET](https://github.com/jzills/HmacManager/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jzills/HmacManager/actions/workflows/dotnet.yml)

- [Quickstart](#quickstart)
    * [Register without built-in authentication flow](#register-without-built-in-authentication-flow)
    * [Register with built-in authentication flow](#register-with-built-in-authentication-flow)
    * [Register HttpClient with HmacHttpMessageHandler](#register-httpclient-with-hmachttpmessagehandler)
- [In-Depth Tutorial](#in-depth-tutorial)
    * [The HmacManager Object](#the-hmacmanager-object)
    * [The HmacManagerFactory Object](#the-hmacmanagerfactory-object)
    * [The HmacPolicyCollection Object](#the-hmacpolicycollection-object)

# Quickstart

A short and sweet overview of how to register `HmacManager` to help you get up and running. There are two methods of dependency injection registration. You should choose the one appropriate for your situation and how much flexibility you might require.

- [Register without built-in authentication flow](#register-without-built-in-authentication-flow)
    - Only registers the `HmacManager` object through an `IHmacManagerFactory` service where you will be required to handle signatures and verification manually.
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

Access configured policies from `IHmacManagerFactory`

    IHmacManager hmacManager = hmacManagerFactory.Create("SomePolicy")

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

## Register HttpClient with HmacHttpMessageHandler

The `AddHmacHttpMessageHandler` extension method registers an instance of `HmacDelegatingHandler` to the specified `HttpClient` with the configured policy and the optional scheme. This handler will automatically sign outgoing requests for that client. If the request cannot be signed, then an `HmacSigningException` exception is thrown.

    builder.Services
        .AddHttpClient("Hmac", client => ...)
        .AddHmacHttpMessageHandler("MyPolicy", "MyScheme");

- If a scheme is specified, then all headers in that scheme must be added to the request prior to calling `Send` or `SendAsync` on the `HttpClient`. The corresponding header values will be part of the signing content used to create the hmac.

# In-Depth Tutorial

This is where you can find a comprehensive guide on all of the functionality available to your disposal.

## The HmacManager Object

Coming Soon

## The HmacManagerFactory Object

Coming Soon

## The HmacPolicyCollection Object

Coming Soon
