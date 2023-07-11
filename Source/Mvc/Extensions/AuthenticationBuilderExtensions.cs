using Microsoft.AspNetCore.Authentication;
using HmacManager.Mvc;

namespace HmacManager.Mvc.Extensions;

public static class AuthenticationBuilderExtensions
{
    // public static AuthenticationBuilder AddHmac(
    //     this AuthenticationBuilder builder,
    //     Action<HmacAuthenticationOptions> configureOptions
    // ) => 
    //     builder.AddScheme<HmacAuthenticationOptions, HmacAuthenticationHandler>(
    //         HmacAuthenticationDefaults.Scheme,
    //         HmacAuthenticationDefaults.Scheme,
    //         configureOptions
    //     );
}