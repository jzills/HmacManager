using HmacManager.Kubernetes;
using HmacManager.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHmacManager(builder.Configuration.GetSection("HmacManager"));
builder.Services.AddScoped<ExtAuthzHandler>();
builder.Services.AddScoped<SignHandler>();

var app = builder.Build();

app.MapPost("/check", async (HttpContext ctx, ExtAuthzHandler handler) =>
    await handler.CheckAsync(ctx));

if (app.Environment.IsDevelopment())
{
    app.MapPost("/sign", async (SignRequest request, SignHandler handler) =>
        await handler.SignAsync(request));
}

app.Run();
