using HmacManagement.Caching.Memory;
using HmacManagement.Components;
using HmacManagement.Remodel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HmacManagement.Mvc.Extensions;

public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddHmac(
        this AuthenticationBuilder builder,
        Action<HmacAuthenticationOptions> configureOptions
    )
    {
        builder.AddScheme<HmacAuthenticationOptions, HmacAuthenticationHandler>(
            HmacAuthenticationDefaults.AuthenticationScheme,
            HmacAuthenticationDefaults.AuthenticationScheme,
            configureOptions
        );

        var options = new HmacAuthenticationOptions();
        configureOptions.Invoke(options);
        var policies = options.GetPolicies();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IHmacManager, HmacManager>(provider => 
        {
            var contextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            var request = contextAccessor.HttpContext.GetHttpRequestMessage();
            if (request.Headers.TryGetValues("X-Hmac-Policy", out var values))
            {
                var policyValue = values.First();
                var optionsSnapshot = provider.GetRequiredService<IOptionsSnapshot<HmacOptions>>();
                var policyOptions = optionsSnapshot.Get(policyValue);
                
                // Get HeaderScheme in AuthenticationHandler and
                // try to validate there
                // if (request.Headers.TryGetValues("X-Hmac-Scheme", out values))
                // {
                //     var schemeValue = values.First();
                //     var scheme = policyOptions.GetHeaderScheme(schemeValue);
                // }

                var hmacProvider = new HmacProvider(new HmacProviderOptions
                {
                    Keys = policyOptions.Keys,
                    Algorithms = policyOptions.Algorithms
                });

                var hmacManager = new HmacManager(
                    new HmacManagerOptions(),
                    new NonceMemoryCache(
                        provider.GetRequiredService<IMemoryCache>(),
                        new Caching.NonceCacheOptions
                        {
                            MaxAge = policyOptions.Nonce.MaxAge,
                            Type = Caching.NonceCacheType.Memory
                        }
                    ),
                    hmacProvider
                );

                return hmacManager;
            }
            else
            {
                throw new Exception("The incoming request does not specify a required \"X-Hmac-Policy\" header.");
            }
        });
        
        // builder.Services.AddHmacManagement(options =>
        // {
        //     options.ClientId = authenticationOptions.
        // });

        return builder;
    }
}