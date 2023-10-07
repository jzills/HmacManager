using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using HmacManagement.Components;
using HmacManagement.Exceptions;
using HmacManagement.Remodel;

namespace HmacManagement.Mvc;

public class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationOptions>
{
    private readonly IHmacManager _hmacManager;

    public HmacAuthenticationHandler(
        IHmacManager hmacManager,
        IOptionsMonitor<HmacAuthenticationOptions> options, 
        ILoggerFactory logger, 
        ISystemClock clock,
        UrlEncoder encoder
    ) : base(options, logger, encoder, clock)
    {
        _hmacManager = hmacManager;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return null;
        // if (Request.Headers.ContainsKey("X-Signing-Policy"))
        // {   
        //     var signingPolicyHeader = Request.Headers.First(header => header.Key == "X-Signing-Policy");
        //     if (string.IsNullOrEmpty(signingPolicyHeader.Value))
        //     {
        //         throw new Exception();
        //     }


        //     var hmacResult = await _hmacManager.VerifyAsync(
        //         Request.HttpContext.GetHttpRequestMessage(), 
        //         signingPolicy.PolicyName
        //     );
            
        //     if (Request.HttpContext.Request.Body.CanSeek)
        //     {
        //         Request.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
        //     }

        //     if (hmacResult.IsSuccess)
        //     {
        //         List<Claim> claims = new List<Claim>();

        //         var mappings = signingPolicy.HeaderClaimMappings.GetHeaderClaimMappings();
        //         foreach (var mapping in mappings)
        //         {
        //             var value = Request.Headers[mapping.HeaderName];
        //             claims.Add(new Claim(mapping.ClaimType, value.First()));
        //         }

        //         if (Options.Events?.OnAuthenticationSuccess is not null)
        //         {
        //             var handlerClaims = Options.Events.OnAuthenticationSuccess(Request.HttpContext);
        //             foreach (var claim in handlerClaims)
        //             {
        //                 if (!claims.Contains(claim))
        //                 {
        //                     claims.Add(claim);
        //                 }
        //             }
        //         }

        //         return AuthenticateResult.Success(
        //             new AuthenticationTicket(
        //                 new ClaimsPrincipal(
        //                     new ClaimsIdentity(claims, 
        //                         HmacAuthenticationDefaults.AuthenticationScheme)), 
        //                 new AuthenticationProperties(), 
        //                 HmacAuthenticationDefaults.AuthenticationScheme
        //             ));
        //     }
        //     else
        //     {
        //         if (Options.Events?.OnAuthenticationFailure is not null)
        //         {
        //             var exception = Options.Events.OnAuthenticationFailure(Request.HttpContext);
        //             return AuthenticateResult.Fail(exception);
        //         }
        //         else
        //         {
        //             return AuthenticateResult.Fail(new HmacAuthenticationException());
        //         }
        //     }
        // }
        // else
        // {
        //     var hmacResult = await _hmacManager.VerifyAsync(
        //         Request.HttpContext.GetHttpRequestMessage(), new HeaderScheme(""));
            
        //     if (Request.HttpContext.Request.Body.CanSeek)
        //     {
        //         Request.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
        //     }

        //     if (hmacResult.IsSuccess)
        //     {
        //         IEnumerable<Claim> claims = Enumerable.Empty<Claim>();
        //         if (Options.Events?.OnAuthenticationSuccess is not null)
        //         {
        //             claims = Options.Events.OnAuthenticationSuccess(Request.HttpContext);
        //         }

        //         return AuthenticateResult.Success(
        //             new AuthenticationTicket(
        //                 new ClaimsPrincipal(
        //                     new ClaimsIdentity(claims, 
        //                         HmacAuthenticationDefaults.AuthenticationScheme)), 
        //                 new AuthenticationProperties(), 
        //                 HmacAuthenticationDefaults.AuthenticationScheme
        //             ));
        //     }
        //     else
        //     {
        //         if (Options.Events?.OnAuthenticationFailure is not null)
        //         {
        //             var exception = Options.Events.OnAuthenticationFailure(Request.HttpContext);
        //             return AuthenticateResult.Fail(exception);
        //         }
        //         else
        //         {
        //             return AuthenticateResult.Fail(new HmacAuthenticationException());
        //         }
        //     }
        // }
    }
}
