
# HmacManager


### Register without automatic authentication handlers

Use the IServiceCollection extension method AddHmacManager to add IHmacManagerFactory to the dependency injection container. 

    builder.Services
        .AddHmacManager(options =>
        {
            options.AddPolicy("SomePolicy", policy =>
            {
                policy.UsePublicKey(...);
                policy.UsePrivateKey(...);
                policy.UseInMemoryCache(...);
            });
        });

Access configured policies from IHmacManagerFactory

    private readonly IHmacManager _hmacManager;

    public MyService(IHmacManagerFactory hmacManagerFactory)
    {
        _hmacManager = hmacManagerFactory.Create("SomePolicy");
    }