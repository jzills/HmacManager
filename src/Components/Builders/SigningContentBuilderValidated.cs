using HmacManager.Headers;
using HmacManager.Validation;
using HmacManager.Validation.Extensions;

namespace HmacManager.Components;

internal class SigningContentBuilderValidated : SigningContentBuilder
{
    protected readonly PublicKeyValidator PublicKeyValidator = new();
    protected readonly NonceValidator NonceValidator = new();
    protected readonly HeaderValueCollectionValidator HeaderValueCollectionValidator;

    public SigningContentBuilderValidated(HttpRequestMessage request) : base(request)
    {
        HeaderValueCollectionValidator = new HeaderValueCollectionValidator(request);
    }

    public override string Build()
    {
        PublicKeyValidator.ValidateOrThrow(PublicKey);
        NonceValidator.ValidateOrThrow(Nonce);

        if (HeaderValues.Any())
        {
            HeaderValueCollectionValidator.ValidateOrThrow(HeaderValues);
        }

        return base.Build();
    }
}