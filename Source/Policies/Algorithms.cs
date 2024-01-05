using HmacManager.Components;

namespace HmacManager.Policies;

public class Algorithms
{
    public ContentHashAlgorithm ContentHashAlgorithm { get; set; } = ContentHashAlgorithm.SHA256;
    public SigningHashAlgorithm SigningHashAlgorithm { get; set; } = SigningHashAlgorithm.HMACSHA256;
}