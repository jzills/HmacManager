using System.Text.Json;
using HmacManager.Policies;

namespace Unit.Tests;

public class Test_HmacPolicy_JsonConverter : TestBase
{
    private static string Json = """[{"Name":"MyFirstPolicy","Keys":{"PublicKey":"a18f5729-32ce-43a4-ac4d-af0a699539ae","PrivateKey":"xCy0Ucg3YEKlmiK23Zph+g=="},"Algorithms":{"ContentHashAlgorithm":"SHA256","SigningHashAlgorithm":"HMACSHA256"},"Nonce":{"CacheType": "Memory", "MaxAge":100},"HeaderSchemes":[{"Name":"MyFirstScheme","Headers":[{"Name":"X-Required-Header-1","ClaimType":"MyRequiredHeaderClaim1"},{"Name":"X-Required-Header-2","ClaimType":"MyRequiredHeaderClaim2"}]}]}]""";

    [Test]
    public void Test()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new HmacPolicyJsonConverter());

        var result = JsonSerializer.Deserialize<HmacPolicy>(Json, options);
        Assert.DoesNotThrow(() => 
            JsonSerializer.Deserialize<HmacPolicy>(
                JsonSerializer.Serialize(result, options), options));
    }
}