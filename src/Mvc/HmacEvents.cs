using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using HmacManager.Components;
using HmacManager.Policies;

namespace HmacManager.Mvc;

/// <summary>
/// A class representing <c>HmacEvents</c>.
/// </summary>
public class HmacEvents
{
    /// <summary>
    /// The event fired when authentication is successful.
    /// </summary>
    /// <remarks>The <c>HttpContext</c> and <c>HmacResult</c> are passed into the handler with
    /// the expectation that user claims will be returned and automatically added to the <c>ClaimsIdentity</c>
    /// for the authentication scheme found at <c>HmacAuthenticationDefaults.AuthenticationScheme</c>.
    /// </remarks>
    public Func<HttpContext, HmacResult, Claim[]> OnAuthenticationSuccess { get; init; } = HmacEventsDefaults.OnAuthenticationSuccess;

    /// <summary>
    /// The event fired when authentication is a failure.
    /// </summary>
    /// /// <remarks>The <c>HttpContext</c> and <c>HmacResult</c> are passed into the handler with
    /// the expectation that an exception will be returned indicating the error.
    /// </remarks>
    public Func<HttpContext, HmacResult, Exception> OnAuthenticationFailure { get; init; } = HmacEventsDefaults.OnAuthenticationFailure;

    /// <summary>
    /// The event fired prior to authenticating a request in order to prevalidate the keys.
    /// </summary>
    /// /// <remarks>The <c>HttpContext</c> and <c>KeyCredentials</c> are passed into the handler with
    /// the expectation that a <c>bool</c> will be returned indicating the validity of the keys.
    /// </remarks>
    public Func<HttpContext, KeyCredentials, bool> OnValidateKeys { get; init;  } = HmacEventsDefaults.OnValidateKeys;
}