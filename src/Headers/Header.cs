namespace HmacManager.Headers;

public record Header
{
    public Header(string? name, string? claimType)
    {
        Name = name;
        ClaimType = claimType;

        if (string.IsNullOrWhiteSpace(ClaimType))
        {
            ClaimType = Name;
        }
    }

    public string? Name { get; init; }
    public string? ClaimType { get; init; }
}