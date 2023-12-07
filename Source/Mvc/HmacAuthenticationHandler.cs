using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
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
        if (TryGetSignature(Request.Headers, out var signature))
        {
            if (TryGetManager(Request.Headers, out var hmacManager))
            {
                var hmacResult = await hmacManager
                    .VerifyAsync(Request.HttpContext.GetHttpRequestMessage());
                
                RewindBody(Request.Body);

                if (hmacResult.IsSuccess)
                {
                    var claims = new List<Claim>();
                    foreach (var headerValue in hmacResult!.Hmac!.HeaderValues)
                    {
                        claims.Add(new Claim(
                            headerValue.ClaimType, 
                            headerValue.Value
                        ));
                    }

                    if (Options.Events?.OnAuthenticationSuccess is not null)
                    {
                        var successHandlerClaims = Options.Events.OnAuthenticationSuccess(Request.HttpContext);
                        foreach (var claim in successHandlerClaims)
                        {
                            claims.Add(claim);
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
                return AuthenticateResult.Fail(new MissingHeaderException());
            }
        }
        else
        {
            return AuthenticateResult.NoResult();
        }
    }

    private void RewindBody(Stream body)
    {
        if (body.CanSeek)
        {
            body.Seek(0, SeekOrigin.Begin);
        }
    }

    private bool TryGetSignature(IHeaderDictionary headers, out string signature)
    {
        var hasValidAuthorizationHeader = 
            Request.Headers.TryGetValue("X-Hmac", out var signatureValues) && 
            !string.IsNullOrWhiteSpace(signatureValues) &&
            signatureValues.Count == 1;

        signature = signatureValues.FirstOrDefault()!;

        return hasValidAuthorizationHeader;
    }

    private bool TryGetManager(IHeaderDictionary headers, out IHmacManager manager)
    {
        var hasConfiguredPolicy = headers.TryGetValue("X-Hmac-Policy", out var policy) && !string.IsNullOrWhiteSpace(policy);
        var hasConfiguredScheme = headers.TryGetValue("X-Hmac-Scheme", out var scheme) && !string.IsNullOrWhiteSpace(scheme);

#pragma warning disable CS8604
        manager = (hasConfiguredPolicy, hasConfiguredScheme) switch
        {
            (true, true)    => _hmacManagerFactory.Create(policy, scheme),
            (true, false)   => _hmacManagerFactory.Create(policy),
            _               => default!
        };
#pragma warning restore CS8604

        return hasConfiguredPolicy;
    }
}