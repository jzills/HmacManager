namespace HmacManager.Headers;

public record HeaderValue(string Name, string ClaimType, string Value) : Header(Name, ClaimType);