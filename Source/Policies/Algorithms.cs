using HmacManagement.Components;

namespace HmacManagement.Policies;

public class Algorithms
{
    public ContentHashAlgorithm ContentAlgorithm { get; set; } = ContentHashAlgorithm.SHA256;
    public SigningHashAlgorithm SigningAlgorithm { get; set; } = SigningHashAlgorithm.HMACSHA256;
}