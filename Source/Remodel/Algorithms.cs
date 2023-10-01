using HmacManagement.Components;

namespace HmacManagement.Remodel;

public class Algorithms
{
    public ContentHashAlgorithm ContentAlgorithm { get; set; } = ContentHashAlgorithm.SHA256;
    public SignatureHashAlgorithm SignatureAlgorithm { get; set; } = SignatureHashAlgorithm.HMACSHA256;
}