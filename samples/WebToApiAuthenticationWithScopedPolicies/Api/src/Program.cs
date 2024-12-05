using System.Security.Claims;
using System.Text;
using HmacManager.Mvc;
using HmacManager.Mvc.Extensions;
using HmacManager.Policies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddMemoryCache()
    .AddAuthentication()
    .AddHmac(options =>
    {
        options.EnableScopedPolicies(serviceProvider =>
        {
            // Create policy collection
            var policies = new HmacPolicyCollection();

            // Create policies
            var builder = new HmacPolicyBuilder("HmacPolicy_1");
            builder.UsePublicKey(Guid.Parse("eb8e9dae-08bd-4883-80fe-1d9a103b30b5"));
            builder.UsePrivateKey(Convert.ToBase64String(Encoding.UTF8.GetBytes("thisIsMySuperCoolHmacPolicy_1PrivateKey")));
            builder.UseMemoryCache(30);
            builder.AddScheme("HmacScheme_1", scheme =>
            {
                scheme.AddHeader("X-Scheme_1");
                scheme.AddHeader("X-Scheme_2");
            });
            
            // Add policy to the collection
            policies.Add(builder.Build());

            // Create policies
            builder = new HmacPolicyBuilder("HmacPolicy_2");
            builder.UsePublicKey(Guid.Parse("ac2f1dae-08bd-4883-80fe-1d9a103b30b5"));
            builder.UsePrivateKey(Convert.ToBase64String(Encoding.UTF8.GetBytes("thisIsMySuperCoolHmacPolicy_2PrivateKey")));
            builder.UseMemoryCache(30);
            builder.AddScheme("HmacScheme_2", scheme =>
            {
                scheme.AddHeader("X-Scheme_1");
                scheme.AddHeader("X-Scheme_2");
            });
            
            // Add policy to the collection
            policies.Add(builder.Build());

            // Return the collection
            return policies;
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