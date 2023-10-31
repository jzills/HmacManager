using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Console;
using HmacManagement.Caching;
using HmacManagement.Components;
using HmacManagement.Policies;

var policies = new HmacPolicyCollection();
policies.AddPolicy("Policy1", options =>
{
    options.Keys.PublicKey = Guid.NewGuid();
    options.Keys.PrivateKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("myPrivateKey"));
    options.Algorithms.ContentAlgorithm = ContentHashAlgorithm.SHA256;
    options.Algorithms.SigningAlgorithm = SigningHashAlgorithm.HMACSHA256;
    options.Nonce.MaxAge = TimeSpan.FromMinutes(1);
    options.Nonce.CacheType = NonceCacheType.Memory;
    options.HeaderSchemes.AddHeaderScheme("Policy1_Scheme1", options =>
    {
        options.AddRequiredHeader("Header1");
        options.AddRequiredHeader("Header2");
    });
});

var caches = new NonceCacheCollection();
caches.Add(NonceCacheType.Memory, new NonceCacheMock());

var factory = new HmacManagerFactory(policies, caches);

var hmacManager = factory.Create("Policy1", "Policy1_Scheme1");

var request = new HttpRequestMessage(HttpMethod.Post, "/api/user/123")
{
    Content = new StringContent(JsonSerializer.Serialize(new { Id = 1, Name = "Joshua" }), new MediaTypeHeaderValue("application/json"))
};
request.Headers.Add("Header1", "Header1_Value");
request.Headers.Add("Header2", "Header2_Value");

var signingResult = await hmacManager.SignAsync(request);
var debug = signingResult;

var verificationResult = await hmacManager.VerifyAsync(request);
var debug2 = verificationResult;