using HmacManager.Components;
using HmacManager.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace Unit.Tests.Mvc;

[TestFixture]
public class Test_HmacAuthenticateAttribute_OnAuthorizationAsync : TestServiceCollection
{
    public HttpContext HttpContext;
    public AuthorizationFilterContext FilterContext;

    [SetUp]
    public void Init()
    {
        HttpContext = new DefaultHttpContext { RequestServices = ServiceProvider };
        FilterContext = new AuthorizationFilterContext(
            new ActionContext(
                HttpContext,
                new RouteData(),
                new ActionDescriptor()
            ), []);
    }

    [Test]
    public async Task Test_OnAuthorizeAsync_With_ExistingPolicy()
    {
        var hmacAuthorizationFilter = new HmacAuthenticateAttribute 
        { 
            Policy = PolicySchemeType.Policy_Memory_Scheme_1.Policy,
            Scheme = PolicySchemeType.Policy_Memory_Scheme_1.Scheme
        };

        FilterContext.Filters.Add(hmacAuthorizationFilter);

        var uri = new Uri("https://localhost:1122/api/endpoint");
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Add("Scheme_Header_1", "Scheme_Header_Value_1");

        var hmacManager = HmacManagerFactory.Create(
            PolicySchemeType.Policy_Memory_Scheme_1.Policy,
            PolicySchemeType.Policy_Memory_Scheme_1.Scheme
        );

        var signingResult = await hmacManager!.SignAsync(request);
        AddHttpRequestConfiguration(FilterContext.HttpContext.Request, uri);
        AddRequiredHmacHeaders(FilterContext.HttpContext.Request, signingResult);
        FilterContext.HttpContext.Request.Headers.Append("Scheme_Header_1", "Scheme_Header_Value_1");

        await hmacAuthorizationFilter.OnAuthorizationAsync(FilterContext);

        // FilterContext.Result is null
        // because fall through if it doesn't hit UnauthorizedResult
        // TODO: See if there is a better way to verify success
        var result = FilterContext.Result as OkObjectResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
    }
    
    [Test]
    public async Task Test_OnAuthorizeAsync_With_NonExistentPolicy()
    {
        var hmacAuthorizationFilter = new HmacAuthenticateAttribute { Policy = "Some_Policy_That_Doesn't_Exist" };
        FilterContext.Filters.Add(hmacAuthorizationFilter);

        await hmacAuthorizationFilter.OnAuthorizationAsync(FilterContext);

        var result = FilterContext.Result as UnauthorizedResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(401));
    }

    internal void AddHttpRequestConfiguration(HttpRequest request, Uri uri)
    {
        request.Host = new HostString(uri.Authority);
        request.PathBase = new PathString(uri.AbsolutePath);
        request.IsHttps = true;
        request.Method = HttpMethod.Get.ToString();
    }

    internal void AddRequiredHmacHeaders(HttpRequest request, HmacResult signingResult)
    {
        var hmac = signingResult?.Hmac;

        request.Headers.Append(
            HmacAuthenticationDefaults.Headers.DateRequested, 
            hmac?.DateRequested.UtcDateTime.Ticks.ToString()
        );

        request.Headers.Append(
            HmacAuthenticationDefaults.Headers.Nonce, 
            hmac?.Nonce.ToString()
        );

        request.Headers.Append(
            HmacAuthenticationDefaults.Headers.Policy, 
            signingResult?.Policy
        );

        request.Headers.Append(
            HmacAuthenticationDefaults.Headers.Scheme, 
            signingResult?.HeaderScheme
        );

        var authorizationHeaderValue = $"{HmacAuthenticationDefaults.AuthenticationScheme} {hmac?.Signature}";
        request.Headers.Authorization = new StringValues(authorizationHeaderValue);
    }
}