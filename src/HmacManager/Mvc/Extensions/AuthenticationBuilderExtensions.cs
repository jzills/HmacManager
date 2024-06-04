using Microsoft.AspNetCore.Authentication;
using HmacManager.Mvc.Extensions.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace HmacManager.Mvc.Extensions;

/// <summary>
/// A class representing extension methods on an <c>AuthenticationBuilder</c>.
/// </summary>
public static class AuthenticationBuilderExtensions
{
    /// <summary>
    /// Adds HmacAuthentication to the <c>AuthenticationBuilder</c> with the
    /// configured <c>HmacAuthenticationOptions</c>. This method also adds
    /// the implementation for <c>IHmacManagerFactory</c> to the DI container.
    /// </summary>
    /// <param name="builder">The calling <c>AuthenticationBuilder</c>.</param>
    /// <param name="configureOptions">The configuration action for <c>HmacAuthenticationOptions</c>.</param>
    /// <returns>An <c>AuthenticationBuilder</c></returns>
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