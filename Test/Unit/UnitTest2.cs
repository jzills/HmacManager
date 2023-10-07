using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HmacManagement.Components;
using HmacManagement.Remodel;

namespace Unit;

public class Tests1
{
    public readonly IHmacManager Manager;

    public Tests1()
    {
        Manager = new HmacManager(
            new HmacManagerOptions
            {
                MaxAge = TimeSpan.FromSeconds(30)
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
        var scheme = new HeaderScheme("MyScheme");
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/accounts?id=1");
        
        var signingResult = await Manager.SignAsync(request, scheme);
        Assert.True(signingResult.IsSuccess);
        Assert.NotNull(signingResult.Hmac);
        Assert.NotNull(signingResult.Hmac!.HeaderValues);

        var headerValues = signingResult.Hmac.HeaderValues;
        Assert.True(headerValues!.Length == 0);
    }

    [Test]
    [TestCase("9012313", "X-Id", "")]
    //[TestCase("7735541", "X-Id", null)]
    [TestCase("4c59aec6-517c-47b0-a681-3c0251037416", "X-Account-Id", "1")]
    [TestCase("thisIsMyQueryParameter", "X-Scheme-Header", "4c59aec6-517c-47b0-a681-3c0251037416")]
    public async Task Sign_One_Scheme_Header_Get_Request_With_Single_Query_Parameter(
        string queryParameter,
        string schemeHeader, 
        string schemeHeaderValue
    )
    {
        var scheme = new HeaderScheme("MyScheme");
        scheme.AddRequiredHeader(schemeHeader);

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/accounts?id={queryParameter}");
        request.Headers.Add(schemeHeader, schemeHeaderValue);

        var signingResult = await Manager.SignAsync(request, scheme);
        Assert.True(signingResult.IsSuccess);
        Assert.NotNull(signingResult.Hmac);
        Assert.NotNull(signingResult.Hmac!.HeaderValues);

        var headerValues = signingResult.Hmac.HeaderValues;
        Assert.DoesNotThrow(() => headerValues!.First(header => header.Name == schemeHeader));
        Assert.True(headerValues!.First(header => header.Name == schemeHeader).Value == schemeHeaderValue);
    }

    [TestCase("9012313", "X-Id", "MyHeaderValue")]
    public async Task Sign_One_Scheme_Header_Post_Request(
        string queryParameter,
        string schemeHeader, 
        string schemeHeaderValue
    )
    {
        var scheme = new HeaderScheme("MyScheme");
        scheme.AddRequiredHeader(schemeHeader);

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/accounts?id={queryParameter}");
        request.Headers.Add(schemeHeader, schemeHeaderValue);
        request.Content = new StringContent(JsonSerializer.Serialize(new
        {
            Id = Guid.NewGuid(),
            Type = "Account",
            Date = DateTime.Now
        }), Encoding.UTF8, new MediaTypeHeaderValue("application/json"));

        var signingResult = await Manager.SignAsync(request, scheme);
        Assert.True(signingResult.IsSuccess);
        Assert.NotNull(signingResult.Hmac);
        Assert.NotNull(signingResult.Hmac!.HeaderValues);

        var headerValues = signingResult.Hmac.HeaderValues;
        Assert.DoesNotThrow(() => headerValues!.First(header => header.Name == schemeHeader));
        Assert.True(headerValues!.First(header => header.Name == schemeHeader).Value == schemeHeaderValue);
    }

    [Test]
    public async Task Verify_Get_Request_With_Single_Query_Parameter()
    {
        var scheme = new HeaderScheme("MyScheme");
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/accounts?id=1");

        var signingResult = await Manager.SignAsync(request, scheme);
        Assert.True(signingResult.IsSuccess);
        Assert.NotNull(signingResult.Hmac);

        var verificationResult = await Manager.VerifyAsync(request, scheme);
        Assert.True(verificationResult.IsSuccess);
        Assert.NotNull(verificationResult.Hmac);

        Assert.True(signingResult.Hmac!.Signature == verificationResult.Hmac!.Signature);
    }
}