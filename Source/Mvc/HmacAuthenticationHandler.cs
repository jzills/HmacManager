using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using HmacManagement.Components;
using HmacManagement.Exceptions;

namespace HmacManagement.Mvc;

public class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationOptions>
{
    private readonly IHmacManagerFactory _hmacManagerFactory;

    public HmacAuthenticationHandler(
        IHmacManagerFactory hmacManagerFactory,
        IOptionsMonitor<HmacAuthenticationOptions> options, 
        ILoggerFactory logger, 
        ISystemClock clock,
        UrlEncoder encoder
    ) : base(options, logger, encoder, clock)
    {
        _hmacManagerFactory = hmacManagerFactory;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Headers.TryGetValue("X-Hmac", out var value))
        {
            IHmacManager hmacManager;
            string? policy = null;
            string? scheme = null;

            // Key: X-Hmac-Policy Value: MyPolicy HmacSignature
            // Key: X-Hmac-Policy Value: MyPolicy MyScheme HmacSignature
            var hmacHeaderValues = value.First()?.Split(" ");
            if (hmacHeaderValues?.Length == 2)
            {
                policy = hmacHeaderValues[0];
                hmacManager = _hmacManagerFactory.Create(policy);
            }
            else if (hmacHeaderValues?.Length == 3)
            {
                policy = hmacHeaderValues[0];
                scheme = hmacHeaderValues[1];
                hmacManager = _hmacManagerFactory.Create(policy, scheme);
            }
            else
            {
                throw new Exception();
            }

            var hmacResult = await hmacManager.VerifyAsync(Request.HttpContext.GetHttpRequestMessage());
            if (Request.HttpContext.Request.Body.CanSeek)
            {
                Request.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            if (hmacResult.IsSuccess)
            {
                List<Claim> claims = new List<Claim>();

                if (!string.IsNullOrEmpty(scheme))
                {
                    var claimsHeaders = Options.GetPolicy(policy)
                        !.GetHeaderScheme(scheme)
                        !.GetRequiredHeaders();

                    foreach (var claimsHeader in claimsHeaders)
                    {
                        var claimsValue = Request.Headers[claimsHeader.Name];
                        claims.Add(new Claim(claimsHeader.ClaimType, claimsValue!.First()!));
                    }
                }

                if (Options.Events?.OnAuthenticationSuccess is not null)
                {
                    var successHandlerClaims = Options.Events.OnAuthenticationSuccess(Request.HttpContext);
                    foreach (var claim in successHandlerClaims)
                    {
                        if (!claims.Contains(claim))
                        {
                            claims.Add(claim);
                        }
                    }
                }

                return AuthenticateResult.Success(
                    new AuthenticationTicket(
                        new ClaimsPrincipal(
                            new ClaimsIdentity(claims, 
                                HmacAuthenticationDefaults.AuthenticationScheme)), 
                        new AuthenticationProperties(), 
                        HmacAuthenticationDefaults.AuthenticationScheme
                    ));
            }
            else
            {
                if (Options.Events?.OnAuthenticationFailure is not null)
                {
                    var exception = Options.Events.OnAuthenticationFailure(Request.HttpContext);
                    return AuthenticateResult.Fail(exception);
                }
                else
                {
                    return AuthenticateResult.Fail(new HmacAuthenticationException());
                }
            }
        }
        else
        {
            return AuthenticateResult.NoResult();
        }
    }
}
