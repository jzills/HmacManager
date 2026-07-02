using System.Text;
using HmacManager.Mvc.Extensions.Internal;
using HmacManager.Policies;
using HmacManager.Policies.Extensions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Unit.Tests.Mvc.Extensions.Internal;

[TestFixture]
public class Test_HmacPolicyCollectionReloader
{
    private string Path = null!;

    [SetUp]
    public void SetUp() => Path = System.IO.Path.GetTempFileName();

    [TearDown]
    public void TearDown() => File.Delete(Path);

    [Test]
    public void Reload_AddsNewAndReplacesChangedAndRemovesMissingPolicies()
    {
        File.WriteAllText(Path, BuildJson(("PolicyA", "00000000-0000-0000-0000-000000000001", "key-a-v1")));

        var configurationRoot = new ConfigurationBuilder()
            .AddJsonFile(Path, optional: false, reloadOnChange: false)
            .Build();

        var section = configurationRoot.GetSection("HmacManager");
        var policies = section.GetPolicySection();
        _ = new HmacPolicyCollectionReloader(section, policies);

        Assert.IsTrue(policies.TryGetValue("PolicyA", out var policyA));
        Assert.AreEqual(Encode("key-a-v1"), policyA.Keys.PrivateKey);

        // Rotate PolicyA's key and introduce PolicyB.
        File.WriteAllText(Path, BuildJson(
            ("PolicyA", "00000000-0000-0000-0000-000000000001", "key-a-v2"),
            ("PolicyB", "00000000-0000-0000-0000-000000000002", "key-b-v1")));
        configurationRoot.Reload();

        Assert.IsTrue(policies.TryGetValue("PolicyA", out var rotatedPolicyA));
        Assert.AreEqual(Encode("key-a-v2"), rotatedPolicyA.Keys.PrivateKey);
        Assert.IsTrue(policies.TryGetValue("PolicyB", out var policyB));
        Assert.AreEqual(Encode("key-b-v1"), policyB.Keys.PrivateKey);
        CollectionAssert.AreEquivalent(new[] { "PolicyA", "PolicyB" }, policies.Values.Select(p => p.Name));

        // Remove PolicyA entirely.
        File.WriteAllText(Path, BuildJson(("PolicyB", "00000000-0000-0000-0000-000000000002", "key-b-v1")));
        configurationRoot.Reload();

        Assert.IsFalse(policies.TryGetValue("PolicyA", out _));
        CollectionAssert.AreEquivalent(new[] { "PolicyB" }, policies.Values.Select(p => p.Name));
    }

    [Test]
    public void Reload_WithConfigurationThatFailsToBind_LeavesPreviousPoliciesUnchanged()
    {
        File.WriteAllText(Path, BuildJson(("PolicyA", "00000000-0000-0000-0000-000000000001", "key-a-v1")));

        var configurationRoot = new ConfigurationBuilder()
            .AddJsonFile(Path, optional: false, reloadOnChange: false)
            .Build();

        var section = configurationRoot.GetSection("HmacManager");
        var policies = section.GetPolicySection();
        _ = new HmacPolicyCollectionReloader(section, policies);

        // An empty "HmacManager" section fails to bind any policies.
        File.WriteAllText(Path, "{}");
        configurationRoot.Reload();

        Assert.IsTrue(policies.TryGetValue("PolicyA", out var policyA));
        Assert.AreEqual(Encode("key-a-v1"), policyA.Keys.PrivateKey);
    }

    private static string Encode(string value) => Convert.ToBase64String(Encoding.UTF8.GetBytes(value));

    private static string BuildJson(params (string Name, string PublicKey, string PrivateKey)[] policies)
    {
        var entries = policies.Select(policy => $$"""
            {
                "Name": "{{policy.Name}}",
                "Keys": {
                    "PublicKey": "{{policy.PublicKey}}",
                    "PrivateKey": "{{Encode(policy.PrivateKey)}}"
                }
            }
            """);

        return $$"""{ "HmacManager": [{{string.Join(",", entries)}}] }""";
    }
}
