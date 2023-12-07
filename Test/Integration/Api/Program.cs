using System.Security.Claims;
using System.Text;
using HmacManagement.Mvc;
using HmacManagement.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddAuthentication()
    .AddHmac(options =>
    {
        options.AddPolicy("MyPolicy_1", policy =>
        {
            policy.UsePublicKey(Guid.Parse("eb8e9dae-08bd-4883-80fe-1d9a103b30b5"));
            policy.UsePrivateKey(Convert.ToBase64String(Encoding.UTF8.GetBytes("thisIsMySuperCoolPrivateKey")));
            policy.UseInMemoryCache(TimeSpan.FromSeconds(30));
            policy.AddScheme("AccountEmailScheme", scheme =>
            {
                scheme.AddHeader("X-Account-Id", "AccountId");
                scheme.AddHeader("X-Email", "Email");
            });
        });

        options.Events = new HmacEvents
        {
            OnAuthenticationSuccess = context =>
            {
                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, "MyName"),
                    new Claim(ClaimTypes.NameIdentifier, "MyNameId"),
                    new Claim(ClaimTypes.Email, "MyEmail")
                };

                return claims;
            },
            OnAuthenticationFailure = context =>
            {
                return new Exception("My Test Exception");
            }
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

app.UseAuthorization();

app.MapControllers();

app.Run();
