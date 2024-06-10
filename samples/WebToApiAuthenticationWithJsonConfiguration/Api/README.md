
# Example

The call to `AddHmac` uses the overload accepting an `IConfigurationSection`. The name of the section, i.e. "Authentication" can be any name but the json structure must match.

    var options = builder.Configuration.GetSection("Authentication");
    builder.Services
        .AddAuthentication()
        .AddHmac(options);

See [WebToApiAuthentication/Api](../../WebToApiAuthentication/Api/README.md) for more details.