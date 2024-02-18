namespace Unit.Tests;

public class Test_Memory_ReplayAttack : TestServiceCollection
{
    [Test]
    [TestCaseSource(typeof(TestCaseSource), nameof(TestCaseSource.GetHttpRequestMessages))]
    public async Task Test(HttpRequestMessage request)
    {
        var hmacManager = HmacManagerFactory.Create("Policy_Memory");
        var signingResult = await hmacManager.SignAsync(request);
        Assert.IsTrue(signingResult.IsSuccess);
        Assert.IsTrue(signingResult.Policy == "Policy_Memory");
        Assert.IsTrue(signingResult.Hmac?.HeaderValues?.Count() == 0);
        Assert.IsNull(signingResult.HeaderScheme);

        var verificationResult = await hmacManager.VerifyAsync(request);
        Assert.IsTrue(signingResult.IsSuccess);
        Assert.IsTrue(signingResult.Policy == "Policy_Memory");
        Assert.IsTrue(signingResult.Hmac?.HeaderValues?.Count() == 0);
        Assert.IsNull(signingResult.HeaderScheme);

        var replayVerificationResult = await hmacManager.VerifyAsync(request);
        Assert.IsFalse(replayVerificationResult.IsSuccess);
    }
}