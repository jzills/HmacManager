using HmacManager.Kubernetes;
using HmacManager.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHmacManager(builder.Configuration.GetSection("HmacManager"));
builder.Services.AddScoped<ExtAuthzHandler>();

var app = builder.Build();

app.MapPost("/check", async (HttpContext ctx, ExtAuthzHandler handler) =>
    await handler.CheckAsync(ctx));

app.Run();
