
# WebToApiAuthenticationWithJsonConfiguration

This demo is an example of using [HmacManager](../../README.md) for authentication between two .NET applications. One, the [Web](Web) is signing requests to interact with protected [Api](Api) endpoints.

## Api

The call to `AddHmac` uses the overload accepting an `IConfigurationSection`. The name of the section, i.e. "Authentication" can be any name but the json structure must match.

    var options = builder.Configuration.GetSection("Authentication");
    builder.Services
        .AddAuthentication()
        .AddHmac(options);

See [WebToApiAuthentication/Api](../WebToApiAuthentication/Api/README.md) for more details.

## Web

The call to `AddHmacManager` uses the overload accepting an `IConfigurationSection`. Again, the name of the section can be anything.

    // The important piece is that the schema matches an array of policies.
    var section = builder.Configuration.GetSection("HmacManager");

    // Pass the configuration section instead of using the 
    // builder overload.
    builder.Services.AddHmacManager(section);

See [WebToApiAuthentication/Web](../WebToApiAuthentication/Web/README.md) for more details.