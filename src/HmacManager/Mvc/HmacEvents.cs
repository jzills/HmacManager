using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using HmacManager.Components;

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
    public Func<HttpContext, HmacResult, Claim[]>? OnAuthenticationSuccess { get; set; }

    /// <summary>
    /// The event fired when authentication is a failure.
    /// </summary>
    /// /// <remarks>The <c>HttpContext</c> and <c>HmacResult</c> are passed into the handler with
    /// the expectation that an exception will be returned indicating the error.
    /// </remarks>
    public Func<HttpContext, HmacResult, Exception>? OnAuthenticationFailure { get; set; }
}