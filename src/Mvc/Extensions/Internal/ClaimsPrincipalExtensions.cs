using System.Security.Claims;

namespace HmacManager.Mvc.Extensions.Internal;

internal static class ClaimsPrincipalExtensions
{
    public static bool TryGetPolicyScheme(this ClaimsPrincipal user, out string? policy, out string? scheme)
    {
        var identity = user.Identities.FirstOrDefault(identity => identity.AuthenticationType == HmacAuthenticationDefaults.AuthenticationScheme);
        policy = identity?.FindFirst(HmacAuthenticationDefaults.Properties.PolicyProperty)?.Value;
        scheme = identity?.FindFirst(HmacAuthenticationDefaults.Properties.SchemeProperty)?.Value;
        return !string.IsNullOrWhiteSpace(policy);
    }
}