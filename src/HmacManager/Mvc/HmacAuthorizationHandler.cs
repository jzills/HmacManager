using Microsoft.AspNetCore.Authorization;
using HmacManager.Mvc.Extensions.Internal;

namespace HmacManager.Mvc;

public class HmacAuthorizationHandler : AuthorizationHandler<HmacAuthenticateAttribute>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        HmacAuthenticateAttribute requirement
    )
    {
        if (context.User.TryGetPolicyScheme(out var policy, out var scheme) && 
             requirement.IsMatch(policy, scheme))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}