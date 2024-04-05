using HmacManager.Extensions;
using HmacManager.Headers;

namespace Unit.Tests;

public class Test_HttpRequestHeaderExtensions_TryParseHeaders : TestBase
{
    [Test]
    public void Test()
    {
        var headerScheme = new HeaderScheme("MyScheme_1");
        headerScheme.AddHeader("X-Account-Id");
        headerScheme.AddHeader("X-Email");

        var request = new HttpRequestMessage(HttpMethod.Get, "api/endpoint");
        request.Headers.Add("X-Account-Id", Guid.NewGuid().ToString());

        var hasHeaderValues = request.Headers.TryParseHeaders(headerScheme, out var _);
        Assert.IsFalse(hasHeaderValues);
    }
}