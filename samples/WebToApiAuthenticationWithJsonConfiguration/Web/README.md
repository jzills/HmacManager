
# Example

The call to `AddHmacManager` uses the overload accepting an `IConfigurationSection`. Again, the name of the section can be anything.

    // The important piece is that the schema matches an array of policies.
    var section = builder.Configuration.GetSection("HmacManager");

    // Pass the configuration section instead of using the 
    // builder overload.
    builder.Services.AddHmacManager(section);

See [WebToApiAuthentication/Web](../../WebToApiAuthentication/Web/README.md) for more details.