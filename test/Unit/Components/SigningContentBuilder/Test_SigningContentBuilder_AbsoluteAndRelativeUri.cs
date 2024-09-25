namespace Unit.Tests.Components;

public class Test_SigningContentBuilder_AbsoluteAndRelativeUri
{
    [Test]
    public async Task Test()
    {
        var requestRelative = new HttpRequestMessage(HttpMethod.Get, "/api?q=1");
        var requestAbsolute = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44342/api?q=1");

        var signingContentRelative = new HmacManager.Components.SigningContentBuilder()
            .WithRequest(requestRelative)
            .Build();

        var signingContentAbsolute = new HmacManager.Components.SigningContentBuilder()
            .WithRequest(requestAbsolute)
            .Build();

        Assert.That(signingContentRelative, Contains.Substring(":/api?q=1:"));
        Assert.That(signingContentAbsolute, Contains.Substring(":/api?q=1:"));
    }
}