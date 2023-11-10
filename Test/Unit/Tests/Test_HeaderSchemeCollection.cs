using HmacManagement.Headers;

namespace Unit.Tests;

public class Test_HeaderSchemeCollection
{
    [Test]
    public void Test()
    {
        var schemeCollection = new HeaderSchemeCollection();
        schemeCollection.AddScheme("MyScheme", options =>
        {
            options.AddRequiredHeader("X-Account-Id");
            options.AddRequiredHeader("X-Email");
        });

        var myScheme = schemeCollection.GetHeaderScheme("MyScheme");
        
        Assert.True(myScheme.Name == "MyScheme");
        Assert.True(myScheme.GetRequiredHeaders().Count == 2);
    }
}