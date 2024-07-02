using System.Security.Cryptography;
using HmacManager.Components;
using HmacManager.Policies;

namespace Unit.Tests.Components;

[TestFixture("a18f5729-32ce-43a4-ac4d-af0a699539ae", "xCy0Ucg3YEKlmiK23Zph+g==", ContentHashAlgorithm.SHA1)]
[TestFixture("84a778d9-b4cf-4971-ada5-0ce009d1bf86", "Hii9mvaSlUm9RRLwsfuUcg==", ContentHashAlgorithm.SHA256)]
[TestFixture("b6ce3749-a4e3-4391-93ca-eaa20dd56ea3", "zdYk8nnPdE2khpt2ROgadw==", ContentHashAlgorithm.SHA512)]
public class Test_ContentHashGenerator
{
    public ContentHashGenerator Generator;

    public readonly HmacProviderOptions Options;

    public Test_ContentHashGenerator(
        string publicKey, 
        string privateKey, 
        ContentHashAlgorithm contentHashAlgorithm
    )
    {
        Options = new HmacProviderOptions
        {
            Algorithms = new Algorithms
            {
                ContentHashAlgorithm = contentHashAlgorithm,
                SigningHashAlgorithm = SigningHashAlgorithm.HMACSHA1 // Not used
            },
            Keys = new KeyCredentials
            {
                PublicKey = Guid.Parse(publicKey),
                PrivateKey = privateKey
            },
            ContentHashGenerator = new ContentHashGenerator(ContentHashAlgorithm.SHA1),
            SignatureHashGenerator = new SignatureHashGenerator(privateKey, SigningHashAlgorithm.HMACSHA1),
            SigningContentBuilder = new SigningContentBuilder()
        };
    }

    [SetUp]
    public void Init() => Generator = new ContentHashGenerator(Options.Algorithms.ContentHashAlgorithm);

    [Test]
    [TestCaseSource(nameof(GetContent))]
    public async Task Test_HashAsync_With_HashExecutor(string content)
    {
        var contentHash = await Generator.HashAsync(content);
        using HashAlgorithm hashAlgorithm = Options.Algorithms.ContentHashAlgorithm switch
        {
            ContentHashAlgorithm.SHA1   => SHA1  .Create(),
            ContentHashAlgorithm.SHA256 => SHA256.Create(),
            ContentHashAlgorithm.SHA512 => SHA512.Create(),
            _                           => SHA256.Create()
        };

        var hashExecutor = new HashExecutor(hashAlgorithm);
        var contentHashInternal = await hashExecutor.ExecuteAsync(content);

        Assert.That(contentHash, Is.EqualTo(contentHashInternal));
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