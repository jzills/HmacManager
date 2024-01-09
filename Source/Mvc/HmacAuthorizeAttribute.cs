using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HmacManager.Mvc;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class HmacAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
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
                    if (authenticateResult.Properties.Items.TryGetValue("Policy", out var policy) &&
                        authenticateResult.Properties.Items.TryGetValue("Scheme", out var scheme))
                    {
                        var hasMatchingPolicyScheme = 
                            Policy == policy && 
                            Scheme == scheme;

                        if (!hasMatchingPolicyScheme)
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
}