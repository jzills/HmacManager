using Microsoft.AspNetCore.Authorization;

namespace HmacManager.Mvc;

public static class AuthorizationPolicyBuilderExtensions
{
    public static AuthorizationPolicyBuilder RequireHmacAuthentication(
        this AuthorizationPolicyBuilder builder,
        string  policy,
        string? scheme = null
    ) 
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(policy, nameof(policy));
        
        builder.RequireHmacPolicy(policy);

        if (!string.IsNullOrWhiteSpace(scheme))
        {
            builder.RequireHmacScheme(scheme);
        }

        return builder;
    }

    public static AuthorizationPolicyBuilder RequireHmacPolicy(
        this AuthorizationPolicyBuilder builder, 
        params string[] policies
    ) 
    {
        ThrowIfAnyAreNullOrWhiteSpace(policies);

        return builder.RequireClaim(HmacAuthenticationDefaults.Properties.PolicyProperty, policies);
    }

    public static AuthorizationPolicyBuilder RequireHmacScheme(
        this AuthorizationPolicyBuilder builder, 
        params string[] schemes
    ) 
    {
        ThrowIfAnyAreNullOrWhiteSpace(schemes);

        return builder.RequireClaim(HmacAuthenticationDefaults.Properties.SchemeProperty, schemes);
    }

    private static void ThrowIfAnyAreNullOrWhiteSpace(params string[] values)
    {
        foreach (var value in values)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
        }
    }
}