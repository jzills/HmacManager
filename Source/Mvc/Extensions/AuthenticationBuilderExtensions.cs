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

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IComponentCollection<INonceCache>, NonceCacheCollection>();
        builder.Services.AddScoped<IComponentCollection<HmacPolicy>, HmacPolicyCollection>();
        builder.Services.AddScoped<IHmacManagerFactory, HmacManagerFactory>();
        
        return builder;
    }
}