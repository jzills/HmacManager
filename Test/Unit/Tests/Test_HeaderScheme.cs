using HmacManagement.Remodel;

namespace Unit.Tests;

public class Test_HeaderScheme
{
    [Test]
    public void Add_One_RequiredHeader_Returns_One_RequiredHeader_With_Equal_ClaimType()
    {
        var scheme = new HeaderScheme("MyScheme");
        scheme.AddRequiredHeader("X-Account-Id");
        
        var headers = scheme.GetRequiredHeaders();
        Assert.True(headers.Count == 1);
        Assert.True(headers.First().Name == "X-Account-Id");
        Assert.True(headers.First().ClaimType == "X-Account-Id");
    }

    [Test]
    public void Add_One_RequiredHeader_With_ClaimType_Returns_Same_RequiredHeader()
    {
        var scheme = new HeaderScheme("MyScheme");
        scheme.AddRequiredHeader("X-Account-Id", "AccountId");
        
        var headers = scheme.GetRequiredHeaders();
        Assert.True(headers.Count == 1);
        Assert.True(headers.First().Name == "X-Account-Id");
        Assert.True(headers.First().ClaimType == "AccountId");
    }

    [Test]
    public void Init_Empty_HeaderScheme_Throws() =>
        Assert.Throws<ArgumentException>(
            () => new HeaderScheme(string.Empty));

    [Test]
    public void Init_Null_HeaderScheme_Throws() =>
        Assert.Throws<ArgumentNullException>(
            () => new HeaderScheme(null!));
}