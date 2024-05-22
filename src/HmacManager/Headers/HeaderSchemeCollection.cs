using System.Text.Json.Serialization;
using HmacManager.Common;
using HmacManager.Policies;

namespace HmacManager.Headers;

[JsonConverter(typeof(HeaderSchemeCollectionJsonConverter))]
public class HeaderSchemeCollection : ComponentCollection<HeaderScheme>
{
    protected IValidator<HeaderScheme> Validator => new HeaderSchemeValidator();

    public void Add(HeaderScheme scheme)
    {
        var validationResult = Validator.Validate(scheme);
        if (validationResult.IsValid)
        {
            Add(scheme.Name, scheme); 
        }
        else
        {
            throw validationResult.GetError();
        }
    }
}