namespace HmacManager.Headers;

public record HeaderValue(string Name, string? ClaimType, string? Value) : Header(Name, ClaimType)
{
    public HeaderValue(string name, string? value) 
        : this(name, null, value)
    {
        
    }

    public HeaderValue(KeyValuePair<string, string?> keyValuePair) 
        : this(keyValuePair.Key, null, keyValuePair.Value)
    {
        
    }
}