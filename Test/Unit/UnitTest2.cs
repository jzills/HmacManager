using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HmacManagement.Components;
using HmacManagement.Remodel;

namespace Unit;

public class HmacManagerTests
{
    public readonly IHmacManager Manager;

    public HmacManagerTests()
    {
        Manager = new HmacManager(
            new HmacManagerOptions
            {
                MaxAge = TimeSpan.FromSeconds(30),
                HeaderScheme = null
            },
            new NonceCacheMock(),
            new HmacProvider(
                new HmacProviderOptions
                {
                    Keys = new KeyCredentials
                    {
                        PublicKey = Guid.Parse("4c59aec6-517c-47b0-a681-3c0251037416"),
                        PrivateKey = "CKnebrN5WUmFdIZE01O3hA=="
                    },
                    Algorithms = new Algorithms
                    {
                        ContentAlgorithm = ContentHashAlgorithm.SHA256,
                        SigningAlgorithm = SigningHashAlgorithm.HMACSHA256
                    }
                }
            )
        );
    }

    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public async Task Sign_Get_Request_With_Single_Query_Parameter()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/accounts?id=1");

        var signingResult = await Manager.SignAsync(request);
        Assert.True(signingResult.IsSuccess);
        Assert.NotNull(signingResult.Hmac);
        Assert.NotNull(signingResult.Hmac!.HeaderValues);

        var headerValues = signingResult.Hmac.HeaderValues;
        Assert.True(headerValues!.Length == 0);
    }
}