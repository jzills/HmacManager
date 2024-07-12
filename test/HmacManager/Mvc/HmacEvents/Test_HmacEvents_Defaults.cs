using System.Security.Claims;
using HmacManager.Components;
using HmacManager.Mvc;
using HmacManager.Policies;
using Microsoft.AspNetCore.Http;

namespace Unit.Tests.Mvc;

[TestFixture]
public class Test_HmacEvents_Defaults
{
    [Test]
    public void Test_HmacEvents_Default_Init()
    {
        var events = new HmacEvents();
        Assert.That(events.OnAuthenticationFailure, Is.EqualTo(HmacEventsDefaults.OnAuthenticationFailure));
        Assert.That(events.OnAuthenticationSuccess, Is.EqualTo(HmacEventsDefaults.OnAuthenticationSuccess));
        Assert.That(events.OnValidateKeys, Is.EqualTo(HmacEventsDefaults.OnValidateKeys));
    }

    [Test]
    public void Test_HmacEvents_OnAuthenticationFailure_Init()
    {
        var events = new HmacEvents
        {
            OnAuthenticationFailure = (_, _) => new Exception("Test")
        };

        var error = events.OnAuthenticationFailure(new DefaultHttpContext(), new HmacResult("Policy", "Scheme"));
        Assert.That(error.Message, Is.EqualTo("Test"));
        Assert.That(events.OnAuthenticationSuccess, Is.EqualTo(HmacEventsDefaults.OnAuthenticationSuccess));
        Assert.That(events.OnValidateKeys, Is.EqualTo(HmacEventsDefaults.OnValidateKeys));
    }

    [Test]
    public void Test_HmacEvents_OnAuthenticationSuccess_Init()
    {
        var events = new HmacEvents
        {
            OnAuthenticationSuccess = (_, _) => [new Claim("Test", "Value")]
        };

        var claims = events.OnAuthenticationSuccess(new DefaultHttpContext(), new HmacResult("Policy", "Scheme"));
        Assert.DoesNotThrow(() => claims.First(claim => claim.Type == "Test" && claim.Value == "Value"));
        Assert.That(events.OnAuthenticationFailure, Is.EqualTo(HmacEventsDefaults.OnAuthenticationFailure));
        Assert.That(events.OnValidateKeys, Is.EqualTo(HmacEventsDefaults.OnValidateKeys));
    }

    [Test]
    public void Test_HmacEvents_OnValidateKeys_Init()
    {
        var events = new HmacEvents 
        { 
            OnValidateKeys = (_, _) => true 
        };

        var isValid = events.OnValidateKeys(new DefaultHttpContext(), new KeyCredentials());
        Assert.That(isValid, Is.True);
        Assert.That(events.OnAuthenticationFailure, Is.EqualTo(HmacEventsDefaults.OnAuthenticationFailure));
        Assert.That(events.OnAuthenticationSuccess, Is.EqualTo(HmacEventsDefaults.OnAuthenticationSuccess));
    }
}