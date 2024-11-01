using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using HmacManager.Components;
using HmacManager.Exceptions;
using HmacManager.Extensions;
using HmacManager.Policies;
using HmacManager.Mvc.Extensions.Internal;
using HmacManager.Features;

namespace HmacManager.Mvc;

internal class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationOptions>
{
    protected readonly IHmacManagerFactory HmacManagerFactory;
    protected readonly IHmacPolicyCollection HmacPolicies;

    public HmacAuthenticationHandler(
        IHmacManagerFactory hmacManagerFactory,
        IHmacPolicyCollection hmacPolicies,
        IOptionsMonitor<HmacAuthenticationOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder
    ) : base(options, logger, encoder)
    {
        HmacManagerFactory = hmacManagerFactory;
        HmacPolicies = hmacPolicies;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Headers.TryGetSignature(out var hmacSignature))
        {
            if (Request.Headers.TryGetHmacManager(HmacManagerFactory, HmacPolicies, 
                    out var hmacManager, 
                    out var hmacPolicy
            ))
            {
                var hasValidKeys = await Options.Events.OnValidateKeysAsync(Context, hmacPolicy!.Keys);
                if (!hasValidKeys)
                {
                    return AuthenticateResult.Fail(new HmacAuthenticationException());
                }

                if (Request.HasContent())
                {
                    Request.EnableBuffering();
                }
        
                var hmacResult = await hmacManager!.VerifyAsync(
                    Request.HttpContext.GetHttpRequestMessage()
                );

                if (Request.HasContent())
                {
                    Request.Body.Rewind();
                }

                if (hmacResult.IsSuccess)
                {
                    var claims = await CreateClaimsAsync(hmacResult);
                    return AuthenticateResult.Success(CreateSuccessTicket(claims, 
                        hmacResult.Policy, 
                        hmacResult.HeaderScheme
                    ));
                }
                else
                {
                    var failure = await Options.Events.OnAuthenticationFailureAsync(Request.HttpContext, hmacResult);
                    return AuthenticateResult.Fail(failure);
                }
            }
            else
            {
                return AuthenticateResult.Fail(new MissingHeaderException());
            }
        }
        else
        {
            return AuthenticateResult.NoResult();
        }
    }

    private async Task<IEnumerable<Claim>> CreateClaimsAsync(HmacResult hmacResult)
    {
        var claims = new List<Claim>();
        foreach (var headerValue in hmacResult!.Hmac!.HeaderValues)
        {
            claims.Add(new Claim(
                headerValue.ClaimType, 
                headerValue.Value
            ));
        }

        var successHandlerClaims = await Options.Events.OnAuthenticationSuccessAsync(Request.HttpContext, hmacResult);
        foreach (var claim in successHandlerClaims)
        {
            claims.Add(claim);
        }

        // Add policy and header claim to support 
        // IAuthorizationRequirement for dynamic authorization policies.
        claims.Add(new Claim(HmacAuthenticationDefaults.Properties.PolicyProperty, hmacResult.Policy));
        claims.Add(new Claim(HmacAuthenticationDefaults.Properties.SchemeProperty, hmacResult.HeaderScheme));

        return claims;
    }

    private AuthenticationTicket CreateSuccessTicket( 
        IEnumerable<Claim> claims, 
        string policy, 
        string scheme
    ) => new AuthenticationTicket(
            new ClaimsPrincipal(
                new ClaimsIdentity(claims,
                    HmacAuthenticationDefaults.AuthenticationScheme)),
            new AuthenticationProperties(new Dictionary<string, string?>
            {
                { HmacAuthenticationDefaults.Properties.PolicyProperty, policy },
                { HmacAuthenticationDefaults.Properties.SchemeProperty, scheme }
            }),
            HmacAuthenticationDefaults.AuthenticationScheme
        );
}