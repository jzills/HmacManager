using System.Security.Cryptography;
using HmacManager.Components;
using HmacManager.Policies;

namespace Unit.Tests.Components;

[TestFixture("a18f5729-32ce-43a4-ac4d-af0a699539ae", "xCy0Ucg3YEKlmiK23Zph+g==", SigningHashAlgorithm.HMACSHA1)]
[TestFixture("84a778d9-b4cf-4971-ada5-0ce009d1bf86", "Hii9mvaSlUm9RRLwsfuUcg==", SigningHashAlgorithm.HMACSHA256)]
[TestFixture("b6ce3749-a4e3-4391-93ca-eaa20dd56ea3", "zdYk8nnPdE2khpt2ROgadw==", SigningHashAlgorithm.HMACSHA512)]
public class Test_SignatureHashGenerator
{
    public SignatureHashGenerator Generator;

    public readonly HmacProviderOptions Options;

    public Test_SignatureHashGenerator(
        string publicKey, 
        string privateKey, 
        SigningHashAlgorithm signingHashAlgorithm
    )
    {
        Options = new HmacProviderOptions
        {
            Algorithms = new Algorithms
            {
                ContentHashAlgorithm = ContentHashAlgorithm.SHA1,  // Not used
                SigningHashAlgorithm = signingHashAlgorithm
            },
            Keys = new KeyCredentials
            {
                PublicKey = Guid.Parse(publicKey),
                PrivateKey = privateKey
            }
        };
    }

    [SetUp]
    public void Init() => Generator = new SignatureHashGenerator(Options);

    [Test]
    [TestCaseSource(nameof(GetContent))]
    public async Task Test_HashAsync_With_HashExecutor(string content)
    {
        var signatureHash = await Generator.HashAsync(content);
        var keyBytes = Convert.FromBase64String(Options.Keys.PrivateKey);
        using HashAlgorithm hashAlgorithm = Options.Algorithms.SigningHashAlgorithm switch
        {
            SigningHashAlgorithm.HMACSHA1   => new HMACSHA1  (keyBytes),
            SigningHashAlgorithm.HMACSHA256 => new HMACSHA256(keyBytes),
            SigningHashAlgorithm.HMACSHA512 => new HMACSHA512(keyBytes),
            _                               => new HMACSHA256(keyBytes)
        };

        var hashExecutor = new HashExecutor(hashAlgorithm);
        var signatureHashInternal = await hashExecutor.ExecuteAsync(content);

        Assert.That(signatureHash, Is.EqualTo(signatureHashInternal));
    }

    public static IEnumerable<string> GetContent() =>
    [
        "ThisIsMyContent",
        Guid.Empty.ToString(),
        Guid.NewGuid().ToString(),
        "A",
        "Here is some more content with spaces",
        "Special Characters: /<>!@#$%^&*()_-+='[],.`~{}"
    ];
}