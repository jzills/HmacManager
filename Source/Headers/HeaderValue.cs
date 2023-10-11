namespace HmacManagement.Headers;

public class HeaderValue : Header
{
    public HeaderValue(
        string name, 
        string claimType, 
        string value
    ) : base(name, claimType) => Value = value;

    public readonly string Value;
}