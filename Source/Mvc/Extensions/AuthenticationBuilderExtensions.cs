using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using HmacManagement.Caching;
using HmacManagement.Components;
using HmacManagement.Policies;

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

        // var options = new HmacAuthenticationOptions();
        // configureOptions.Invoke(options);
        // var policies = options.GetPolicies();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<INonceCacheCollection, NonceCacheCollection>();
        builder.Services.AddScoped<IHmacPolicyCollection, HmacPolicyCollection>();
        builder.Services.AddScoped<IHmacManagerFactory, HmacManagerFactory>();
        
        return builder;
    }
}