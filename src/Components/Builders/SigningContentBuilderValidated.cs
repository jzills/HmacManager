namespace HmacManager.Components;

public class SigningContentBuilderValidated : SigningContentBuilder
{
    public override SigningContentBuilder CreateBuilder() => new SigningContentBuilderValidated();

    public override string Build()
    {
        ArgumentNullException.ThrowIfNull(Context.Nonce, nameof(Context.Nonce));
        ArgumentNullException.ThrowIfNull(Context.Request, nameof(Context.Request));
        ArgumentNullException.ThrowIfNull(Context.PublicKey, nameof(Context.PublicKey));
        ArgumentNullException.ThrowIfNull(Context.DateRequested, nameof(Context.DateRequested));

        return base.Build();
    }
}