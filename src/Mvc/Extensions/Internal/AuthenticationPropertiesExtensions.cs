using Microsoft.AspNetCore.Authentication;

namespace HmacManager.Mvc.Extensions.Internal;

internal static class AuthenticationPropertiesExtensions
{
    internal static bool TryGetHmacProperties(this AuthenticationProperties properties, out string? policy, out string? scheme)
    {
        var hasPolicy = properties.Items.TryGetValue(HmacAuthenticationDefaults.Properties.PolicyProperty, out policy);
        var hasScheme = properties.Items.TryGetValue(HmacAuthenticationDefaults.Properties.SchemeProperty, out scheme);
        return hasPolicy && hasScheme;
    }
}