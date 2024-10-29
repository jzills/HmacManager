using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using HmacManager.Caching.Memory;
using HmacManager.Components;
using HmacManager.Headers;
using HmacManager.Policies;
using HmacManager.Exceptions;

namespace Unit.Tests.Components;

[TestFixture]
public class Test_HmacManager_SignAsync_MissingHeaders
{
    public readonly HmacManager.Components.HmacManager HmacManager;
    public readonly HmacManagerOptions HmacManagerOptions;
    public readonly HmacSignatureProviderOptions HmacSignatureProviderOptions;

    public Test_HmacManager_SignAsync_MissingHeaders()
    {
        HmacSignatureProviderOptions = new HmacSignatureProviderOptions
        {
            Algorithms = new Algorithms
            {
                ContentHashAlgorithm = ContentHashAlgorithm.SHA1,
                SigningHashAlgorithm = SigningHashAlgorithm.HMACSHA1
            },
            Keys = new KeyCredentials
            {
                PublicKey = Guid.NewGuid(),
                PrivateKey = "xCy0Ucg3YEKlmiK23Zph+g=="
            },
            ContentHashGenerator = new ContentHashGenerator(ContentHashAlgorithm.SHA1),
            SignatureHashGenerator = new SignatureHashGenerator("xCy0Ucg3YEKlmiK23Zph+g==", SigningHashAlgorithm.HMACSHA1)
        };

        HmacManagerOptions = new HmacManagerOptions("Policy")
        {
            MaxAgeInSeconds = 60,
            HeaderScheme = new HeaderScheme("Scheme")
        };

        HmacManagerOptions.HeaderScheme.AddHeader("X-Required-Header");

        HmacManager = new HmacManager.Components.HmacManager(
            HmacManagerOptions, 
            new HmacFactory(new HmacSignatureProvider(HmacSignatureProviderOptions)), 
            new HmacResultFactory(HmacManagerOptions.Policy, HmacManagerOptions.HeaderScheme.Name),
            new NonceMemoryCache(
                new MemoryCache(Options.Create(new MemoryCacheOptions())), 
                new HmacManager.Caching.NonceCacheOptions()
            )
        );
    }

    [TestCaseSource(typeof(TestCaseSource), nameof(TestCaseSource.GetHttpRequestMessages))]
    public async Task Test_SignAsync_IsFailure_MissingHeaders(HttpRequestMessage request)
    {
        Assert.ThrowsAsync<MissingHeaderException>(async () => await HmacManager.SignAsync(request));
    }
}