using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using HmacManager.Components;
using HmacManager.Exceptions;
using HmacManager.Extensions;
using HmacManager.Mvc.Extensions.Internal;
using HmacManager.Features;

namespace HmacManager.Mvc;

internal class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationOptions>
{
    protected readonly IHmacAuthenticationContextProvider ContextProvider;

    public HmacAuthenticationHandler(
        IHmacAuthenticationContextProvider contextProvider,
        IOptionsMonitor<HmacAuthenticationOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder
    ) : base(options, logger, encoder)
    {
        ContextProvider = contextProvider;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (ContextProvider.TryGetAuthenticationContext(Request.Headers, out var hmacAuthenticationContext))
        {
            var hasValidKeys = await Options.Events.OnValidateKeysAsync(Context, hmacAuthenticationContext.Policy.Keys);
            if (!hasValidKeys)
            {
                return AuthenticateResult.Fail(new HmacAuthenticationException());
            }

            EnableBufferingIfContentExists();

            var result = await GetResultAsync(hmacAuthenticationContext);

            RewindIfContentExists();

            if (result.IsSuccess)
            {
                var claims = await CreateClaimsAsync(result);
                return AuthenticateResult.Success(CreateSuccessTicket(claims, result));
            }
            else
            {
                var failure = await Options.Events.OnAuthenticationFailureAsync(Request.HttpContext, result);
                return AuthenticateResult.Fail(failure);
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
        if (hmacResult.Hmac?.HeaderValues?.Any() ?? false)
        {
            foreach (var headerValue in hmacResult.Hmac.HeaderValues)
            {
                claims.Add(new Claim(headerValue.ClaimType, headerValue.Value));
            }
        }

        var successHandlerClaims = await Options.Events.OnAuthenticationSuccessAsync(Request.HttpContext, hmacResult);
        if (successHandlerClaims.Any())
        {
            foreach (var claim in successHandlerClaims)
            {
                claims.Add(claim);
            }
        }

        // Add policy and header claim to support 
        // IAuthorizationRequirement for dynamic authorization policies.
        claims.Add(new Claim(HmacAuthenticationDefaults.Properties.PolicyProperty, hmacResult.Hmac.Policy));
        claims.Add(new Claim(HmacAuthenticationDefaults.Properties.SchemeProperty, hmacResult.Hmac.HeaderScheme));

        return claims;
    }

    private AuthenticationTicket CreateSuccessTicket( 
        IEnumerable<Claim> claims, 
        HmacResult result
    ) => new AuthenticationTicket(
            new ClaimsPrincipal(
                new ClaimsIdentity(claims,
                    HmacAuthenticationDefaults.AuthenticationScheme)),
            new AuthenticationProperties(new Dictionary<string, string?>
            {
                { HmacAuthenticationDefaults.Properties.PolicyProperty, result.Hmac.Policy },
                { HmacAuthenticationDefaults.Properties.SchemeProperty, result.Hmac.HeaderScheme }
            }),
            HmacAuthenticationDefaults.AuthenticationScheme
        );

    private void EnableBufferingIfContentExists()
    {
        if (Request.HasContent())
        {
            Request.EnableBuffering();
        }
    }

    private void RewindIfContentExists()
    {
        if (Request.HasContent())
        {
            Request.Body.Rewind();
        }
    }

    private Task<HmacResult> GetResultAsync(HmacAuthenticationContext context) => 
        context.HmacManager.VerifyAsync(
            Request.HttpContext.GetHttpRequestMessage()
        );
}