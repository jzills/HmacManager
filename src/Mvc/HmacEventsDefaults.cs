using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using HmacManager.Components;
using HmacManager.Exceptions;
using HmacManager.Policies;

namespace HmacManager.Mvc;

internal static class HmacEventsDefaults
{
    internal static Func<HttpContext, HmacResult, Task<Claim[]>> OnAuthenticationSuccessAsync = (_, _) => Task.FromResult<Claim[]>([]);
    internal static Func<HttpContext, HmacResult, Task<Exception>> OnAuthenticationFailureAsync = (_, _) => Task.FromResult<Exception>(new HmacAuthenticationException());
    internal static Func<HttpContext, KeyCredentials, Task<bool>> OnValidateKeysAsync = (_, _) => Task.FromResult(true);
}