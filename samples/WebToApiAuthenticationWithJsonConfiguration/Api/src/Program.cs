using System.Security.Claims;
using HmacManager.Mvc;
using HmacManager.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configurationSection = builder.Configuration.GetSection("Authentication");
builder.Services
    .AddAuthentication()
    .AddHmac(configurationSection, new HmacEvents
    {
        OnValidateKeys = (context, keys) => 
        {
            // Validate keys against database
            return true;
        },
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
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
