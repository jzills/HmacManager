using HmacManager.Headers;

namespace Unit.Tests.HeaderSchemes;

[TestFixture]
public class Test_HeaderSchemeCollection
{
    [Test]
    public void Test()
    {
        var schemes = new HeaderSchemeCollection();
        var scheme = new HeaderScheme("Scheme_1");
        scheme.AddHeader("Header_1", string.Empty);
        scheme.AddHeader("Header_2", "          ");
        scheme.AddHeader("Header_3", default!);
        scheme.AddHeader("Header_4", null!);

        var hasEmptyClaimTypes = scheme.Headers.Any(header => string.IsNullOrWhiteSpace(header.ClaimType));
        Assert.IsFalse(hasEmptyClaimTypes);
    }
}