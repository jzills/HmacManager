using Client;
using HmacManager.Caching;
using HmacManager.Mvc;
using HmacManager.Mvc.Extensions;

var clientId = "b9926638-6b5c-4a79-a6ca-014d8b848172";
var clientSecret = "11Nv/n22OqU59f9376E//I2rA2+Yg6yRaI0W6YRK/G0=";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddTransient<MyClientHandler>();
builder.Services.AddHttpClient<IMyClient, MyClient>(options =>
{
    options.BaseAddress = new Uri("https://localhost:7058");
}).AddHttpMessageHandler<MyClientHandler>();

builder.Services.AddMemoryCache()
    .AddHmacManager(options =>
    {
        options.ClientId = clientId;
        options.ClientSecret = clientSecret;

        // options.HeaderSchemes.AddScheme("MyFirstScheme", options =>
        // {
        //     options.AddHeader("MyFirstRequiredHeader1");
        //     options.AddHeader("MyFirstRequiredHeader2");
        // });

        // options.HeaderSchemes.AddScheme("MySecondScheme", options =>
        // {
        //     options.AddHeader("MySecondRequiredHeader1");
        //     options.AddHeader("MySecondRequiredHeader2");
        // });
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
