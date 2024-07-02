namespace HmacManager.Components;

public class SigningContentBuilderAccessor : SigningContentBuilder
{
    protected readonly Func<SigningContentContext, string> SigningContentAccessor;

    public SigningContentBuilderAccessor(
        Func<SigningContentContext, string> signingContentAccessor
    ) => SigningContentAccessor = signingContentAccessor;

    public override SigningContentBuilder CreateBuilder() => new SigningContentBuilderAccessor(SigningContentAccessor);

    public override string Build()
    {
        var signingContent = SigningContentAccessor(Context);

        ArgumentException.ThrowIfNullOrWhiteSpace(signingContent);

        return signingContent;
    }
}