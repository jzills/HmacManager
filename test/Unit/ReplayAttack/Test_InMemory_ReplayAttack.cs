namespace Unit.Tests;

public class Test_Memory_ReplayAttack : TestServiceCollection
{
    [Test]
    [TestCaseSource(typeof(TestCaseSource), nameof(TestCaseSource.GetHttpRequestMessages))]
    public async Task Test(HttpRequestMessage request)
    {
        var hmacManager = HmacManagerFactory.Create("Policy_Memory");
        Assert.IsNotNull(hmacManager);

        var signingResult = await hmacManager.SignAsync(request);
        Assert.IsTrue(signingResult.IsSuccess);
        Assert.IsTrue(signingResult.Hmac?.Policy == "Policy_Memory");
        Assert.IsTrue(signingResult.Hmac!.HeaderValues!.Count() == 0);
        Assert.IsNull(signingResult.Hmac?.Scheme);

        var verificationResult = await hmacManager.VerifyAsync(request);
        Assert.IsTrue(verificationResult.IsSuccess);
        Assert.IsTrue(verificationResult.Hmac?.Policy == "Policy_Memory");
        Assert.IsTrue(verificationResult.Hmac!.HeaderValues!.Count() == 0);
        Assert.IsNull(verificationResult.Hmac?.Scheme);

        var replayVerificationResult = await hmacManager.VerifyAsync(request);
        Assert.IsFalse(replayVerificationResult.IsSuccess);
    }
}