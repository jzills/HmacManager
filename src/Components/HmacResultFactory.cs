namespace HmacManager.Components;

public class HmacResultFactory : IHmacResultFactory
{
    protected readonly string Policy;
    protected readonly string HeaderScheme;
    
    public HmacResultFactory(string policy, string? headerScheme = null)
    {
        Policy = policy;
        HeaderScheme = headerScheme;
    }

    public HmacResult Success(Hmac hmac) => Create(isSuccess: true, hmac);

    public HmacResult Failure() => Create(isSuccess: false);

    private HmacResult Create(bool isSuccess, Hmac? hmac = null) =>
        new HmacResult(Policy, HeaderScheme) 
        { 
            Hmac = hmac, 
            IsSuccess = isSuccess 
        };
}