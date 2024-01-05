using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using HmacManager.Mvc.Extensions.Internal;

namespace HmacManager.Mvc.Extensions;

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

        builder.Services.AddHmacManager(options.GetOptions());

        return builder;
    }
}