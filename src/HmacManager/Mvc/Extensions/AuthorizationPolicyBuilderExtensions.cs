using Microsoft.AspNetCore.Authorization;

namespace HmacManager.Mvc;

public static class AuthorizationPolicyBuilderExtensions
{
    public static AuthorizationPolicyBuilder RequireHmacPolicy(
        this AuthorizationPolicyBuilder builder, 
        params string[] allowedPolicies
    ) => builder.RequireClaim(HmacAuthenticationDefaults.Properties.PolicyProperty, allowedPolicies);

    public static AuthorizationPolicyBuilder RequireHmacScheme(
        this AuthorizationPolicyBuilder builder, 
        params string[] allowedSchemes
    ) => builder.RequireClaim(HmacAuthenticationDefaults.Properties.SchemeProperty, allowedSchemes);
}