using System.Security.Claims;
using System.Text;
using HmacManager.Mvc;
using HmacManager.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddMemoryCache()
    .AddAuthentication()
    .AddHmac(options =>
    {
        options.AddPolicy("HmacPolicy_1", policy =>
        {
            policy.UsePublicKey(Guid.Parse("eb8e9dae-08bd-4883-80fe-1d9a103b30b5"));
            policy.UsePrivateKey(Convert.ToBase64String(Encoding.UTF8.GetBytes("thisIsMySuperCoolHmacPolicy_1PrivateKey")));
            policy.UseMemoryCache(30);
            policy.AddScheme("HmacScheme_1", scheme =>
            {
                scheme.AddHeader("X-Scheme_1");
                scheme.AddHeader("X-Scheme_2");
            });
        });

        options.AddPolicy("HmacPolicy_2", policy =>
        {
            policy.UsePublicKey(Guid.Parse("ac2f1dae-08bd-4883-80fe-1d9a103b30b5"));
            policy.UsePrivateKey(Convert.ToBase64String(Encoding.UTF8.GetBytes("thisIsMySuperCoolHmacPolicy_2PrivateKey")));
            policy.UseMemoryCache(30);
            policy.AddScheme("HmacScheme_2", scheme =>
            {
                scheme.AddHeader("X-Scheme_1");
                scheme.AddHeader("X-Scheme_2");
            });
        });

        // Subscribe to HmacEvents to handle
        // successes and failures
        options.Events = new HmacEvents
        {
            OnValidateKeysAsync = (context, keys) => 
            {
                // Validate keys against database
                return Task.FromResult(true);
            },
            OnAuthenticationSuccessAsync = (context, hmacResult) =>
            {
                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, "MyName"),
                    new Claim(ClaimTypes.NameIdentifier, "MyNameId"),
                    new Claim(ClaimTypes.Email, "MyEmail")
                };

                return Task.FromResult(claims);
            },
            OnAuthenticationFailureAsync = (context, hmacResult) => Task.FromResult(new Exception("An error occurred authenticating request."))
        };
    });

builder.Services.AddAuthorization(options => 
{
    // This is one way to register
    // options.AddPolicy("Require_Hmac_PolicyScheme_2", policy => 
    //     policy.AddRequirements(new HmacAuthenticateAttribute 
    //     { 
    //         Policy = "HmacPolicy_2", 
    //         Scheme = "HmacScheme_2"
    //     }));

    // This is another
    // options.AddPolicy("Require_Hmac_PolicyScheme_2", policy =>
    // {
    //     policy.RequireHmacPolicy(`"HmacPolicy_2");
    //     policy.RequireHmacScheme("HmacScheme_2");
    // });

    // This is the preferred approach due to it's simplicity
    options.AddPolicy("Require_Hmac_PolicyScheme_2", policy =>
    {
        policy.RequireHmacAuthentication("HmacPolicy_2", "HmacScheme_2");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
