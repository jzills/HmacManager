using HmacManager.Components;
using HmacManager.Extensions;
using HmacManager.Mvc;

namespace Unit.Tests;

public class Test_DateTimeOffsetExtensions_HasValidDateRequested : TestBase
{
    [Test]
    public void Test_HasValidDateRequested_IsTrue()
    {
        Assert.That(DateTimeOffset.UtcNow.HasValidDateRequested(10), Is.True);
    }

    [Test]
    public void Test_HasValidDateRequested_IsFalse()
    {
        Assert.That(DateTimeOffset.UtcNow.AddSeconds(-10).HasValidDateRequested(10), Is.False);
    }
}