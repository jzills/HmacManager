namespace HmacManager.Components;

public interface IHmacResultFactory
{
    HmacResult Success(Hmac hmac);
    HmacResult Failure();
}

public class HmacResultFactory : IHmacResultFactory
{
    protected readonly string Policy;
    protected readonly string HeaderScheme;
    
    public HmacResultFactory(string policy, string? headerScheme = null)
    {
        Policy = policy;
        HeaderScheme = headerScheme;
    }

    public HmacResult Success(Hmac hmac) =>
        new HmacResult
        {
            Policy = Policy,
            HeaderScheme = HeaderScheme,
            Hmac = hmac,
            IsSuccess = true
        };

    public HmacResult Failure() =>
        new HmacResult
        {
            Policy = Policy,
            HeaderScheme = HeaderScheme,
            Hmac = null,
            IsSuccess = false
        };
}