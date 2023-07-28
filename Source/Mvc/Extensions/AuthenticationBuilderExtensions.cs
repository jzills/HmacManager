using Microsoft.AspNetCore.Authentication;
using HmacManagement.Mvc;

namespace HmacManagement.Mvc.Extensions;

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