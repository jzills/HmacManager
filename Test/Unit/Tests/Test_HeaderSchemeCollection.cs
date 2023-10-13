using HmacManagement.Headers;

namespace Unit.Tests;

public class Test_HeaderSchemeCollection
{
    [Test]
    public void Test()
    {
        var schemeCollection = new HeaderSchemeCollection();
        schemeCollection.Add(new HeaderScheme(""));
    }
}