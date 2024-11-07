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

/// <summary>
/// Handles HMAC-based authentication by verifying requests and generating authentication results.
/// </summary>
internal class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationOptions>
{
    /// <summary>
    /// Provides access to the HMAC authentication context for request verification.
    /// </summary>
    protected readonly IHmacAuthenticationContextProvider ContextProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="HmacAuthenticationHandler"/> class with the specified context provider, options, logger, and URL encoder.
    /// </summary>
    /// <param name="contextProvider">The provider for HMAC authentication context information.</param>
    /// <param name="options">The options monitor for <see cref="HmacAuthenticationOptions"/>.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="encoder">The URL encoder.</param>
    public HmacAuthenticationHandler(
        IHmacAuthenticationContextProvider contextProvider,
        IOptionsMonitor<HmacAuthenticationOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder
    ) : base(options, logger, encoder)
    {
        ContextProvider = contextProvider;
    }

    /// <summary>
    /// Attempts to authenticate the request using HMAC authentication. 
    /// Returns a success result if authentication succeeds, or a failure result otherwise.
    /// </summary>
    /// <returns>A <see cref="Task{AuthenticateResult}"/> representing the authentication outcome.</returns>
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

    /// <summary>
    /// Creates a collection of claims based on the HMAC result and any additional claims from success events.
    /// </summary>
    /// <param name="hmacResult">The result of the HMAC verification.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of claims.</returns>
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

        // Adds policy and header claims to support dynamic authorization policies.
        claims.Add(new Claim(HmacAuthenticationDefaults.Properties.PolicyProperty, hmacResult.Hmac.Policy));
        claims.Add(new Claim(HmacAuthenticationDefaults.Properties.SchemeProperty, hmacResult.Hmac.HeaderScheme));

        return claims;
    }

    /// <summary>
    /// Creates an <see cref="AuthenticationTicket"/> representing a successful authentication.
    /// </summary>
    /// <param name="claims">The collection of claims generated for the user.</param>
    /// <param name="result">The HMAC result containing policy and scheme information.</param>
    /// <returns>The authentication ticket for the authenticated user.</returns>
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

    /// <summary>
    /// Enables buffering for the request body if content exists, allowing it to be read multiple times.
    /// </summary>
    private void EnableBufferingIfContentExists()
    {
        if (Request.HasContent())
        {
            Request.EnableBuffering();
        }
    }

    /// <summary>
    /// Rewinds the request body stream if content exists, resetting the position to the beginning.
    /// </summary>
    private void RewindIfContentExists()
    {
        if (Request.HasContent())
        {
            Request.Body.Rewind();
        }
    }

    /// <summary>
    /// Verifies the HMAC authentication result for the given context.
    /// </summary>
    /// <param name="context">The HMAC authentication context containing request information.</param>
    /// <returns>A task that represents the asynchronous operation, containing the HMAC verification result.</returns>
    private Task<HmacResult> GetResultAsync(HmacAuthenticationContext context) => 
        context.HmacManager.VerifyAsync(
            Request.HttpContext.GetHttpRequestMessage()
        );
}