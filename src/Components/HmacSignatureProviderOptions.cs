using HmacManager.Policies;

namespace HmacManager.Components;

public class HmacSignatureProviderOptions
{
    public required KeyCredentials Keys { get; set; }
    public required Algorithms Algorithms { get; set; }
    public required ContentHashGenerator ContentHashGenerator { get; set; }
    public required SignatureHashGenerator SignatureHashGenerator { get; set; }
    public SigningContentBuilder SigningContentBuilder { get; set; } = new SigningContentBuilderValidated();
}