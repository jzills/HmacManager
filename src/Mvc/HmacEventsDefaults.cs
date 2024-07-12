using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using HmacManager.Components;
using HmacManager.Exceptions;
using HmacManager.Policies;

namespace HmacManager.Mvc;

internal static class HmacEventsDefaults
{
    internal static Func<HttpContext, HmacResult, Claim[]> OnAuthenticationSuccess = (_, _) => [];
    internal static Func<HttpContext, HmacResult, Exception> OnAuthenticationFailure = (_, _) => new HmacAuthenticationException();
    internal static Func<HttpContext, KeyCredentials, bool> OnValidateKeys = (_, _) => true;
}