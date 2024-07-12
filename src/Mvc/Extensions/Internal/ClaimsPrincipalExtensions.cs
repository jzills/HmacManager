using System.Security.Claims;

namespace HmacManager.Mvc.Extensions.Internal;

internal static class ClaimsPrincipalExtensions
{
    internal static bool TryGetPolicyScheme(this ClaimsPrincipal user, out string? policy, out string? scheme)
    {
        var userIdentity = user.Identities.FirstOrDefault(userIdentity => userIdentity.AuthenticationType == HmacAuthenticationDefaults.AuthenticationScheme);
        policy = userIdentity?.FindFirst(HmacAuthenticationDefaults.Properties.PolicyProperty)?.Value;
        scheme = userIdentity?.FindFirst(HmacAuthenticationDefaults.Properties.SchemeProperty)?.Value;
        return !string.IsNullOrWhiteSpace(policy);
    }
}