using HmacManager.Mvc.Extensions.Internal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HmacManager.Mvc;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class HmacAuthenticateAttribute : Attribute, IAuthorizationRequirement, IAsyncAuthorizationFilter
{
    public required string Policy { get; init; }
    public string? Scheme { get; init; } 

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.HttpContext is not null)
        {
            var authenticateResult = await context.HttpContext.AuthenticateAsync(HmacAuthenticationDefaults.AuthenticationScheme);
            if (authenticateResult is not null)
            {
                if (authenticateResult.Succeeded)
                {
                    if (authenticateResult.Properties.TryGetHmacProperties(out var policy, out var scheme))
                    {
                        if (!IsMatch(policy, scheme))
                        {
                            context.Result = new UnauthorizedResult();
                            return;
                        }
                    }
                }
                else
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }
        }
    }

    internal bool IsMatch(string? policy, string? scheme) =>
        (Policy, Scheme) switch
        {
            (null, null)    => false,
            (_, null)       => Policy == policy,
            (_, _)          => Policy == policy && Scheme == scheme
        };
}