using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using HmacManagement.Components;

namespace HmacManagement.Mvc;

public class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationOptions>
{
    private readonly IHmacManager _hmacManager;

    public HmacAuthenticationHandler(
        IHmacManager hmacManager,
        IOptionsMonitor<HmacAuthenticationOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder, 
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
        _hmacManager = hmacManager;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var hmacResult = await _hmacManager.VerifyAsync(
            Request.HttpContext.GetHttpRequestMessage());
            
        if (Request.HttpContext.Request.Body.CanSeek)
        {
            Request.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
        }

        if (hmacResult.IsSuccess)
        {
            IEnumerable<Claim> claims = Enumerable.Empty<Claim>();
            if (Options.Events?.OnAuthenticationSuccess is not null)
            {
                claims = Options.Events.OnAuthenticationSuccess(Request.HttpContext);
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
            return AuthenticateResult.Fail(new Exception());
        }
    }
}
