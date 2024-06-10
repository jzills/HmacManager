using HmacManager.Headers;

namespace Unit.Tests.HeaderSchemes;

[TestFixture]
public class Test_HeaderCollection
{
    [Test]
    public void Test()
    {
        var headers = new HeaderCollection();
        Assert.Throws<ArgumentException>(() => headers.Add(new Header { Name = "Header_1", ClaimType = string.Empty }));
        Assert.Throws<ArgumentException>(() => headers.Add(new Header { Name = "Header_1", ClaimType = "          " }));
        Assert.Throws<ArgumentException>(() => headers.Add(new Header { Name = "Header_1", ClaimType = default! }));
        Assert.Throws<ArgumentException>(() => headers.Add(new Header { Name = "Header_1", ClaimType = null! }));
    }
}