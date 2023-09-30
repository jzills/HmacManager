namespace HmacManagement.Components;

public class HmacProviderOptions
{
    public string? ClientId { get; set; } 
    public string? ClientSecret { get; set; }
    public ContentHashAlgorithm ContentHashAlgorithm { get; set; } = ContentHashAlgorithm.SHA256;
    public SignatureHashAlgorithm SignatureHashAlgorithm { get; set; } = SignatureHashAlgorithm.HMACSHA256;
}