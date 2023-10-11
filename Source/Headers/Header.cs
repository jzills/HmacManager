namespace HmacManagement.Headers;

public class Header
{
    public Header(string name, string claimType) => (Name, ClaimType) = (name, claimType);
    public readonly string Name;
    public readonly string ClaimType;
}