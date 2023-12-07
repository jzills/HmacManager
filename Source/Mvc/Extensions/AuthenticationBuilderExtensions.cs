using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using HmacManagement.Mvc.Extensions.Internal;

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

        builder.Services.AddHmacManagement(options.GetOptions());

        return builder;
    }
}