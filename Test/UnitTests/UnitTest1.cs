using HmacManagement.Components;

namespace UnitTests;

public class Tests
{
    IHmacManager hmacManager;
    
    [SetUp]
    public void Setup()
    {
        hmacManager = new HmacManager()
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}