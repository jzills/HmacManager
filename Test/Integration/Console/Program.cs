using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using HmacManager.Components;
using HmacManager.Mvc;
using HmacManager.Mvc.Extensions;

var services = new ServiceCollection();
services.AddMemoryCache();
services.AddAuthentication()
    .AddHmac(options =>
    {
        options.AddPolicy("MyFirstPolicy", policy =>
        {
            policy.UsePublicKey(Guid.NewGuid());
            policy.UsePrivateKey(Convert.ToBase64String(Encoding.UTF8.GetBytes("mySuperLongString")));
            policy.UseContentHashAlgorithm(ContentHashAlgorithm.SHA256);
            policy.UseSigningHashAlgorithm(SigningHashAlgorithm.HMACSHA256);
            policy.UseInMemoryCache(TimeSpan.FromMinutes(1));

            policy.AddScheme("MyScheme1", scheme =>
            {
                scheme.AddHeader("MyHeaderForScheme1.1");
                scheme.AddHeader("MyHeaderForScheme1.2");
            });

            policy.AddScheme("MyScheme2", scheme =>
            {
                scheme.AddHeader("MyHeaderForScheme2.1");
                scheme.AddHeader("MyHeaderForScheme2.2");
            });
        });
    
        options.Events = new HmacEvents
        {
            OnAuthenticationSuccess = context =>
            {
                return new Claim[] { };
            },
            OnAuthenticationFailure = context =>
            {
                return new Exception();
            }
        };
    });

var serviceProvider = services.BuildServiceProvider();
var hmacFactory = serviceProvider.GetRequiredService<IHmacManagerFactory>();
var hmacManager = hmacFactory.Create("MyFirstPolicy", "MyScheme1");

var request = new HttpRequestMessage(HttpMethod.Post, "/api/users");
request.Headers.Add("MyHeaderForScheme1.1", "VALUE_1.1");
request.Headers.Add("MyHeaderForScheme1.2", "VALUE_1.2");

var signingResult = await hmacManager.SignAsync(request);
var verificationResult = await hmacManager.VerifyAsync(request);
var debug = verificationResult;