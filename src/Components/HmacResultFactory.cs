namespace HmacManager.Components;

internal class HmacResultFactory : IHmacResultFactory
{
    protected readonly string Policy;
    protected readonly string? HeaderScheme;
    
    internal HmacResultFactory(string policy, string? headerScheme = null)
    {
        Policy = policy;
        HeaderScheme = headerScheme;
    }

    public HmacResult Success(Hmac hmac) => Create(isSuccess: true, hmac);

    public HmacResult Failure() => Create(isSuccess: false);

    private HmacResult Create(bool isSuccess, Hmac? hmac = null) =>
        new HmacResult(Policy, HeaderScheme, isSuccess, hmac);
}