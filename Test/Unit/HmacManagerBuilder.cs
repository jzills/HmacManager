using HmacManagement.Caching;
using HmacManagement.Caching.Memory;
using HmacManagement.Components;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Unit;

public class HmacManagerBuilder
{
    private HmacManagerOptions _options = new();
    private IHmacProvider _provider = default!;
    private INonceCache _nonceCache = default!;


    public HmacManagerBuilder WithOptions(
        Action<HmacManagerOptions> configureOptions
    )
    {
        configureOptions.Invoke(_options);
        return this;
    }

    public HmacManagerBuilder WithProviderOptions(
        Action<HmacProviderOptions> configureOptions
    )
    {
        var options = new HmacProviderOptions();
        configureOptions.Invoke(options);

        _provider = new HmacProvider(options);
        return this;
    }

    public HmacManagerBuilder WithNonceCacheOptions(
        Action<NonceCacheOptions> configureOptions
    )
    {
        var nonceCacheOptions = new NonceCacheOptions();
        configureOptions.Invoke(nonceCacheOptions);

        var memoryCache = new MemoryCache(
            Options.Create<MemoryCacheOptions>(
                new MemoryCacheOptions()));

        _nonceCache = new NonceMemoryCache(memoryCache, nonceCacheOptions);
        return this;
    }
    
    public IHmacManager Build() => 
        new HmacManager(
            _options, 
            _nonceCache, 
            _provider
        );
}