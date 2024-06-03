using System.Security.Claims;
using System.Text;
using HmacManager.Mvc;
using HmacManager.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    // HmacManager does not manage adding cache services to the DI container
    .AddMemoryCache()
    .AddAuthentication()
    .AddHmac(options =>
    {
        // The same setup as in the Web project for this demo
        options.AddPolicy("MyPolicy", policy =>
        {
            policy.UsePublicKey(Guid.Parse("eb8e9dae-08bd-4883-80fe-1d9a103b30b5"));
            policy.UsePrivateKey(Convert.ToBase64String(Encoding.UTF8.GetBytes("thisIsMySuperCoolPrivateKey")));
            policy.UseMemoryCache(30);
            policy.AddScheme("RequireAccountAndEmail", scheme =>
            {
                scheme.AddHeader("X-Account");
                scheme.AddHeader("X-Email");
            });
        });

        // Subscribe to HmacEvents to handle
        // successes and failures
        options.Events = new HmacEvents
        {
            OnAuthenticationSuccess = (context, hmacResult) =>
            {
                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, "MyName"),
                    new Claim(ClaimTypes.NameIdentifier, "MyNameId"),
                    new Claim(ClaimTypes.Email, "MyEmail")
                };

                return claims;
            },
            OnAuthenticationFailure = (context, hmacResult) => new Exception("An error occurred authenticating request.")
        };
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
