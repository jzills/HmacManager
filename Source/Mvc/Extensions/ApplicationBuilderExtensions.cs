using Microsoft.AspNetCore.Authentication;
using Souce.Mvc;

namespace Source.Mvc.Extensions;

public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddHMAC(
        this AuthenticationBuilder builder,
        Action<HMACAuthenticationOptions> configureOptions
    ) => 
        builder.AddScheme<HMACAuthenticationOptions, HMACAuthenticationHandler>(
            HMACAuthenticationDefaults.Scheme,
            HMACAuthenticationDefaults.Scheme,
            configureOptions
        );
}