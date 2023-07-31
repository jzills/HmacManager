namespace HmacManagement.Components;

public class HmacProviderOptions
{
    public required string ClientId { get; set; } 
    public required string ClientSecret { get; set; }
    public required ContentHashAlgorithm ContentHashAlgorithm { get; set; } = ContentHashAlgorithm.SHA256;
    public required SignatureHashAlgorithm SignatureHashAlgorithm { get; set; } = SignatureHashAlgorithm.HMACSHA256;
}
