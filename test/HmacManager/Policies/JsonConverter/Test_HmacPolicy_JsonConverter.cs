using System.Text.Json;
using HmacManager.Policies;

namespace Unit.Tests;

public class Test_HmacPolicy_JsonConverter : TestBase
{
    [Test]
    [TestCaseSource(nameof(GetJson))]
    public void Test(HmacPolicy policy) => Assert.DoesNotThrow(() => JsonSerializer.Serialize(policy));

    public static IEnumerable<HmacPolicy> GetJson()
    {
        using var stream = new StreamReader("../../../../Data/hmac_policy_collection.json");
        var policies = JsonSerializer.Deserialize<List<HmacPolicy>>(stream.ReadToEnd());
        return policies ?? throw new FormatException("The data at \"hmac_policy_collection.json\" could not be read in for testing.");
    }
}